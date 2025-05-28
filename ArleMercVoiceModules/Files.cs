using System.IO;
using BepInEx;

public static class Files
{
    public static PluginInfo PluginInfo;
    internal static string assemblyDir => Path.GetDirectoryName(PluginInfo.Location);
    internal static void Init(PluginInfo info)
    {
        PluginInfo = info;
    }
    internal static string GetPathToFile(string folderName, string fileName)
    {
        return Path.Combine(assemblyDir, folderName, fileName);
    }
}
