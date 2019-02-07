using Ccf.Ck.Libs.AutoUpdater.Models;

namespace Ccf.Ck.Libs.AutoUpdater
{
    public interface IAutoUpdate
    {
        string Version { get; }
        string SourceUrl { get; }
        string BearerToken { get; }
        OsCommand StopCommand { get; }
        OsCommand StartCommand { get; }
    }
}
