using System.ComponentModel.DataAnnotations;

namespace WebAPIServer.ReqRes;

public class KillEnemyRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }

    [Range(1, int.MaxValue)]
    public Int64 EnemyCode { get; set; }
}

public class KillEnemyResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}