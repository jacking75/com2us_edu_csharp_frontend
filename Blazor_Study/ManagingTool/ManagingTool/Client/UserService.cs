using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using Microsoft.AspNetCore.Components;

namespace ManagingTool.Client;

public class UserService
{
    public static HttpClient _httpClient { get; set; }

    public async Task<GetUserBasicInfoListResponse> GetUserBasicInfo(Int64 userId)
    {
        var request = new GetUserBasicInfoRequest
        {
            UserID = userId
        };

        var response = await _httpClient.PostAsJsonAsync("api/UserData/GetUserBasicInfo", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetUserBasicInfoListResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }

    public async Task<List<UserInfo>> GetMultipleUserBasicInfo(string category, Int64 minValue, Int64 maxValue)
    {
        var request = new GetMultipleUserBasicInfoRequest
        {
            Category = category,
            MinValue = minValue,
            MaxValue = maxValue
        };

        var response = await _httpClient.PostAsJsonAsync("api/UserData/GetMultipleUserBasicInfo", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetUserBasicInfoListResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO.UserInfo;
    }

    public async Task<UpdateUserBasicInformationResponse> UpdateUserBasicInfo(UserInfo userInfo)
    {
        var request = new UpdateUserBasicInformationRequest
        {
            UserInfo = userInfo
        };

        var response = await _httpClient.PostAsJsonAsync("api/UserData/UpdateUserBasicInfo", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<UpdateUserBasicInformationResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }
}

