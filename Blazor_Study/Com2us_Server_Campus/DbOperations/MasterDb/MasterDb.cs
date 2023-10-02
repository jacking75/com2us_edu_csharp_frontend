using System.Collections.Generic;
using System.Data;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.Intrinsics.X86;
using System.Xml;
using MySqlConnector;
using SqlKata.Execution;
using WebAPIServer.DataClass;
using ZLogger;
using WebAPIServer.Util;

namespace WebAPIServer.DbOperations;

public partial class MasterDb : IMasterDb
{
    // 데이터베이스에서 마스터데이터 가져오기
    // 이후에는 마스터데이터와 관련된 것들은 여기서 사용

    readonly ILogger<MasterDb> _logger;

    public VersionData VersionDataInfo { get; set; }
    public List<Item> ItemInfo { get; set; }
    public List<ItemAttribute> ItemAttributeInfo { get; set; }
    public List<AttendanceReward> AttendanceRewardInfo { get; set; }
    public List<InAppProduct> InAppProductInfo { get; set; }
    public List<StageItem> StageItemInfo { get; set; }
    public List<StageEnemy> StageEnemyInfo { get; set; }
    public List<ExpTable> ExpTableInfo { get; set; }

    IDbConnection _dbConn;
    QueryFactory _queryFactory;

    public MasterDb(ILogger<MasterDb> logger, IConfiguration configuration)
    {
        _logger = logger;

        var DbConnectString = configuration.GetSection("DBConnection")["MasterDataDb"];
        _dbConn = new MySqlConnection(DbConnectString);

        var compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, compiler);
    }

    public async Task<ErrorCode> Init()
    {
        try
        {
            VersionDataInfo = await _queryFactory.Query("VersionData").FirstOrDefaultAsync<VersionData>();
            ItemInfo = await _queryFactory.Query("Item").GetAsync<Item>() as List<Item>;
            ItemAttributeInfo = await _queryFactory.Query("ItemAttribute").GetAsync<ItemAttribute>() as List<ItemAttribute>;
            AttendanceRewardInfo = await _queryFactory.Query("AttendanceReward").GetAsync<AttendanceReward>() as List<AttendanceReward>;
            InAppProductInfo = await _queryFactory.Query("InAppProduct").GetAsync<InAppProduct>() as List<InAppProduct>;
            StageItemInfo = await _queryFactory.Query("StageItem").GetAsync<StageItem>() as List<StageItem>;
            StageEnemyInfo = await _queryFactory.Query("StageEnemy").GetAsync<StageEnemy>() as List<StageEnemy>;
            ExpTableInfo = await _queryFactory.Query("ExpTable").GetAsync<ExpTable>() as List<ExpTable>;

            _queryFactory.Dispose();
            _dbConn.Dispose();

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.MasterDbInitFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "MasterDb Init Exception");

            return errorCode;
        }
    }

    // 게임 버전 검증
    // MasterData에서 가져온 데이터를 바탕으로 검증
    public async Task<ErrorCode> VerifyVersionDataAsync(double appVersion, double masterVersion)
    {
        try
        {
            if (VersionDataInfo.AppVersion != appVersion || VersionDataInfo.MasterVersion != masterVersion)
            {
                return ErrorCode.VerifyVersionDataFailVersionNotMatch;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.VerifyVersionDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "VerifyVersionData Exception");

            return errorCode;
        }
    }


}
