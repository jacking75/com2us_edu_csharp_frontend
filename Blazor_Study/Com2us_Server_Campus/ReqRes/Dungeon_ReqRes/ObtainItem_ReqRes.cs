using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.ReqRes;

public class ObtainItemRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }

    public Int64 ItemCode { get; set; }

    [Range(1,int.MaxValue)]
    public Int64 ItemCount { get; set; }
}

public class ObtainItemResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}