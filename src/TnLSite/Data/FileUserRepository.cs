using System.Globalization;
using Plisky.Diagnostics;
using TnLSite.Models;

namespace TnLSite.Data;

public sealed class FileUserRepository : RepositoryBase {

    public FileUserRepository(DynamicTrace dt) : base(dt) {
        b.Info.Flow();
    }

    public override UserRecord? GetUser(string userId) {
        if (string.IsNullOrWhiteSpace(userId)) {
            return null;
        }

        var path = GetUserFilePath(userId);
        if (!File.Exists(path)) {
            return null;
        }

        var lines = File.ReadAllLines(path);
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in lines) {
            var index = line.IndexOf('=');
            if (index <= 0) {
                continue;
            }

            var key = line[..index].Trim();
            var value = line[(index + 1)..].Trim();
            values[key] = value;
        }

        var record = new UserRecord {
            UserId = GetValue(values, "userId", userId),
            UserName = GetValue(values, "userName"),
            Password = GetValue(values, "password"),
            Balance = ParseDecimal(GetValue(values, "balance")),
            Enabled = ParseBool(GetValue(values, "enabled"), true),
            LastLogin = ParseDate(GetValue(values, "lastLogin"))
        };

        return record;
    }

    public override void SaveUser(UserRecord user) {
        if (user is null || string.IsNullOrWhiteSpace(user.UserId)) {
            return;
        }

        Directory.CreateDirectory(DataDirectory);
        var path = GetUserFilePath(user.UserId);
        var lines = new[] {
            $"userId={user.UserId}",
            $"userName={user.UserName}",
            $"password={user.Password}",
            $"balance={user.Balance.ToString(CultureInfo.InvariantCulture)}",
            $"enabled={user.Enabled.ToString(CultureInfo.InvariantCulture)}",
            $"lastLogin={user.LastLogin.ToString("O", CultureInfo.InvariantCulture)}"
        };

        File.WriteAllLines(path, lines);
    }

    public override bool UserExists(string userId) {
        if (string.IsNullOrWhiteSpace(userId)) {
            return false;
        }

        var path = GetUserFilePath(userId);
        return File.Exists(path);
    }

    private static string GetValue(IReadOnlyDictionary<string, string> values, string key, string fallback = "") {
        return values.TryGetValue(key, out var value) ? value : fallback;
    }

    private static decimal ParseDecimal(string value) {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : 0m;
    }

    private static bool ParseBool(string value, bool fallback) {
        return bool.TryParse(value, out var result) ? result : fallback;
    }

    private static DateTime ParseDate(string value) {
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)
            ? result
            : DateTime.MinValue;
    }
}
