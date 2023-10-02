namespace WebAPIServer.Controllers.ManagingController;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ManagingTool.Shared.DTO;
using System.Net.Http;


[ApiController]
[Route("api/[controller]")]
public class MailData : ControllerBase
{
    readonly ILogger<MailData> _logger;
    readonly HttpClient _httpClient;

    public MailData(ILogger<MailData> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost("SendMail")]
	public async Task<SendMailResponse> Post(SendMailRequest request)
	{
        var response = await _httpClient.PostAsJsonAsync("Managing/MailData/SendMail", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<SendMailResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }

    [HttpPost("GetUserMailList")]
    public async Task<GetUserMailListResponse> Post(GetUserMailListRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("Managing/MailData/GetUserMailList", request);
        var responseDTO = await response.Content.ReadFromJsonAsync<GetUserMailListResponse>();

        if (responseDTO == null)
        {
            // errorlog
        }

        return responseDTO;
    }
}
