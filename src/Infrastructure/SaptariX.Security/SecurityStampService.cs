using System.Security.Cryptography;

namespace SaptariX.Security;

public sealed class SecurityStampService : ISecurityStampService
{
    public string CreateStamp()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }
}
