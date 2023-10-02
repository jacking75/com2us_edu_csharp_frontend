using System;
using Org.BouncyCastle.Asn1.Pkcs;
using SqlKata.Execution;
using WebAPIServer.DataClass;
using WebAPIServer.Util;
using ZLogger;
using WebAPIServer.ReqRes;
using IdGen;

namespace WebAPIServer.DbOperations;

public partial class GameDb : IGameDb
{
	// 유저 출석 데이터 로딩
	// User_BasicInformation 테이블에서 유저 출석 정보 가져온 뒤 새로운 출석인지 아닌지 판별
	public async Task<GetUserBasicInfoListResponse> GetUserBasicInfoAsync(Int64 userId)
	{
		var response = new GetUserBasicInfoListResponse
		{
			errorCode = ErrorCode.None
		};
		var userDataSet = new List<UserInfo>();
		try
		{
			var userData = await _queryFactory.Query("User_BasicInformation").Where("UserID", userId)
											  .FirstOrDefaultAsync<UserInfo>();

			if (userData == null)
			{
				userData = new UserInfo();
			}

			userDataSet.Add(userData);
			response.UserInfo = userDataSet;

			return response;
		}
		catch (Exception ex)
		{
			response.errorCode = ErrorCode.GetUserDataByUserIdFailException;

			_logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "GetUserDataFromUserId Exception");

			return response;
		}
	}

	public async Task<GetUserBasicInfoListResponse> GetMultipleUserBasicInfoAsync(string category, Int64 minValue, Int64 maxValue)
	{
        var response = new GetUserBasicInfoListResponse
        {
            errorCode = ErrorCode.None

        };
        try
		{
			var userDataSet = new List<UserInfo>();

			if (category == "UserID")
			{
				userDataSet = await _queryFactory.Query("User_BasicInformation").WhereBetween("UserId", minValue, maxValue)
												 .GetAsync<UserInfo>() as List<UserInfo>;
			}
			else if (category == "Level")
			{
                userDataSet = await _queryFactory.Query("User_BasicInformation").WhereBetween("Level", minValue, maxValue)
                                                 .GetAsync<UserInfo>() as List<UserInfo>;
            }
			else if (category == "Money")
			{
                userDataSet = await _queryFactory.Query("User_BasicInformation").WhereBetween("Money", minValue, maxValue)
                                                 .GetAsync<UserInfo>() as List<UserInfo>;
            }
			else if (category == "BestClearStage")
			{
                userDataSet = await _queryFactory.Query("User_BasicInformation").WhereBetween("BestClearStage", minValue, maxValue)
                                                 .GetAsync<UserInfo>() as List<UserInfo>;
            }

			if (userDataSet == null)
			{
				userDataSet = new List<UserInfo>();
			}

			response.UserInfo = userDataSet;

			return response;
		}
		catch(Exception ex)
		{
			response.errorCode = ErrorCode.GetUserDataByRangeFailException;

			_logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "GetUserDataByRange Exception");

			return response;
		}
	}

    public async Task<UpdateUserBasicInformationResponse> UpdateUserBasicInfoAsync(UserInfo userInfo)
    {
        var response = new UpdateUserBasicInformationResponse
        {
            errorCode = ErrorCode.None
        };
        var userDataSet = new List<UserInfo>();
        try
        {
			var isSuccess = await _queryFactory.Query("User_BasicInformation").Where("UserID", userInfo.UserID)
											   .UpdateAsync(new
											   {
												   Level = userInfo.Level,
												   Exp = userInfo.Exp,
												   Money = userInfo.Money,
												   BestClearStage = userInfo.BestClearStage
											   });

            if (isSuccess == 0)
            {
                // error
            }

            return response;
        }
        catch (Exception ex)
        {
            response.errorCode = ErrorCode.UpdateUserBasicInfoFailException;

            _logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "UpdateUserBasicInfo Exception");

            return response;
        }
    }

    public async Task<SendMailResponse> SendManagingMailAsync(MailForm mailForm, Int64 userId)
	{
		var mailId = _idGenerator.CreateId();
		var response = new SendMailResponse
		{
			errorCode = ErrorCode.None
		};

		try
		{
			var hasItem = false;
			if (mailForm.ItemCode != 0)
				hasItem = true;
			
			// 메일 본문 전송
			await _queryFactory.Query("Mail_Data").InsertAsync(new
			{
				MailId = mailId,
				UserId = userId,
				SenderId = 0,
				Title = mailForm.Title,
				Content = mailForm.Content,
				hasItem = hasItem,
				ExpiredAt = DateTime.Now.AddDays(7)
			});

			// 메일 아이템 전송
			if (hasItem == true)
			{
				response.errorCode = await InsertItemIntoMailAsync(mailId, mailForm.ItemCode, mailForm.ItemCount);
				if (response.errorCode != ErrorCode.None)
				{
					// 롤백
					await SendMailAttendanceRewardRollBack(mailId);

					return response;
				}
			}

			return response;
		}
		catch (Exception ex)
		{
			response.errorCode = ErrorCode.SendMailFailException;

			_logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "SendManagingMail Exception");

			return response;
		}
	}

    public async Task<GetUserItemListResponse> GetUserItemListAsync(string searchType, Int64 searchValue)
    {
		var response = new GetUserItemListResponse
		{
			errorCode = ErrorCode.None
        };

        var userItem = new List<UserItem>();

        try
        {
			if (searchType == "UserID")
			{
				response.UserItem = await _queryFactory.Query("User_Item").Where("UserId", searchValue)
													   .GetAsync<UserItem>() as List<UserItem>;
			}
			else if (searchType == "ItemID")
			{
                response.UserItem = await _queryFactory.Query("User_Item").Where("ItemId", searchValue)
                                                       .GetAsync<UserItem>() as List<UserItem>;
            }
			else if (searchType == "ItemCode")
			{
                response.UserItem = await _queryFactory.Query("User_Item").Where("ItemCode", searchValue)
                                                       .GetAsync<UserItem>() as List<UserItem>;
            }

			return response;
        }
        catch (Exception ex)
        {
            response.errorCode = ErrorCode.GetUserItemListFailException;

            _logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "GetUserItemList Exception");

			return response;
        }
    }

    public async Task<GetUserMailListResponse> GetUserMailListAsync(Int64 userId)
    {
        var response = new GetUserMailListResponse
        {
            errorCode = ErrorCode.None
        };

        var userItem = new List<UserItem>();

        try
        {
            response.UserMail = await _queryFactory.Query("mail_data").Where("UserId", userId)
                                                   .GetAsync<MailData>() as List<MailData>;

            return response;
        }
        catch (Exception ex)
        {
            response.errorCode = ErrorCode.GetUserMailListFailException;

            _logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "GetUserMailList Exception");

            return response;
        }
    }

    public async Task<RetrieveUserItemResponse> RetrieveUserItemAsync(List<Tuple<Int64, Int64>> selectedItemList, MailForm? mailForm)
    {
        var response = new RetrieveUserItemResponse
        {
            errorCode = ErrorCode.None
        };

        try
        {
			foreach (var item in selectedItemList)
			{
				await _queryFactory.Query("User_Item").Where("ItemId", item.Item1)
                                   .DeleteAsync();
				if (mailForm != null)
				{
					await _queryFactory.Query("Mail_Data").Where("UserId", item.Item2)
									   .InsertAsync(new
									   {
										   MailId = _idGenerator.CreateId(),
										   UserId = item.Item2,
										   SenderId = 0,
										   Title = mailForm.Title,
										   Content = mailForm.Content,
										   hasItem = false,
										   ExpiredAt = DateTime.Now.AddDays(7)
									   });
				}
			}

            return response;
        }
        catch (Exception ex)
        {
            response.errorCode = ErrorCode.RetrieveUserItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(response.errorCode), ex, "RetrieveUserItem Exception");

            return response;
        }
    }
}