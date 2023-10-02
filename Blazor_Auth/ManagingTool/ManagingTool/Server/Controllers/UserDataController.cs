using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using ManagingTool.Client;
using System.Text.Json;
using System.Text;

[Authorize(Policy = "AccountIdPolicy")]
[ApiController]
[Route("api/[controller]")]
public class UserData
{
    static bool _dummyDataCreated = false;
    static List<UserInfo> _dummyUserdataList = new List<UserInfo>();
    static List<UserItem> _dummyItemdataList = new List<UserItem>();
    static List<MailData> _dummyMailDataList = new List<MailData>();

    public UserData()
    {
        if (_dummyDataCreated == false)
        {
            CreateDummyData();
            _dummyDataCreated = true;
        }
    }

    [HttpPost("GetUserBasicInfo")]
	public GetUserBasicInfoListResponse GetUserBasicInfo(GetUserBasicInfoRequest request)
    {
        // UserID는 0부터 19까지만 가능
        if (request.UserID < 0 || request.UserID >= _dummyUserdataList.Count)
        {
            return new GetUserBasicInfoListResponse()
            {
                errorCode = ErrorCode.GetUserDataByUserIdFailException
            };
        }

        var response = new GetUserBasicInfoListResponse()
        {
            errorCode = ErrorCode.None,
            UserInfo = new List<UserInfo>() { _dummyUserdataList[(Int32)request.UserID] }
        };

        return response;
    }

    [HttpPost("GetUserItemList")]
    public GetUserItemListResponse GetUserItemList(GetUserItemListRequest request)
    {
        // 더미 아이템에 유저 ID 지정
        var itemList = new List<UserItem>();
        foreach (var item in _dummyItemdataList)
        {
            item.UserId = request.SearchValue;
            itemList.Add(item);
        }

        var response = new GetUserItemListResponse()
        {
            errorCode = ErrorCode.None,
            UserItem = itemList
        };

        return response;
    }

    [HttpPost("GetUserMailList")]
    public GetUserMailListResponse GetUserMailList(GetUserMailListRequest request)
    {
        // 더미 메일에 유저 ID 지정
        var mailList = new List<MailData>();
        foreach (var mail in _dummyMailDataList)
        {
            mail.UserId = request.UserID;
            mailList.Add(mail);
        }

        var response = new GetUserMailListResponse()
        {
            errorCode = ErrorCode.None,
            UserMail = mailList
        };

        return response;
    }

	[HttpPost("GetMultipleUserBasicInfo")]
	public GetUserBasicInfoListResponse GetMultipleUserBasicInfo(GetMultipleUserBasicInfoRequest request)
    {
        var response = new GetUserBasicInfoListResponse()
        {
            errorCode = ErrorCode.None
        };

        // 카테고리에 따라 더미데이터 필터링
        var filteredUserList = new List<UserInfo>();

        switch (request.Category)
        {
            case "UserID":
                filteredUserList = _dummyUserdataList
                    .Where(user => user.UserID >= request.MinValue && user.UserID <= request.MaxValue)
                    .ToList();
                break;

            case "Level":
                filteredUserList = _dummyUserdataList
                    .Where(user => user.Level >= request.MinValue && user.Level <= request.MaxValue)
                    .ToList();
                break;

            case "Money":
                filteredUserList = _dummyUserdataList
                    .Where(user => user.Money >= request.MinValue && user.Money <= request.MaxValue)
                    .ToList();
                break;

            case "BestClearStage":
                filteredUserList = _dummyUserdataList
                    .Where(user => user.BestClearStage >= request.MinValue && user.BestClearStage <= request.MaxValue)
                    .ToList();
                break;

            default:
                filteredUserList = _dummyUserdataList.ToList();
                break;
        }

        response.UserInfo = filteredUserList;

        return response;
    }

    void CreateDummyData()
    {
        for (var i = 0; i < 20; i++)
        {
            var user = new UserInfo();
            user.AccountID = new Random().Next(0, Int32.MaxValue);
            user.UserID = i;
            user.Level = new Random().Next(1, 100);
            user.Exp = new Random().Next(0, 1000000);
            user.Money = new Random().Next(0, 1000000);
            user.BestClearStage = new Random().Next(0, 100);
            user.LastLogin = DateTime.Now;

            _dummyUserdataList.Add(user);
        }

        for (var i = 0; i < 5; i++)
        {
            var item = new UserItem();
            item.ItemId = new Random().Next(0, Int32.MaxValue);
            item.ItemCode = new Random().Next(0, 100);
            item.ItemCount = new Random().Next(0, 10);
            item.Attack = new Random().Next(0, 100);
            item.Defence = new Random().Next(0, 100);
            item.Magic = new Random().Next(0, 100);
            item.EnhanceCount = new Random().Next(0, 10);
            item.IsDestroyed = new Random().Next(0, 2) == 0 ? false : true;
            item.ObtainedAt = DateTime.Now;

            _dummyItemdataList.Add(item);
        }

        for (var i = 0; i < 5; i++)
        {
            var mail = new MailData();
            mail.MailId = i;
            mail.SenderId = new Random().Next(0, 19);
            mail.Title = (i + 1) + "번째 메일";
            mail.Content = (i + 1) + "번째 메일입니다.";
            mail.IsRead = new Random().Next(0, 2) == 0 ? false : true;
            mail.HasItem = new Random().Next(0, 2) == 0 ? false : true;
            mail.IsDeleted = new Random().Next(0, 2) == 0 ? false : true;
            mail.ObtainedAt = DateTime.Now;
            mail.ExpiredAt = DateTime.Now.AddDays(7);

            _dummyMailDataList.Add(mail);
        }
    }
}
