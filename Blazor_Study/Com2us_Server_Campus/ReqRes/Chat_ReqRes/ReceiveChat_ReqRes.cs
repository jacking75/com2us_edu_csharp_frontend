using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class ReceiveChatRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
}

public class ReceiveChatResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public List<ChatMessage> ChatHistory { get; set; }
}