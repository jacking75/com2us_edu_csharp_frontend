using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;
using WebAPIServer.ReqRes;

namespace WebAPIServer.ReqRes;

public class OpenMailBoxRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }

    [Range(1, int.MaxValue)]
    public Int64 PageNumber { get; set; }
}

public class OpenMailBoxResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public List<MailData> mailData { get; set; }
}