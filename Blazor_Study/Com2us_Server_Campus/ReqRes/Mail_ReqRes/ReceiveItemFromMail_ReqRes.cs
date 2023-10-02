using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class ReceiveItemFromMailRequest : UserAuthRequest
{
    public Int64 MailId { get; set; }
    public Int64 UserId { get; set; }
}

public class ReceiveItemFromMailResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public List<ItemInfo> itemInfo { get; set; }
}