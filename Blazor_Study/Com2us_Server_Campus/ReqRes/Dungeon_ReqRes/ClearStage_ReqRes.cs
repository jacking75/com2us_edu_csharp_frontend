using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;
using WebAPIServer.Util;

namespace WebAPIServer.ReqRes;

public class ClearStageRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }

    [Range(1, 10)]
    public Int64 ClearRank { get; set; }

    [TimeSpanRange("00:01:00", "10:00:00", ErrorMessage = "TimeSpan value is out of range.")]
    public TimeSpan ClearTime { get; set; }
}

public class ClearStageResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public List<ItemInfo> itemInfo { get; set; }
    public Int64 ObtainedExp { get; set; }
}