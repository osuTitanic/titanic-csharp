using Newtonsoft.Json;

namespace Titanic.CDN.Models;

public class AdminPresignUploadResponseModel
{
    [JsonProperty("key")]
    public string Key { get; set; } = null!;

    [JsonProperty("url")]
    public string Url { get; set; } = null!;

    [JsonProperty("method")]
    public string Method { get; set; } = null!;
}
