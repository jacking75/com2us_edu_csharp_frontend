using System;
using SqlKata.Execution;
using WebAPIServer.DataClass;
using WebAPIServer.DbOperations;
using WebAPIServer.Util;
using ZLogger;

namespace WebAPIServer.DbOperations;

public partial class GameDb : IGameDb
{
    // 메일 기본 데이터 로딩
    // Mail_Data 테이블에서 메일 기본 정보 가져오기
    public async Task<Tuple<ErrorCode, List<MailData>>> LoadMailDataAsync(Int64 userId, Int64 pageNumber)
    {
        var mailData = new List<MailData>();
        var mailsPerPage = _defaultSetting.MailsPerPage;

        try
        {
            mailData = await _queryFactory.Query("Mail_Data").Where("UserId", userId)
                                          .Where("IsDeleted", false).Where("ExpiredAt", ">", DateTime.Now)
                                          .OrderByDesc("MailId").Offset((pageNumber - 1) * mailsPerPage).Limit((int)mailsPerPage)
                                          .GetAsync<MailData>() as List<MailData>;

            return new Tuple<ErrorCode, List<MailData>>(ErrorCode.None, mailData);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.LoadMailDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "LoadMailData Exception");

            return new Tuple<ErrorCode, List<MailData>>(errorCode, mailData);
        }
    }

    // 메일 본문 및 포함 아이템 읽기
    // Mail_Data 테이블에서 본문내용, Mail_Item 테이블에서 아이템 정보 가져오기
    public async Task<Tuple<ErrorCode, string, List<MailItem>>> ReadMailAsync(Int64 mailId, Int64 userId)
    {
        try
        {
            // 메일 본문 가져오기
            (var errorCode, var content, var isRead) = await ReadMailContent(mailId, userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, string, List<MailItem>>(errorCode, null, null);
            }

            // 메일 아이템 가져오기
            var mailItem = await ReadMailItem(mailId);

            // 메일 읽음 처리
            await UpdateMailReadStatus(mailId, isRead);

            return new Tuple<ErrorCode, string, List<MailItem>>(ErrorCode.None, content, mailItem);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.ReadMailFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReadMailFail Exception");

            return new Tuple<ErrorCode, string, List<MailItem>>(errorCode, null, null);
        }
    }

    private async Task<Tuple<ErrorCode, string, bool>> ReadMailContent(Int64 mailId, Int64 userId)
    {
        var result = await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                                        .Where("UserId", userId).Where("IsDeleted", false)
                                        .Where("ExpiredAt", ">", DateTime.Now).Select("Content", "IsRead")
                                        .FirstOrDefaultAsync<(string content, bool isRead)>();

        if (result.content == null)
        {
            return new Tuple<ErrorCode, string, bool>(ErrorCode.ReadMailFailWrongData, null, false);
        }

        return new Tuple<ErrorCode, string, bool>(ErrorCode.None, result.content, result.isRead);
    }

    private async Task<List<MailItem>> ReadMailItem(Int64 mailId)
    {
        var mailItem = await _queryFactory.Query("Mail_Item").Where("MailId", mailId)
                                          .Select("ItemId", "ItemCode", "ItemCount", "IsReceived")
                                          .GetAsync<MailItem>() as List<MailItem>;

        return mailItem;
    }

    private async Task UpdateMailReadStatus(Int64 mailId, bool isRead)
    {
        if (isRead == true)
        {
            return;
        }

        await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                           .UpdateAsync(new { IsRead = true });
    }

    // 메일 아이템 수령
    // Mail_Item 테이블에서 아이템 정보 가져와서 User_Item 테이블에 추가
    public async Task<Tuple<ErrorCode, List<ItemInfo>>> ReceiveMailItemAsync(Int64 mailId, Int64 userId)
    {
        var itemInfo = new List<ItemInfo>();
        var errorCode = new ErrorCode();

        try
        {
            // 올바른 아이템 수령 요청인지 검증
            errorCode = await CheckReceiveableMailItem(mailId, userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, null);
            }

            // 아이템을 유저에게 지급
            (errorCode, itemInfo) = await InsertMailItemToUserItem(mailId, userId);
            if (errorCode != ErrorCode.None)
            {
                // 롤백
                await InsertMailItemToUserItemRollBack(userId, itemInfo);

                return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, null);
            }

            // 메일 아이템 수령 처리
            errorCode = await UpdateMailItemReceiveStatus(mailId);
            if (errorCode != ErrorCode.None)
            {
                // 롤백
                await InsertMailItemToUserItemRollBack(userId, itemInfo);

                return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, null);
            }

            return new Tuple<ErrorCode, List<ItemInfo>>(ErrorCode.None, itemInfo);
        }
        catch (Exception ex)
        {
            errorCode = ErrorCode.ReceiveMailItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReceiveMailItem Exception");

            return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, itemInfo);
        }
    }

    private async Task<ErrorCode> CheckReceiveableMailItem(Int64 mailId, Int64 userId)
    {
        var isCorrectMail = await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                                               .Where("UserId", userId).Where("IsDeleted", false)
                                               .Where("HasItem", true).Where("ExpiredAt", ">", DateTime.Now)
                                               .ExistsAsync();

        if (isCorrectMail == false)
        {
            return ErrorCode.ReceiveMailItemFailWrongData;
        }

        return ErrorCode.None;
    }

    private async Task<Tuple<ErrorCode, List<ItemInfo>>> InsertMailItemToUserItem(Int64 mailId, Int64 userId)
    {
        var itemInfo = new List<ItemInfo>();

        try
        {
           
            var mailItem = await _queryFactory.Query("Mail_Item").Where("MailId", mailId)
                                              .GetAsync<MailItem>() as List<MailItem>;

            foreach (MailItem item in mailItem)
            {
                (var errorCode, var newItem) = await InsertUserItemAsync(userId, item.ItemCode, item.ItemCount, item.ItemId);

                if (errorCode != ErrorCode.None)
                {
                    return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, itemInfo);
                }

                itemInfo.Add(newItem);
            }

            return new Tuple<ErrorCode, List<ItemInfo>>(ErrorCode.None, itemInfo);
        }
        catch (Exception ex)
        {
            //롤백
            await InsertMailItemToUserItemRollBack(userId, itemInfo);

            var errorCode = ErrorCode.InsertMailItemToUserItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "InsertMailItemToUserItem Exception");

            return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, null);
        }
    }

    private async Task<ErrorCode> UpdateMailItemReceiveStatus(Int64 mailId)
    {
        try
        {
            await _queryFactory.Query("Mail_Item").Where("MailId", mailId)
                               .UpdateAsync(new { IsReceived = true });

            await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                               .UpdateAsync(new { HasItem = false });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            // 롤백
            await _queryFactory.Query("Mail_Item").Where("MailId", mailId)
                               .UpdateAsync(new { IsReceived = false });

            var errorCode = ErrorCode.UpdateMailItemReceiveStatusFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "UpdateMailItemReceiveStatus Exception");

            return errorCode;
        }
    }

    private async Task InsertMailItemToUserItemRollBack(Int64 userId, List<ItemInfo> itemInfo)
    {
        foreach (ItemInfo delitem in itemInfo)
        {
            await DeleteUserItemAsync(userId, delitem.ItemId, delitem.ItemCount);
        }
    }

    // 메일 삭제
    // Mail_Data 테이블에서 논리적으로만 삭제
    public async Task<ErrorCode> DeleteMailAsync(Int64 mailId, Int64 userId)
    {
        try
        {
            var isCorrectRequest = await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                                                      .Where("UserId", userId).Where("IsDeleted", false)
                                                      .Select("UserId", "IsDeleted").ExistsAsync();

            if (isCorrectRequest == false)
            {
                return ErrorCode.DeleteMailFailWrongData;
            }
            else
            {
                await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                                   .UpdateAsync(new { IsDeleted = true });

                return ErrorCode.None;
            }
        }
        catch (Exception ex)
        {
            //롤백
            await _queryFactory.Query("Mail_Data").Where("MailId", mailId)
                               .UpdateAsync(new { IsDeleted = false });

            var errorCode = ErrorCode.DeleteMailFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "DeleteMail Exception");

            return errorCode;
        }
    }
    // 메일에 아이템 첨부
    // Mail_Item 테이블에 데이터 추가
    private async Task<ErrorCode> InsertItemIntoMailAsync(Int64 mailId, Int64 itemCode, Int64 itemCount)
    {
        try
        {
            var itemId = _idGenerator.CreateId();

            await _queryFactory.Query("Mail_Item").InsertAsync(new
            {
                ItemId = itemId,
                MailId = mailId,
                ItemCode = itemCode,
                ItemCount = itemCount
            });

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.InsertItemIntoMailFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "InsertItemIntoMail Exception");

            return errorCode;
        }
    }
}
