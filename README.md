# NetCoreAutoUpdater

## Description
.Net Core implementation of a self updating component which even updates the updater itself.

## Installation
You can get the source code from the current releases or install 
__NetCoreAutoUpdater__  as a NuGet package:
```
PM> Install-Package Ccf.Ck.Libs.AutoUpdater
PM> Install-Package Ccf.Ck.Libs.AutoUpdater.Process
```
or
```
dotnet add package Ccf.Ck.Libs.AutoUpdater
dotnet add package Ccf.Ck.Libs.AutoUpdater.Process
```

## How to use it?
### Prerequisites
You will need an URL to a remote folder containing:
1. .zip archive with the updated version.
2. .json file - 
```json
{
  	"TargetVersion": "<Version>",
	"UrlZipFile": "<Name>.zip"
}
```

### Usage
1. Implement IAutoUpdate interface:
```csharp
    public interface IAutoUpdate
    {
        string Version { get; }
        string SourceUrl { get; }
        string BearerToken { get; }
        OsCommand StopCommand { get; }
        OsCommand StartCommand { get; }
    }
``` 
    where: 
* `Version` is the current version of your app;
* `SourceUrl` is the URL where zip archive will be obtained from (check prerequisites);
* `BearerToken` for authentication purposes (or `string.Empty`);
* `OsCommand`:
```csharp
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

    // Example (Windows)
    OsCommand osCommand = new OsCommand();
    osCommand.FileName = "cmd";
    osCommand.Arguments = $"/c sc stop {_ConfigurationRoot["ServiceName"]}";
```
2. Final step - call `CheckUpdateAvailableAsync()` and `UpdateAsync(string targetDir)`. 

    Example usage: 
    ```csharp
    AutoUpdateImp autoUpdateImp = new AutoUpdateImp(); // Your implementation of IAutoUpdate
    AutoUpdate autoUpdate = new AutoUpdate(autoUpdateImp);
    Task<bool> isUpdateAvailable = autoUpdate.CheckUpdateAvailableAsync();

    if (isUpdateAvailable.Result)
    {
       string dir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
       Task task = Task.Run(async () => { await autoUpdate.UpdateAsync(dir); });
    }
    else
    {
        content = "No update required";
    }
    ```

## License
Open sourced under the MIT license.
