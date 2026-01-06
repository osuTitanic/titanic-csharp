using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Titanic.API;
using Titanic.API.Models;
using Titanic.API.Requests;
using Titanic.Updater.Versioning;

namespace Titanic.Updater;

public class UpdateManager : IDisposable
{
    private readonly TitanicAPI _api = new();
    private readonly UpdateManagerSettings _settings;

    public UpdateManager(UpdateManagerSettings settings)
    {
        this._settings = settings;

        if (settings.SharpZipLibCodePage != null)
            ZipConstants.DefaultCodePage = settings.SharpZipLibCodePage.Value;

        // Clean up old the executable we made if we just updated
        if (this._settings.ReplaceCurrentExecutable)
        {
            string processPath = ExecutablePath + ".old";
            if(File.Exists(processPath))
                File.Delete(processPath);
        }
    }

    public ModdedReleaseEntryModel? CheckUpdateForClient(ModdedClientInformation clientInfo)
    {
        GetModdedReleaseEntriesRequest request = new(clientInfo.ClientIdentifier);
        
        IEnumerable<ModdedReleaseEntryModel> entries = request.BlockingPerform(this._api)
            .OrderByDescending(e => e.Id);
        
        if (clientInfo.InstalledStream != null)
            entries = entries.Where(e => e.Stream == clientInfo.InstalledStream);
        
        if (clientInfo.InstalledVersion == null)
            return entries.FirstOrDefault();

        OsuVersion currentVersion = OsuVersion.Parse(clientInfo.InstalledVersion, clientInfo.VersionKind);
        foreach (ModdedReleaseEntryModel entry in entries)
        {
            OsuVersion version = OsuVersion.Parse(entry.Version, clientInfo.VersionKind);
            if (currentVersion.IsOlderThan(version))
                return entry;
        }

        return null;
    }

    public DownloadedUpdate DownloadClientUpdate(ModdedClientInformation info, ModdedReleaseEntryModel entry)
    {
        if (!entry.IsExtractable)
            throw new Exception("Update is not extractable (not a .zip), check entry.IsExtractable and prompt the user to open the entry.DownloadUrl instead or repackage your updates");

        string updatePath = Path.Combine(_settings.DataDirectory, $"{info.ClientIdentifier}{Path.DirectorySeparatorChar}");
        if (!Directory.Exists(updatePath))
            Directory.CreateDirectory(updatePath);

        string filename = Path.GetFileName(entry.DownloadUrl);
        string path = Path.Combine(updatePath, filename);

        DownloadedUpdate update = new()
        {
            Filename = filename,
            Path = path,
            Client = info,
        };
        
        if (File.Exists(path))
            return update;
        
        byte[] data = this._api.Download(entry.DownloadUrl);
        File.WriteAllBytes(path, data);

        return update;
    }

    public void InstallClientUpdate(DownloadedUpdate update)
    {
        string staging = Path.Combine(_settings.DataDirectory, "_staging");
        if (!Directory.Exists(staging))
            Directory.CreateDirectory(staging);

        ZipUtil.Extract(update.Path, staging);
        
        if (this._settings.ReplaceCurrentExecutable)
        {
            string processPath = ExecutablePath;
            File.Move(processPath, processPath + ".old");
        }

        string outputDir = _settings.OutputPath;
        if (string.IsNullOrEmpty(outputDir))
            outputDir = Environment.CurrentDirectory;

        if (this._settings.IncludeClientIdentifierInOutputPath)
            outputDir = Path.Combine(outputDir, update.Client.ClientIdentifier + '/');

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string[] files = Directory.GetFiles(staging, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string dest = Path.Combine(outputDir, file.Replace(staging + '/', ""));
            if(File.Exists(dest))
                File.Delete(dest);
            
            File.Move(file, dest);
        }
        
#if NET10_0_OR_GREATER
        string osuExecutable = Path.Combine(outputDir, "osu!");
        if (OperatingSystem.IsLinux() && File.Exists(osuExecutable))
        {
            UnixFileMode fileMode = File.GetUnixFileMode(osuExecutable);
            fileMode |= UnixFileMode.GroupExecute | UnixFileMode.OtherExecute | UnixFileMode.UserExecute;
            File.SetUnixFileMode(osuExecutable, fileMode);
        } 
#endif

        this._settings.Exit?.Invoke();
    }

    private static string ExecutablePath =>
#if NET10_0_OR_GREATER
        Environment.ProcessPath;
#else
        Assembly.GetEntryAssembly().Location;
#endif

    public void Dispose()
    {
        _api.Dispose();
        GC.SuppressFinalize(this);
    }
}