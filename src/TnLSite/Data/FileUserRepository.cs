using System.Globalization;

namespace TnLSite.Repository;

public sealed class FileUserRepository : RepositoryBase {

    public FileUserRepository(ILogger lgr) : base(lgr) {
        lg.LogInformation("Enter FileUserRepository");

    }

    public override UserRecord? GetUser(string userId) {
        if (string.IsNullOrWhiteSpace(userId)) {
            return null;
        }

        string path = GetUserFilePath(userId);
        if (!File.Exists(path)) {
            return null;
        }

        string[] lines = File.ReadAllLines(path);
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string line in lines) {
            int index = line.IndexOf('=');
            if (index <= 0) {
                continue;
            }

            string key = line[..index].Trim();
            string value = line[(index + 1)..].Trim();
            values[key] = value;
        }

        var record = new UserRecord {
            UserId = GetValue(values, "userId", userId),
            UserName = GetValue(values, "userName"),
            Password = GetValue(values, "password"),
            Balance = (long)ParseDecimal(GetValue(values, "balance")),
            Enabled = ParseBool(GetValue(values, "enabled"), true),
            LastLogin = ParseDate(GetValue(values, "lastLogin"))
        };

        return record;
    }

    public override async Task SaveUser(UserRecord user) {
        if (user is null || string.IsNullOrWhiteSpace(user.UserId)) {
            return;
        }

        Directory.CreateDirectory(DataDirectory);
        string path = GetUserFilePath(user.UserId);
        string[] lines = new[] {
            $"userId={user.UserId}",
            $"userName={user.UserName}",
            $"password={user.Password}",
            $"balance={user.Balance.ToString(CultureInfo.InvariantCulture)}",
            $"enabled={user.Enabled.ToString(CultureInfo.InvariantCulture)}",
            $"lastLogin={user.LastLogin.ToString("O", CultureInfo.InvariantCulture)}"
        };

        await File.WriteAllLinesAsync(path, lines);
    }

    public override bool UserExists(string userId) {
        if (string.IsNullOrWhiteSpace(userId)) {
            return false;
        }

        string path = GetUserFilePath(userId);
        return File.Exists(path);
    }

    private static string GetValue(IReadOnlyDictionary<string, string> values, string key, string fallback = "") {
        return values.TryGetValue(key, out string? value) ? value : fallback;
    }

    private static decimal ParseDecimal(string value) {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : 0m;
    }

    private static bool ParseBool(string value, bool fallback) {
        return bool.TryParse(value, out bool result) ? result : fallback;
    }

    private static DateTime ParseDate(string value) {
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)
            ? result
            : DateTime.MinValue;
    }
}
