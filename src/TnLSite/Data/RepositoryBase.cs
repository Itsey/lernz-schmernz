namespace TnLSite.Repository;

public class RepositoryBase {
    protected ILogger lg;
    public static string DataDirectory { get; private set; } = "data";
    public static bool IsInitialised { get; set; } = false;

    public RepositoryBase(ILogger lgr) {
        lg = lgr;
        lg.LogInformation("RepositoryBase initialized");
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