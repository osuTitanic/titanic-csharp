using Titanic.API.Models;

namespace Titanic.Updater;

public static class ModdedReleaseEntryModelExtensions
{
    extension(ModdedReleaseEntryModel entry) {
        public bool IsExtractable =>
            !string.IsNullOrEmpty(entry.DownloadUrl) &&
            entry.DownloadUrl.EndsWith(".zip") &&
            Uri.TryCreate(entry.DownloadUrl, UriKind.Absolute, out _);
    }
}