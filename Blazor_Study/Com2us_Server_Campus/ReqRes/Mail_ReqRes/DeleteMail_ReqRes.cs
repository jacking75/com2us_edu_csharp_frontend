using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class DeleteMailRequest : UserAuthRequest
{
    public Int64 MailId { get; set; }
    public Int64 UserId { get; set; }
}

public class DeleteMailResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}