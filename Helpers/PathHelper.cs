namespace ChunxApp.Helpers;

public static class PathHelper
{
    public static string GetAppDataDirectory()
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(baseDir, "ChunxApp");
        Directory.CreateDirectory(appDir);
        return appDir;
    }

    public static string GetDatabasePath()
    {
        return Path.Combine(GetAppDataDirectory(), "metadata.db");
    }

    public static string GetChunksDirectory()
    {
        var dir = Path.Combine(GetAppDataDirectory(), "files", "chunks");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetRestoredDirectory()
    {
        var dir = Path.Combine(GetAppDataDirectory(), "files", "restored");
        Directory.CreateDirectory(dir);
        return dir;
    }
}
