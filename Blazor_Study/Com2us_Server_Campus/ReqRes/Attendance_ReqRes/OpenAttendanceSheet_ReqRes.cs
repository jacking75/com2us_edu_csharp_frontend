using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class OpenAttendanceSheetRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
}

public class OpenAttendanceSheetResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public Int64 attendanceCount { get; set; }
    public bool IsNewAttendance { get; set; }
}