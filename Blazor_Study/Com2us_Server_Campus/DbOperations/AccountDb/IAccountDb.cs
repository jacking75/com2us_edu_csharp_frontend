using WebAPIServer.DataClass;

namespace WebAPIServer.DbOperations;

public interface IAccountDb : IDisposable
{
    public Task<Tuple<ErrorCode, Int64>> CreateAccountAsync(string email, string password);
    public Task DeleteAccountAsync(string email);

    public Task<Tuple<ErrorCode, Int64>> VerifyAccountAsync(string email, string password);
}
