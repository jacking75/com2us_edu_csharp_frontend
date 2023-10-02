using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class ReadMailRequest : UserAuthRequest
{
    public Int64 MailId { get; set; }
    public Int64 UserId { get; set; }
}

public class ReadMailResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public string Content { get; set; }
    public List<MailItem> Item { get; set; }
}