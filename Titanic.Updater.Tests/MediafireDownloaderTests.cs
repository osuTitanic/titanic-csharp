using Titanic.API.Http;
using Xunit;

namespace Titanic.Updater.Tests;

public class MediafireDownloaderTests
{
    [Fact]
    public void FetchesDownloadLinkAndFilename()
    {
        string mediafireUrl = "https://www.mediafire.com/file/p3oy7lbkbpc8okz/b20130303+Digital+Client+++4.4.31.7z/file";

        IHttpInterface http = HttpInterfaceFactory.Create("https://api.titanic.sh");
        MediaFireDownloader downloader = new MediaFireDownloader(http);

        MediaFireDownloader.DownloadItem? result = downloader.FetchDirectDownloadUrl(mediafireUrl);
        Assert.NotNull(result);
        Assert.NotNull(result.DownloadUrl);
    }
}