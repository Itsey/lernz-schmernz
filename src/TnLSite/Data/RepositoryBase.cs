using Plisky.Diagnostics;
using TnLSite.Models;

namespace TnLSite.Data;

public class RepositoryBase {
    protected Bilge b;
    public static string DataDirectory { get; private set; } = "data";
    public static bool IsInitialised { get; set; } = false;

    public RepositoryBase(DynamicTrace dt) {
        b = dt.CreateBilge("tnl-repository");
        b.Info.Flow($"IsInitalised={IsInitialised}");
    }


    public static void Initialize(string contentRootPath) {
        DataDirectory = Path.Combine(contentRootPath, "data");
        Directory.CreateDirectory(DataDirectory);
    }

    public static string GetUserFilePath(string userId) {
        return Path.Combine(DataDirectory, $"{userId}.txt");
    }


    public virtual UserRecord? GetUser(string userId) {
        return null;
    }

    public virtual void SaveUser(UserRecord user) {
    }

    public virtual bool UserExists(string userId) {
        return false;
    }
}