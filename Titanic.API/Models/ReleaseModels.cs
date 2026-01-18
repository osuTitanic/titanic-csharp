using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Titanic.API.Models
{
    public class TitanicReleaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("known_bugs")]
        public string KnownBugs { get; set; }

        [JsonProperty("supported")]
        public bool Supported { get; set; }

        [JsonProperty("preview")]
        public bool Preview { get; set; }

        [JsonProperty("downloads")]
        public List<string> Downloads { get; set; }

        [JsonProperty("screenshots")]
        public List<string> Screenshots { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class OsuReleaseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("subversion")]
        public int Subversion { get; set; }

        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("files")]
        public List<OsuReleaseFile> Files { get; set; }
    }

    public class OsuReleaseFile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("file_version")]
        public string FileVersion { get; set; }

        [JsonProperty("file_hash")]
        public string FileHash { get; set; }

        [JsonProperty("filesize")]
        public long Filesize { get; set; }

        [JsonProperty("patch_id")]
        public int? PatchId { get; set; }

        [JsonProperty("url_full")]
        public string UrlFull { get; set; }

        [JsonProperty("url_patch")]
        public string UrlPatch { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class OsuChangelogModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("build")]
        public string Build { get; set; }
    }

    public class ModdedReleaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("creator_id")]
        public int? CreatorId { get; set; }
        
        [JsonProperty("topic_id")]
        public int? TopicId { get; set; }
        
        [JsonProperty("client_version")]
        public int ClientVersion { get; set; }
        
        [JsonProperty("client_extension")]
        public string ClientExtension { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Obsolete("Use ClientExtension instead.")]
        public string Identifier => ClientExtension;
        
        [Obsolete("Use CreatorId instead.", true)]
        public UserModel Creator { get; set; }
    }

    public class ModdedReleaseEntryModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("mod_name")]
        public string ModName { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }
        
        [JsonProperty("update_url")]
        public string UpdateUrl { get; set; }
        
        [JsonProperty("post_id")]
        public int? PostId { get; set; }

        [JsonProperty("changelog")]
        public string Changelog { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
