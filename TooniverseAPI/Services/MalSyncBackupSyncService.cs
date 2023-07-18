using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using Path = System.IO.Path;

namespace TooniverseAPI.Services;

public static class MalSyncBackupSyncService
{
    public static void Start()
    {
        var fileUrl = "https://github.com/MALSync/MAL-Sync-Backup/archive/refs/heads/master.zip";
        var downloadPath = "master.zip";
        var unzipPath = "unzippedFolder";
        var folderPath = @"unzippedFolder\MALSyncData";

        using (var client = new WebClient())
        {
            client.DownloadFile(fileUrl, downloadPath);
            ZipFile.ExtractToDirectory(downloadPath, unzipPath);
        }

        DeleteFilesExceptFolder(folderPath);

        Console.WriteLine("Operation completed.");
        Console.ReadLine();
    }

    private static void DeleteFilesExceptFolder(string folderPath)
    {
        var directory = new DirectoryInfo(folderPath);

        foreach (var file in directory.GetFiles()) file.Delete();

        foreach (var subDirectory in directory.GetDirectories())
        {
            DeleteFilesExceptFolder(subDirectory.FullName);
            subDirectory.Delete();
        }
    }
}