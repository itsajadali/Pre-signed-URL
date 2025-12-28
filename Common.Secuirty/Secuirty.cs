using System.Security.Cryptography;
using System.Text;

namespace Common.Secuirty;

public static class Secuirty
{
    public static string Sign(Dictionary<string, string> metadata, long expiresAt, string secertKey)
    {

        var internalMetadata = new Dictionary<string, string>(metadata);
        internalMetadata["ExpiresAt"] = expiresAt.ToString();

        var payload = CreateCanonicalPayload(internalMetadata);
        return GenerateHmacSignature(payload, secertKey);
    }

    public static bool Verify(Dictionary<string, string> metadata,
                              string providedSignature,
                              long expiresAt,
                              string secertKey)
    {


        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresAt)
            return false;

        var payload = CreateCanonicalPayload(metadata);
        var expectedSignature = GenerateHmacSignature(payload, secertKey);

        return expectedSignature == providedSignature;
    }

    private static string CreateCanonicalPayload(Dictionary<string, string> metadata)
    {
        return string.Join("|", metadata.OrderBy(k => k.Key)
                                        .Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
    private static string GenerateHmacSignature(string payload, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA256(keyBytes);
        return Convert.ToBase64String(hmac.ComputeHash(payloadBytes));
    }
}
