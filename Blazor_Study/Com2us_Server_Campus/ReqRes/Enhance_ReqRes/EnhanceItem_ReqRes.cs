using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class EnhanceItemRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
    public Int64 ItemId { get; set; }
}

public class EnhanceItemResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public UserItem userItem { get; set; }
}