namespace Ccf.Ck.Libs.AutoUpdater.Models
{
    /// <summary>
    /// Configures a command which will be executed on different Operating Systems (OS e.g. Windows, Linux)
    /// If your code targets different OSs please return different OsCommand objects (configured for the correct one)
    /// </summary>
    public class OsCommand
    {
        /// <summary>
        /// Init a ProcessStartInfo with FileName
        /// e.g. Linux: FileName = "/bin/bash"
        /// e.g. Windows FileName = "cmd.exe"
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Init a ProcessStartInfo with Arguments
        /// e.g. Linux: Arguments = $"-c \"{escapedArgs}\""
        /// e.g. Windows Arguments = $"/c \"{escapedArgs}\""
        /// </summary>
        public string Arguments { get; set; }
    }
}
