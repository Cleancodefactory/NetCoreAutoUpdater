using System.IO;
using System.IO.Compression;

namespace Ccf.Ck.Libs.AutoUpdater.Models
{
    internal static class ZipHelper
    {
        internal static void Unzip(Stream zippedStream, string targetDir)
        {
            DirectoryInfo targetDirInfo = new DirectoryInfo(targetDir);
            if (targetDirInfo.Exists)
            {
                DeleteRecursive(targetDirInfo);
            }
            using (ZipArchive archive = new ZipArchive(zippedStream))
            {
                archive.ExtractToDirectory(targetDir);
            }
        }

        private static void DeleteRecursive(DirectoryInfo targetDirInfo)
        {
            DirectoryInfo[] subDirs = targetDirInfo.GetDirectories();
            FileInfo[] files = targetDirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
            for (int i = 0; i < subDirs.Length; i++)
            {
                DeleteRecursive(subDirs[i]);
                subDirs[i].Delete();
            }
        }
    }
}
