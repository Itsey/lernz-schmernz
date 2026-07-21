using TnLSite.Repository;

namespace TnLSite.Models;

public static class UserMapperExtensions {
    public static UserRecord ToRecord(this UserDetails details) {
        if (details == null) {
            throw new ArgumentNullException(nameof(details));
        }

        return new UserRecord {
            UserId = details.UserId,
            UserName = details.UserName,
            Balance = (long)details.Balance,
            Enabled = details.Enabled,
            LastLogin = details.LastLogin,
            Password = string.Empty
        };
    }

    public static UserDetails ToDetails(this UserRecord record) {
        if (record == null) {
            throw new ArgumentNullException(nameof(record));
        }

        return new UserDetails {
            UserId = record.UserId,
            UserName = record.UserName,
            Balance = record.Balance,
            Enabled = record.Enabled,
            LastLogin = record.LastLogin
        };
    }

}
