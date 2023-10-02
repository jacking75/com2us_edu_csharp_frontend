using System.Security.Cryptography;
using System.Text;

namespace WebAPIServer.Util;

public class Security
{
    private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string RandomString(Int64 num)
    {
        var bytes = new Byte[num];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }

        return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
    }

    public static string MakeHashedPassword(string saltValue, string password)
    {
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + password));
            var stringBuilder = new StringBuilder();

            foreach (var b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
        }
    }
}