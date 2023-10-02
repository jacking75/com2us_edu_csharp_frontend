namespace ManagingTool.Shared.DTO;

public class ManagingLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ManagingLoginResponse
{
    public ErrorCode Result { get; set; }
    public Int64 AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string accessToken { get; set; } = string.Empty;
    public string refreshToken { get; set; } = string.Empty;
}