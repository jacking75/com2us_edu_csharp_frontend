namespace WebAPIServer.DataClass;

public class Account
{
    public Int64 AccountId { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public string SaltValue { get; set; }
}

public class UserAuth
{
    public Int64 AccountId { get; set; } = 0;
    public string AuthToken { get; set; } = "";
    public DateTime LastLogin { get; set; }
}