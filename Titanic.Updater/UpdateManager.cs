using System.Reflection;
using System.Text;
#if NET8_0_OR_GREATER
using ICSharpCode.SharpZipLib.Zip;
#else
using Titanic.ICSharpCode.SharpZipLib.Zip;
#endif
using Titanic.API;
using Titanic.API.Models;
using Titanic.API.Requests;

namespace Titanic.Updater;

public class UpdateManager : IDisposable
{
    private readonly TitanicAPI _api;
    private readonly UpdateManagerSettings _settings;

    public UpdateManager(UpdateManagerSettings settings)
    {
        this._settings = settings;
        this._api = new TitanicAPI(settings.ApiBaseUrl);

        if (settings.SharpZipLibCodePage != null)
        {
#if !NET8_0_OR_GREATER
            ZipConstants.DefaultCodePage = settings.SharpZipLibCodePage.Value;
#endif
        }

        // Clean up old files we moved aside during a previous update
        if (this._settings.ReplaceCurrentExecutable)
            DeleteOldUpdateFiles(ExecutablePath);
    }

    public UpdateInformation? CheckUpdateForClient(ModdedClientInformation clientInfo)
    {
        GetModdedReleaseUpdateRequest request = new(
            clientInfo.ClientIdentifier,
            version: clientInfo.InstalledVersion,
            stream: clientInfo.InstalledStream
        );
        ModdedReleaseUpdateModel update = request.BlockingPerform(this._api);

        return update.TargetRelease == null
            ? null
            : new UpdateInformation(update, clientInfo.ClientIdentifier);
    }

    public DownloadedUpdate DownloadClientUpdate(UpdateInformation update)
    {
        bool canUsePatchUpdate = update.HasPatchUpdatePath && CanDownloadPatchUpdatePath(update);

        if (this._settings.PreferPatchUpdates && canUsePatchUpdate)
            return DownloadPatchUpdatePath(update);

        return DownloadFullUpdate(update);
    }

    private DownloadedUpdate DownloadFullUpdate(UpdateInformation update)
    {
        if (!update.IsExtractable)
            throw new Exception("Update is not extractable (not a .zip), check update.IsExtractable and prompt the user to open the update.DownloadUrl instead or repackage your updates");

        string updatePath = Path.Combine(_settings.DataDirectory, $"{update.ClientIdentifier}{Path.DirectorySeparatorChar}");
        if (!Directory.Exists(updatePath))
            Directory.CreateDirectory(updatePath);

        string filename = Path.GetFileName(update.DownloadUrl);
        string path = Path.Combine(updatePath, filename);

        DownloadedUpdate downloadedUpdate = new()
        {
            Kind = DownloadedUpdateKind.FullArchive,
            Filename = filename,
            Path = path,
            ClientIdentifier = update.ClientIdentifier,
            FullArchiveUrl = update.DownloadUrl,
            FullArchivePath = path,
        };

        if (File.Exists(path))
            return downloadedUpdate;

        byte[] data = this._api.Download(update.DownloadUrl);
        File.WriteAllBytes(path, data);

        return downloadedUpdate;
    }

    private DownloadedUpdate DownloadPatchUpdatePath(UpdateInformation update)
    {
        string updatePath = Path.Combine(_settings.DataDirectory, "_manifests");

        if (!Directory.Exists(updatePath))
            Directory.CreateDirectory(updatePath);

        DownloadedUpdate downloadedUpdate = new()
        {
            Kind = DownloadedUpdateKind.PatchUpdatePath,
            ClientIdentifier = update.ClientIdentifier,
            FullArchiveUrl = update.DownloadUrl,
        };

        for (int i = 0; i < update.UpdatePath.Count; i++)
        {
            UpdateInformation pathUpdate = update.UpdatePath[i];

            string filename = GetManifestFilename(pathUpdate);
            string path = Path.Combine(updatePath, filename);

            byte[] data = this._api.Download(pathUpdate.DownloadUrl);
            File.WriteAllBytes(path, data);

            UpdateManifest manifest = UpdateManifestReader.ReadFromJson(Encoding.UTF8.GetString(data));
            UpdateManifestValidator.Validate(manifest, pathUpdate.ClientIdentifier);

            if (manifest.ForceUpdate)
                downloadedUpdate.IsForceUpdate = true;

            downloadedUpdate.Parts.Add(new DownloadedUpdatePart
            {
                ClientIdentifier = pathUpdate.ClientIdentifier,
                Filename = filename,
                Path = path,
                Version = pathUpdate.Version,
                ManifestUrl = pathUpdate.DownloadUrl,
                Manifest = manifest,
            });
        }

        if (downloadedUpdate.Parts.Count > 0)
        {
            downloadedUpdate.Filename = downloadedUpdate.Parts[downloadedUpdate.Parts.Count - 1].Filename;
            downloadedUpdate.Path = downloadedUpdate.Parts[downloadedUpdate.Parts.Count - 1].Path;
        }
        return downloadedUpdate;
    }

    private static bool CanDownloadPatchUpdatePath(UpdateInformation update)
    {
        return update.UpdatePath.Count > 0;
    }

    private static string GetManifestFilename(UpdateInformation update)
    {
        string path = update.DownloadUrl;
        if (Uri.TryCreate(update.DownloadUrl, UriKind.RelativeOrAbsolute, out Uri? uri))
            path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.OriginalString;

        string name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(name))
            name = update.Version;

        return SanitizeFilename(name);
    }

    private static string SanitizeFilename(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "unknown";

        char[] invalidChars = Path.GetInvalidFileNameChars();
        char[] chars = value.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (Array.IndexOf(invalidChars, chars[i]) >= 0 || chars[i] == '/' || chars[i] == '\\')
                chars[i] = '_';
        }

        return new string(chars);
    }

    public void InstallClientUpdate(DownloadedUpdate update)
    {
        if (update.Kind == DownloadedUpdateKind.PatchUpdatePath)
        {
            InstallPatchUpdate(update);
            return;
        }

        InstallFullArchiveUpdate(update);
    }

    private void InstallFullArchiveUpdate(DownloadedUpdate update)
    {
        string staging = Path.Combine(_settings.DataDirectory, "_staging");
        if (Directory.Exists(staging))
            Directory.Delete(staging, true);

        if (!Directory.Exists(staging))
            Directory.CreateDirectory(staging);

        ZipUtil.Extract(update.Path, staging);

        string outputDir = GetOutputDirectory(update.ClientIdentifier);

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string[] files = Directory.GetFiles(staging, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string relativePath = UpdatePathUtil.GetRelativePath(staging, file);
            string dest = UpdatePathUtil.CombineSafe(outputDir, relativePath);

            PatchUpdateApplier.MoveFileAndReplace(file, dest, this._settings.ReplaceCurrentExecutable ? ExecutablePath : null);
        }

        SetUnixExecutableBit(outputDir);

        this._settings.Exit?.Invoke();
    }

    private void InstallPatchUpdate(DownloadedUpdate update)
    {
        string staging = Path.Combine(_settings.DataDirectory, "_staging");
        if (Directory.Exists(staging))
            Directory.Delete(staging, true);

        Directory.CreateDirectory(staging);

        string outputDir = GetOutputDirectory(update.ClientIdentifier);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        try
        {
            PatchUpdateApplier applier = new(this._settings, outputDir, staging, ExecutablePath, this._api.Download);
            applier.Apply(update.Parts);
            SetUnixExecutableBit(outputDir);
            this._settings.Exit?.Invoke();
        }
        catch (Exception ex)
        {
            if (!this._settings.FallbackToFullArchive)
                throw new PatchUpdateException("Patch update failed and full archive fallback is disabled", ex);

            DownloadedUpdate fullUpdate = DownloadFallbackFullUpdate(update);
            InstallFullArchiveUpdate(fullUpdate);
        }
    }

    private DownloadedUpdate DownloadFallbackFullUpdate(DownloadedUpdate update)
    {
        if (!string.IsNullOrEmpty(update.FullArchivePath) && File.Exists(update.FullArchivePath))
        {
            return new DownloadedUpdate
            {
                Kind = DownloadedUpdateKind.FullArchive,
                ClientIdentifier = update.ClientIdentifier,
                Filename = Path.GetFileName(update.FullArchivePath),
                Path = update.FullArchivePath,
                FullArchivePath = update.FullArchivePath,
            };
        }

        if (string.IsNullOrEmpty(update.FullArchiveUrl))
            throw new PatchUpdateException("Patch update failed and no full archive fallback URL is available");

        UpdateInformation fallback = new(update.FullArchiveUrl, update.ClientIdentifier, string.Empty);
        return DownloadFullUpdate(fallback);
    }

    private string GetOutputDirectory(string clientIdentifier)
    {
        string outputDir = _settings.OutputPath;
        if (string.IsNullOrEmpty(outputDir))
            outputDir = Environment.CurrentDirectory;

        if (_settings.IncludeClientIdentifierInOutputPath)
            outputDir = Path.Combine(outputDir, clientIdentifier + Path.DirectorySeparatorChar);

        return outputDir;
    }

    private static void SetUnixExecutableBit(string outputDir)
    {
#if NET10_0_OR_GREATER
        string osuExecutable = Path.Combine(outputDir, "osu!");
        if ((OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) && File.Exists(osuExecutable))
        {
            UnixFileMode fileMode = File.GetUnixFileMode(osuExecutable);
            fileMode |= UnixFileMode.GroupExecute | UnixFileMode.OtherExecute | UnixFileMode.UserExecute;
            File.SetUnixFileMode(osuExecutable, fileMode);
        }
#endif
    }

    /// <summary>
    /// Resolves the path to the currently running executable.
    /// On .NET 8+ we can use Environment.ProcessPath which is more reliable,
    /// otherwise we fall back to Assembly.GetEntryAssembly().Location which
    /// may not always be accurate, e.g. in single-file publish scenarios
    /// </summary>
    private static string ExecutablePath
    {
        get
        {
#if NET8_0_OR_GREATER
            if (!string.IsNullOrEmpty(Environment.ProcessPath) && IsPathInDirectory(Environment.ProcessPath, AppContext.BaseDirectory))
                return Environment.ProcessPath;
#endif

            return Assembly.GetEntryAssembly()!.Location;
        }
    }

    /// <summary>
    /// Deletes old files we may have created during a previous update.
    /// We move old files to {filename}.old.{guid} & move the new ones in place,
    /// so we want to clean up any old ones that may be left over from previous updates.
    /// </summary>
    private static void DeleteOldUpdateFiles(string executablePath)
    {
        TryDeleteFile(executablePath + ".old");

        string? directory = Path.GetDirectoryName(executablePath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            return;

        DeleteMovedAsideFilesInDirectory(directory);
    }

    private static void DeleteMovedAsideFilesInDirectory(string directory)
    {
        // Search target directory for matching moved-aside files & nuke them
        string[] files;
        try
        {
            files = Directory.GetFiles(directory, "*.old.*", SearchOption.TopDirectoryOnly);
        }
        catch
        {
            return;
        }

        foreach (string path in files)
        {
            if (IsMovedAsideFile(path))
                TryDeleteFile(path);
        }

        // Recursively repeat this process for subdirectories
        string[] directories;
        try
        {
            directories = Directory.GetDirectories(directory);
        }
        catch
        {
            return;
        }

        foreach (string childDirectory in directories)
            DeleteMovedAsideFilesInDirectory(childDirectory);
    }

    private static bool IsMovedAsideFile(string path)
    {
        // Moved-aside filenames are in the format {filename}.old.{guid}
        string filename = Path.GetFileName(path);
        int oldMarkerIndex = filename.LastIndexOf(".old.", StringComparison.OrdinalIgnoreCase);

        if (oldMarkerIndex < 0)
            // ".old" marker not found in filename
            return false;

        string suffix = filename.Substring(oldMarkerIndex + ".old.".Length);

        try
        {
            // Check if guid suffix is also valid just to be sure
            new Guid(suffix);
            return true;
        }
        catch
        {
            // Not a valid guid
            return false;
        }
    }

#if NET8_0_OR_GREATER
    private static bool IsPathInDirectory(string path, string directory)
    {
        try
        {
            string fullPath = Path.GetFullPath(path);
            string fullDirectory = Path.GetFullPath(directory);
            
            if (!fullDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()) && !fullDirectory.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                fullDirectory += Path.DirectorySeparatorChar;

            return fullPath.StartsWith(fullDirectory, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
#endif

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // The previous process may still be shutting down &
            // is holding a lock on the file
        }
    }

    public void Dispose()
    {
        _api.Dispose();
        GC.SuppressFinalize(this);
    }
}
