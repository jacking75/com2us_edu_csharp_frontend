namespace ManagingTool.Shared.DTO;

public class SendMailRequest
{
    public MailForm MailForm { get; set; }
    public Int64 UserID { get; set; }
}

public class SendMailResponse
{
    public ErrorCode errorCode { get; set; }
}

public class MailForm
{
	public string Title { get; set; }
	public string Content { get; set; }
	public Int64? ItemCode { get; set; }
	public Int64 ItemCount { get; set; }
}

public class GetUserMailListRequest
{
    public Int64 UserID { get; set; }
}   

public class GetUserMailListResponse
{
    public ErrorCode errorCode { get; set; }
    public List<MailData> UserMail { get; set; }
}

public class MailData
{
    public Int64 MailId { get; set; }
    public Int64 UserId { get; set; }
    public Int64 SenderId { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }

    public bool IsRead { get; set; }
    public bool HasItem { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime ObtainedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
}