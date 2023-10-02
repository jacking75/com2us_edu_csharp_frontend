namespace WebAPIServer.DataClass;

public class UserData
{
    public Int64 AccountId { get; set; }
    public Int64 UserId { get; set; }
    public Int64 Level { get; set; }
    public Int64 Exp { get; set; }
    public Int64 Money { get; set; }
    public DateTime LastLogin { get; set; }
}

public class UserAttendance
{
    public Int64 AttendanceCount { get; set; }
    public DateTime LastAttendance { get; set; }
}

public class UserItem
{
    public Int64 ItemId { get; set; }
    public Int64 UserId { get; set; }
    public Int64 ItemCode { get; set; }
    public Int64 ItemCount { get; set; }
    public Int64 Attack { get; set; }
    public Int64 Defence { get; set; }
    public Int64 Magic { get; set; }
    public Int64 EnhanceCount { get; set; }
    public bool IsDestroyed { get; set; }
    public DateTime ObtainedAt { get; set; }
}

public class ItemInfo
{
    public Int64 ItemId { get; set; }
    public Int64 ItemCode { get; set; }
    public Int64 ItemCount { get; set; }
}

public class MailItem : ItemInfo
{
    public bool IsReceived { get; set; }
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

public class ClearData
{
    public Int64 StageCode { get; set; }
    public TimeSpan ClearTime { get; set; }
    public Int64 ClearRank { get; set; }
}