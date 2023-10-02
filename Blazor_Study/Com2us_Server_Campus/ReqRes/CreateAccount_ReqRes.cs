using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.ReqRes;

public class CreateAccountRequest
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
}

public class CreateAccountResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public Int64 AccountId { get; set; }
}