namespace WebAPIServer.ReqRes;

public class AbortStageRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
}

public class AbortStageResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}