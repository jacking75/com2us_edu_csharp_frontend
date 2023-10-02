using System.Net.Http.Json;
using ManagingTool.Shared.DTO;

namespace ManagingTool.Client;

public class MailService
{
    public static HttpClient _httpClient { get; set; }

    public async Task<SendMailResponse> SendMail(MailForm mailForm, Int64 userId)
    {
        var request = new SendMailRequest
        {
            MailForm = mailForm,
            UserID = userId
        };

        var response = await _httpClient.PostAsJsonAsync("api/MailData/SendMail", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<SendMailResponse>();

        return responseDTO;
    }

    public async Task<GetUserMailListResponse> GetUserMailList(Int64 userId)
    {
        var request = new GetUserMailListRequest
        {
            UserID = userId
        };

        var response = await _httpClient.PostAsJsonAsync("api/MailData/GetUserMailList", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetUserMailListResponse>();

        return responseDTO;
    }
}

