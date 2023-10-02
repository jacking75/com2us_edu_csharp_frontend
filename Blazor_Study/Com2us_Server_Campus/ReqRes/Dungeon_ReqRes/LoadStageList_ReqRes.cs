using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class LoadStageListRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
}

public class LoadStageListResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public List<ClearData> ClearStage { get; set; }
}