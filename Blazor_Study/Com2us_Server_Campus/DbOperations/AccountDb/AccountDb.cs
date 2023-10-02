using System.Data;
using WebAPIServer.Controllers;
using WebAPIServer.DataClass;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;
using IdGen;
using WebAPIServer.Util;

namespace WebAPIServer.DbOperations;

public class AccountDb : IAccountDb
{
    readonly ILogger<AccountDb> _logger;
    readonly IIdGenerator<long> _idGenerator;

    IDbConnection _dbConn;
    QueryFactory _queryFactory;

    public AccountDb(ILogger<AccountDb> logger, IIdGenerator<long> idGenerator, IConfiguration configuration)
    {
        _logger = logger;
        _idGenerator = idGenerator;

        var DbConnectString = configuration.GetSection("DBConnection")["AccountDb"];
        _dbConn = new MySqlConnection(DbConnectString);

        var compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, compiler);
    }

    public void Dispose()
    {
        _queryFactory.Dispose();
        _dbConn.Dispose();
        GC.SuppressFinalize(this);
    }

    // 계정 정보 생성
    // Account 테이블에 계정 추가 
    public async Task<Tuple<ErrorCode, Int64>> CreateAccountAsync(string email, string password)
    {
        try
        {
            var accountId = _idGenerator.CreateId();
            var saltValue = Security.RandomString(64);
            var hashedPassword = Security.MakeHashedPassword(saltValue, password);
            
            await _queryFactory.Query("Account").InsertAsync(new
            {
                AccountId = accountId,
                Email = email,
                SaltValue = saltValue,
                HashedPassword = hashedPassword
            });

            return new Tuple<ErrorCode, Int64>(ErrorCode.None, accountId);
        }
        catch (Exception ex)
        {
            var errorCode = new ErrorCode();

            if (ex is MySqlException mysqlEx && mysqlEx.Number == 1062)
            {
                errorCode = ErrorCode.CreateAccountFailDuplicate;
            }
            else
            {
                errorCode = ErrorCode.CreateAccountFailException;
            }

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "CreateAccount Exception");

            return new Tuple<ErrorCode, Int64>(errorCode, 0);
        }
    }

    // 계정 정보 삭제
    // Account 테이블의 계정 삭제
    public async Task DeleteAccountAsync(string email)
    {
        await _queryFactory.Query("Account").Where("Email", email)
                           .DeleteAsync();
    }

    // 계정 정보 검증
    // Account 테이블의 데이터를 바탕으로 검증
    public async Task<Tuple<ErrorCode, Int64>> VerifyAccountAsync(string email, string password)
    {
        try
        {
            // Account 테이블에 email에 해당하는 레코드가 존재하는지 확인
            var accountInfo = await _queryFactory.Query("Account").Where("Email", email).FirstOrDefaultAsync<Account>();
            if (accountInfo is null || accountInfo.AccountId == null)
            {
                return new Tuple<ErrorCode, Int64>(ErrorCode.LoginFailUserNotExist, 0);
            }

            // SaltValue를 이용해 해시한 패스워드가 일치하는지 확_
            var hashedPassword = Security.MakeHashedPassword(accountInfo.SaltValue, password);
            if (hashedPassword != accountInfo.HashedPassword)
            {
                return new Tuple<ErrorCode, Int64>(ErrorCode.LoginFailPwNotMatch, 0);
            }

            return new Tuple<ErrorCode, Int64>(ErrorCode.None, accountInfo.AccountId);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.VerifyAccountFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "VerifyAccount Exception");

            return new Tuple<ErrorCode, Int64>(errorCode, 0);
        }
    }

}