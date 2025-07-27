using System.Security.Cryptography;

namespace ChunxApp.Helpers;

/// <summary>
/// Provides SHA256 checksum calculation for file integrity verification.
/// </summary>
public static class ChecksumHelper
{
    public static string CalculateChecksum(byte[] data)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
