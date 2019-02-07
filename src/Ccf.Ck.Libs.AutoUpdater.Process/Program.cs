using System;
using System.IO;
using System.Text;

namespace Ccf.Ck.Libs.AutoUpdater.Process
{
    class Program
    {
        private static DirectoryInfo _TargetDirInfo;
        private const string AUTOUPDATER_PROCESS_EXEC = "Ccf.Ck.Libs.AutoUpdater.Process";
        static void Main(string[] args)
        {
            InParams inParams = new InParams();
            StringBuilder standardOutput = new StringBuilder();
            int exitCode = 0;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    inParams[i] = args[i];
                }
                _TargetDirInfo = inParams.TargetDirInfo;
                //1. Stop
                if (inParams.HasStopCommand())
                {
                    exitCode = Utilities.ExecuteCommand(inParams.StopCommandFileName, inParams.StopCommandParameters, ref standardOutput);
                    if (exitCode != 0) //Error
                    {
                        Log(standardOutput.ToString());
                    }
                }
                
                KillProcess(inParams.ProcessId);
                //2. Update
                UpdateTarget(inParams.SourceDirInfo, inParams.TargetDirInfo, AUTOUPDATER_PROCESS_EXEC);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
                //3. Clean up
                Cleanup(inParams.SourceDirInfo);
                //4. Start
                if (inParams.HasStartCommand())
                {
                    exitCode = Utilities.ExecuteCommand(inParams.StartCommandFileName, inParams.StartCommandParameters, ref standardOutput);
                    if (exitCode != 0) //Error
                    {
                        Log(standardOutput.ToString());
                    }
                }
            }
        }

        private static void Log(string message)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(_TargetDirInfo.FullName, AUTOUPDATER_PROCESS_EXEC + ".log"), true))
                {
                    outputFile.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} {message}");
                }
            }
            catch
            {
            }
        }

        private static void Cleanup(DirectoryInfo sourceDirInfo)
        {
            DeleteRecursive(sourceDirInfo, string.Empty);
            sourceDirInfo.Delete(true);
        }

        private static void UpdateTarget(DirectoryInfo sourceDirInfo, DirectoryInfo targetDirInfo, string excludeFileName)
        {
            DeleteRecursive(targetDirInfo, excludeFileName);
            CopyAll(sourceDirInfo, targetDirInfo);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                try
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                catch
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name + ".backup"), true);
                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private static void DeleteRecursive(DirectoryInfo targetDirInfo, string excludeFileName)
        {
            DirectoryInfo[] subDirs = targetDirInfo.GetDirectories();
            FileInfo[] files = targetDirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.Contains(excludeFileName, StringComparison.OrdinalIgnoreCase))
                {
                    int shouldRetry = 0;
                    Retry:
                    try
                    {
                        File.SetAttributes(files[i].FullName, FileAttributes.Normal);
                        files[i].Delete();
                        shouldRetry = 0;
                    }
                    catch
                    {
                        shouldRetry++;
                    }
                    if (shouldRetry > 0 && shouldRetry < 10)
                    {
                        goto Retry;
                    }
                }
            }
            for (int i = 0; i < subDirs.Length; i++)
            {
                DeleteRecursive(subDirs[i], excludeFileName);
                subDirs[i].Delete(true);
            }
        }

        /// <summary>
        /// Kill a process and all of its children
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcess(int pid)
        {
            System.Diagnostics.Process process = null;
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            try
            {
                process = System.Diagnostics.Process.GetProcessById(pid);
                if (process == null)
                {
                    Log($"Process with id {pid} is null");
                }
                else
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Log($"Process with Id {pid} threw exception: {ex.Message}");
            }
        }
    }
}
