using System.Data;
using IdGen;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SqlKata.Execution;
using WebAPIServer.DataClass;
using WebAPIServer.Util;
using ZLogger;

namespace WebAPIServer.DbOperations;

public partial class GameDb : IGameDb
{
    readonly ILogger<GameDb> _logger;
    readonly IMasterDb _masterDb;
    readonly IIdGenerator<long> _idGenerator;
    readonly IConfiguration _configuration;
    readonly DefaultSetting _defaultSetting;

    IDbConnection _dbConn;
    QueryFactory _queryFactory;

    public GameDb(ILogger<GameDb> logger, IMasterDb masterDb, IIdGenerator<long> idGenerator, IConfiguration configuration, DefaultSetting defaultSetting)
    {
        _logger = logger;
        _masterDb = masterDb;
        _idGenerator = idGenerator;
        _configuration = configuration;
        _defaultSetting = defaultSetting;

        var DbConnectString = _configuration.GetSection("DBConnection")["GameDb"];
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

    // 유저 기본 데이터 생성
    // User_BasicInformation 테이블에 유저 추가 / User_Item 테이블에 아이템 추가
    public async Task<ErrorCode> CreateUserDefaultDataAsync(Int64 accountId)
    {
        var userId = new Int64();

        try
        {
            // User_BasicInformation 테이블에 유저 초기 기본 데이터 추가
            userId = await _queryFactory.Query("User_BasicInformation")
                                        .InsertGetIdAsync<Int64>(new { accountId = accountId });

            // User_Attendance 테이블에 유저 출석정보 데이터 추가
            await _queryFactory.Query("User_Attendance")
                               .InsertAsync(new { UserId = userId });

            // User_Item 테이블에 유저 초기 아이템 데이터 추가
            var errorCode = await CreateUserDefaultItemAsync(userId);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            // 롤백 
            await CreateUserDefaultDataRollBack(userId);

            var errorCode = ErrorCode.CreateBasicDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "CreateBasicData Exception");

            return errorCode;
        }
    }

    private async Task<ErrorCode> CreateUserDefaultItemAsync(Int64 userId)
    {
        List<(Int64, Int64)> defaultitem = new List<(Int64, Int64)> { (2, 1), (4, 1), (5, 1), (6, 100) };
        foreach ((Int64 itemCode, Int64 count) in defaultitem)
        {
            (var errorCode, var useritem) = await InsertUserItemAsync(userId, itemCode, count);
            if (errorCode != ErrorCode.None)
            {
                // 롤백
                await CreateUserDefaultDataRollBack(userId);

                return errorCode;
            }
        }

        return ErrorCode.None;
    }

    private async Task CreateUserDefaultDataRollBack(Int64 userId)
    {
        if (userId == 0)
        {
            return;
        }

        // 각 테이블에서 User 데이터 모두 제거
        await _queryFactory.Query("User_BasicInformation").Where("UserId", userId).DeleteAsync();
        await _queryFactory.Query("User_Attendance").Where("UserId", userId).DeleteAsync();
        await _queryFactory.Query("User_Item").Where("UserId", userId).DeleteAsync();
    }

    // 유저 데이터에 아이템 추가
    // User_Item 테이블에 아이템 추가
    private async Task<Tuple<ErrorCode, ItemInfo>> InsertUserItemAsync(Int64 userId, Int64 itemCode, Int64 itemCount, Int64 itemId = 0)
    {
        var itemInfo = new ItemInfo();

        try
        {
            var itemData = _masterDb.ItemInfo.Find(i => i.Code == itemCode);

            if (itemData.Attribute == 5) // Attribute가 5면 Money를 뜻함
            {
                await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                                   .IncrementAsync("Money", (int)itemCount);
            }
            else
            {
                // itemId가 미리 부여된 아이템이 아닌경우 새로 부여
                if (itemId == 0)
                {
                    itemId = _idGenerator.CreateId();
                }

                await _queryFactory.Query("User_Item").InsertAsync(new
                {
                    ItemId = itemId,
                    UserId = userId,
                    ItemCode = itemCode,
                    ItemCount = itemCount,
                    Attack = itemData.Attack,
                    Defence = itemData.Defence,
                    Magic = itemData.Magic
                });
            }

            itemInfo.ItemCode = itemCode;
            itemInfo.ItemCount = itemCount;

            return new Tuple<ErrorCode, ItemInfo>(ErrorCode.None, itemInfo);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.InsertItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "InsertUserItem Exception");          
            
            return new Tuple<ErrorCode, ItemInfo>(errorCode, itemInfo);
        }
    }

    // 유저 데이터에서 아이템 제거
    // User_Item 테이블에서 아이템 제거
    private async Task<ErrorCode> DeleteUserItemAsync(Int64 userId, Int64 itemId, Int64 itemCount = 0)
    {
        try
        {
            Int64 isDeleted;

            if (itemId == 0) // itemId가 0이면 Money를 뜻함
            {
                isDeleted = await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                                               .DecrementAsync("Money", (int)itemCount);
            }
            else
            {
                isDeleted = await _queryFactory.Query("User_Item").Where("UserId", userId)
                                               .Where("ItemId", itemId).DeleteAsync();
            }

            if (isDeleted == 0)
            {
                return ErrorCode.DeleteItemFailWrongData;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.DeleteItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "DeleteUserItem Exception");
            
            return ErrorCode.DeleteItemFailException;
        }
    }

    // 유저 기본 데이터 로딩
    // User_BasicInformation 테이블에서 유저 기본 정보 가져오기
    public async Task<Tuple<ErrorCode, UserData>> UserDataLoading(Int64 accountId)
    {
        var userData = new UserData();

        try
        {
            userData = await _queryFactory.Query("User_BasicInformation").Where("accountId", accountId)
                                          .FirstOrDefaultAsync<UserData>();

            return new Tuple<ErrorCode, UserData>(ErrorCode.None, userData);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.UserDataLoadingFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "UserDataLoading Exception");

            return new Tuple<ErrorCode, UserData>(errorCode, userData);
        }
    }

    // 유저 아이템 로딩
    // User_Item 테이블에서 유저 아이템 정보 가져오기
    public async Task<Tuple<ErrorCode, List<UserItem>>> UserItemLoadingAsync(Int64 userId)
    {
        var userItem = new List<UserItem>();

        try
        {
            userItem = await _queryFactory.Query("User_Item").Where("UserId", userId)
                                          .Where("IsDestroyed", false).GetAsync<UserItem>() as List<UserItem>;

            return new Tuple<ErrorCode, List<UserItem>>(ErrorCode.None, userItem);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.UserItemLoadingFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "UserItemLoading Exception");

            return new Tuple<ErrorCode, List<UserItem>>(errorCode, userItem);
        }
    } 
}