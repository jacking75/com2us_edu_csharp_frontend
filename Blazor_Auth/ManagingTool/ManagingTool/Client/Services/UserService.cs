using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace ManagingTool.Client;

public class UserService : BaseService
{
    public static HttpClient _httpClient { get; set; }

    public UserService(TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public async Task<GetUserBasicInfoListResponse> GetUserBasicInfo(long userId)
    {
        // request 생성
        var request = new GetUserBasicInfoRequest
        {
            UserID = userId
        };

        var requestMessage = await CreateReqMsg(HttpMethod.Post, "api/UserData/GetUserBasicInfo", request);

        // response 받아오고 재발급 토큰이 있으면 갱신
        var response = await _httpClient.SendAsync(requestMessage);
        await _tokenManager.UpdateAccessTokenIfPresent(response);

        // 성공
        if (response.IsSuccessStatusCode)
        {
            var responseDTO = await response.Content.ReadFromJsonAsync<GetUserBasicInfoListResponse>();
            if (responseDTO == null)
            {
                return new GetUserBasicInfoListResponse { errorCode = ErrorCode.EmptyResponse };
            }
            return responseDTO;
        }

        // 인증 실패
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new GetUserBasicInfoListResponse { errorCode = ErrorCode.Unauthorized };
        }

        return new GetUserBasicInfoListResponse { errorCode = ErrorCode.GetUserBasicInfoFail };
    }

    public async Task<List<UserInfo>> GetMultipleUserBasicInfo(string category, long minValue, long maxValue)
    {
        // request 생성
        var request = new GetMultipleUserBasicInfoRequest
        {
            Category = category,
            MinValue = minValue,
            MaxValue = maxValue
        };

        var requestMessage = await CreateReqMsg(HttpMethod.Post, "api/UserData/GetMultipleUserBasicInfo", request);

        // response 받아오고 재발급 토큰이 있으면 갱신
        var response = await _httpClient.SendAsync(requestMessage);
        await _tokenManager.UpdateAccessTokenIfPresent(response);

        // 성공
        if (response.IsSuccessStatusCode)
        {
            var responseDTO = await response.Content.ReadFromJsonAsync<GetUserBasicInfoListResponse>();
            if (responseDTO == null)
            {
                return new List<UserInfo>();
            }
            return responseDTO.UserInfo;
        }

        return new List<UserInfo>();
    }

    public async Task<GetUserItemListResponse> GetUserItemList(string searchType, long userId)
    {
        // request 생성
        var request = new GetUserItemListRequest
        {
            SearchType = searchType,
            SearchValue = userId
        };

        var requestMessage = await CreateReqMsg(HttpMethod.Post, "api/UserData/GetUserItemList", request);

        // response 받아오고 재발급 토큰이 있으면 갱신
        var response = await _httpClient.SendAsync(requestMessage);
        await _tokenManager.UpdateAccessTokenIfPresent(response);

        // 성공
        if (response.IsSuccessStatusCode)
        {
            var responseDTO = await response.Content.ReadFromJsonAsync<GetUserItemListResponse>();
            if (responseDTO == null)
            {
                return new GetUserItemListResponse { errorCode = ErrorCode.EmptyResponse };
            }
            return responseDTO;
        }

        // 인증 실패
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new GetUserItemListResponse { errorCode = ErrorCode.Unauthorized };
        }

        return new GetUserItemListResponse { errorCode = ErrorCode.GetUserItemListFail };
    }

    public async Task<GetUserMailListResponse> GetUserMailList(long userId)
    {
        // request 생성
        var request = new GetUserMailListRequest
        {
            UserID = userId
        };

        var requestMessage = await CreateReqMsg(HttpMethod.Post, "api/UserData/GetUserMailList", request);

        // response 받아오고 재발급 토큰이 있으면 갱신
        var response = await _httpClient.SendAsync(requestMessage);
        await _tokenManager.UpdateAccessTokenIfPresent(response);

        // 성공
        if (response.IsSuccessStatusCode)
        {
            var responseDTO = await response.Content.ReadFromJsonAsync<GetUserMailListResponse>();
            if (responseDTO == null)
            {
                return new GetUserMailListResponse { errorCode = ErrorCode.EmptyResponse };
            }
            return responseDTO;
        }

        // 인증 실패
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new GetUserMailListResponse { errorCode = ErrorCode.Unauthorized };
        }

        return new GetUserMailListResponse { errorCode = ErrorCode.GetUserMailListFail };
    }
}

