using System.Security.Cryptography;

namespace UserManagement.Infrastructure.Services;

public class PasswordHasherHasherService : IPasswordHasherService
{
    /// <summary>
    /// Size of salt.
    /// </summary>
    private const int SALT_SIZE = 16;

    private const int KEY_SIZE = 32; // 256 bits

    private const int ITERATIONS = 10000;
    private const char SEGMENT_DELIMITER = ':';

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            ITERATIONS,
            HashAlgorithmName.SHA256,
            KEY_SIZE
        );
        return String.Join(
            SEGMENT_DELIMITER,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            ITERATIONS,
            HashAlgorithmName.SHA256
        );
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var segments = passwordHash.Split(SEGMENT_DELIMITER);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = Int32.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            algorithm,
            hash.Length
        );
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}