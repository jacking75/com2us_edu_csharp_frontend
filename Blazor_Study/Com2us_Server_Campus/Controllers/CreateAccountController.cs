using System.ComponentModel.DataAnnotations;
using WebAPIServer.DbOperations;
using WebAPIServer.ReqRes;
using WebAPIServer.Util;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;

namespace WebAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccount: ControllerBase
{
    readonly ILogger<CreateAccount> _logger;
    readonly IAccountDb _accountDb;
    readonly IGameDb _gameDb;

    public CreateAccount(ILogger<CreateAccount> logger, IAccountDb accountDb, IGameDb gameDb)
    {
        _logger = logger;
        _accountDb = accountDb;
        _gameDb = gameDb;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> Post(CreateAccountRequest request)
    {
        var response = new CreateAccountResponse();
        var errorCode = new ErrorCode();

        // 계정 정보 생성
        // Account 테이블에 계정 추가 
        (errorCode, response.AccountId) = await _accountDb.CreateAccountAsync(request.Email, request.Password);
        if (errorCode != ErrorCode.None)
        {
            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "CreateAccount Error");

            response.Result = errorCode;
            return response;
        }

        // 유저 기본 데이터 생성
        // User_BasicInformation 테이블에 유저 추가 / User_Item 테이블에 아이템 추가
        errorCode = await _gameDb.CreateUserDefaultDataAsync(response.AccountId);
        if (errorCode != ErrorCode.None)
        {
            // 롤백
            await _accountDb.DeleteAccountAsync(request.Email);

            _logger.ZLogErrorWithPayload(LogManager.MakeEventId(errorCode), new { Email = request.Email }, "CreateAccount Error");

            response.Result = errorCode;
            return response;
        }

        return response;
    }
}