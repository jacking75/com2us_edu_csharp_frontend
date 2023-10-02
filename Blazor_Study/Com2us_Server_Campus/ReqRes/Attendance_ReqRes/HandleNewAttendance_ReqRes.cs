using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class HandleNewAttendanceRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
}

public class HandleNewAttendanceResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}