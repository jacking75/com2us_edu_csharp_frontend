using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class LoginRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
    [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public String Email { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public String Password { get; set; }

    public double AppVersion { get; set; }

    public double MasterVersion { get; set; }
}

public class LoginResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public string Authtoken { get; set; }
    public UserData userData { get; set; }
    public List<UserItem> userItem { get; set; }
    public string notificationUrl { get; set; }
    public Int64 LobbyNum { get; set; }
}
