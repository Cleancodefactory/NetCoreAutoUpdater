namespace Ccf.Ck.Libs.AutoUpdater.Models
{
    /// <summary>
    /// Maps the version file on the server which has the following structure
    /// {
    ///   "TargetVersion": "X.Y.Z",
    ///    "UrlZipFile": "Name.zip"
    /// }
    /// </summary>
    internal class VersionResponse
    {
        public string TargetVersion { get; set; }
        public string UrlZipFile { get; set; }
    }
}
