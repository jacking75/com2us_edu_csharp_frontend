using System;
using WebAPIServer.DataClass;
using WebAPIServer.ReqRes;

namespace WebAPIServer.DbOperations;

public interface IMasterDb
{
    VersionData VersionDataInfo { get; }
    List<Item> ItemInfo { get; }
    List<ItemAttribute> ItemAttributeInfo { get; }
    List<AttendanceReward> AttendanceRewardInfo { get; }
    List<InAppProduct> InAppProductInfo { get; }
    List<StageItem> StageItemInfo { get; }
    List<StageEnemy> StageEnemyInfo { get; }
    List<ExpTable> ExpTableInfo { get; }

    public Task<ErrorCode> Init();

    public Task<ErrorCode> VerifyVersionDataAsync(double appVersion, double masterVersion);

    public Task<Tuple<ErrorCode, GetItemTableResponse>> GetItemTableAsync();
}

