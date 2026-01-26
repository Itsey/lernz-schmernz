using System.Security.Cryptography;

namespace TnLSite.Services;

public sealed class InMemoryTokenService : ITokenService {
    private const string TOKEN_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int TOKEN_LENGTH = 8;
    private readonly Dictionary<string, string> tokens = new(StringComparer.OrdinalIgnoreCase);
    private readonly object gate = new();

    public string CreateToken(string userId) {
        var token = GenerateToken();
        lock (gate) {
            tokens[userId] = token;
        }

        return token;
    }

    public bool ValidateToken(string userId, string token) {
        lock (gate) {
            return tokens.TryGetValue(userId, out var existing) && string.Equals(existing, token, StringComparison.Ordinal);
        }
    }

    private static string GenerateToken() {
        Span<byte> data = stackalloc byte[TOKEN_LENGTH];
        RandomNumberGenerator.Fill(data);
        Span<char> chars = stackalloc char[TOKEN_LENGTH];
        for (var i = 0; i < TOKEN_LENGTH; i++) {
            chars[i] = TOKEN_CHARS[data[i] % TOKEN_CHARS.Length];
        }

        return new string(chars);
    }
}
