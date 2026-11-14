namespace Titanic.Updater;

public delegate void UpdateExitHandler();
public delegate void UpdateEventHandler<T>(T evt);

public class UpdateManagerSettings
{
    /// <summary>
    /// Called after installing an update to signal this version of osu! to close.
    /// </summary>
    public UpdateExitHandler? Exit;

    /// <summary>
    /// Called after a patch update action successfully patches a file.
    /// </summary>
    public UpdateEventHandler<FilePatchedEvent>? PatchUpdateFilePatched;

    /// <summary>
    /// Called after a patch update manifest has been fully applied.
    /// </summary>
    public UpdateEventHandler<ManifestAppliedEvent>? PatchUpdateManifestApplied;

    /// <summary>
    /// The data directory to use for staging updates. Default is 'Data/u'.
    /// </summary>
    public string DataDirectory = "Data/u";
    
    /// <summary>
    /// Should we replace the currently running executable? Disable if writing an external update manager.
    /// </summary>
    public bool ReplaceCurrentExecutable = true;

    /// <summary>
    /// The path we should install updates to. Should be left blank if this is an osu! client.
    /// Defaults to the executable's directory for self-updates, or the current working
    /// directory when ReplaceCurrentExecutable is disabled.
    /// </summary>
    public string OutputPath = string.Empty;

    /// <summary>
    /// Should we include the client identifier as a subdirectory in the output path? Set to true if writing an external update manager.
    /// </summary>
    public bool IncludeClientIdentifierInOutputPath = false;

    /// <summary>
    /// Prefer update_url patch packages when the API provides an ordered update path.
    /// </summary>
    public bool PreferPatchUpdates = true;

    /// <summary>
    /// Fall back to the target release's full archive if a patch update cannot be installed.
    /// </summary>
    public bool FallbackToFullArchive = false;

    /// <summary>
    /// Validate MD5 checksums declared by patch update manifests.
    /// </summary>
    public bool ValidatePatchUpdateChecksums = true;

    /// <summary>
    /// The base URL to use for API requests. Should only be changed for testing against staging or local API instances.
    /// </summary>
    public string ApiBaseUrl = "https://api.titanic.sh";

    /// <summary>
    /// The value to set ZipConstants.DefaultCodePage to. Leave as default (0 on CoreCLR, null on framework) if this doesn't break your client.
    /// Set to null to disable setting this variable. Set to 0 to use the system's default code page (fixes exceptions on CoreCLR).
    /// </summary>
    /// <remarks>
    /// If your client is on CoreCLR and using SharpZipLib, this is your sign to switch to System.IO.Compression.
    /// </remarks>
    public int? SharpZipLibCodePage =
#if NET10_0_OR_GREATER
        0;
#else
        null;
#endif
}

public sealed class FilePatchedEvent
{
    public FilePatchedEvent(DownloadedUpdatePart part, UpdateManifest manifest, UpdateAction action, string destination)
    {
        Part = part;
        Manifest = manifest;
        Action = action;
        Destination = destination;
    }

    public DownloadedUpdatePart Part { get; }
    public UpdateManifest Manifest { get; }
    public UpdateAction Action { get; }
    public string Destination { get; }
}

public sealed class ManifestAppliedEvent
{
    public ManifestAppliedEvent(DownloadedUpdatePart part, UpdateManifest manifest)
    {
        Part = part;
        Manifest = manifest;
    }

    public DownloadedUpdatePart Part { get; }
    public UpdateManifest Manifest { get; }
}
