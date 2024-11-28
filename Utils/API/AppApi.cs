namespace ToNSaveManager.Utils.API;

[System.Runtime.InteropServices.ComVisible(true)]
public class AppApi
{
    public string GetAppVersion()
    {
        var currentVersion = Program.GetVersion();
        return currentVersion == null ? "unknown" : currentVersion.ToString();
    }
}