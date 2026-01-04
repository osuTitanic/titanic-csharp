using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Titanic.API.Models
{
    public class ForumModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("parent_id")]
        public int? ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("allow_icons")]
        public bool AllowIcons { get; set; }

        [JsonProperty("icons")]
        public List<ForumIconModel> Icons { get; set; }

        [JsonProperty("children")]
        public List<ForumModel> Children { get; set; }
    }

    public class ForumIconModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }

    public class ForumTopicModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("forum_id")]
        public int ForumId { get; set; }

        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [JsonProperty("icon_id")]
        public int? IconId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("views")]
        public int Views { get; set; }

        [JsonProperty("announcement")]
        public bool Announcement { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("status_text")]
        public string StatusText { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("can_change_icon")]
        public bool CanChangeIcon { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("last_post_at")]
        public DateTime LastPostAt { get; set; }

        [JsonProperty("post_count")]
        public int PostCount { get; set; }

        [JsonProperty("creator")]
        public UserModelCompact Creator { get; set; }

        [JsonProperty("icon")]
        public ForumIconModel Icon { get; set; }
    }

    public class ForumPostModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("topic_id")]
        public int TopicId { get; set; }

        [JsonProperty("forum_id")]
        public int ForumId { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("edit_time")]
        public DateTime? EditTime { get; set; }

        [JsonProperty("edit_count")]
        public int EditCount { get; set; }

        [JsonProperty("edit_locked")]
        public bool EditLocked { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("user")]
        public UserModelCompact User { get; set; }
    }

    public class ForumSubscriptionModel
    {
        [JsonProperty("user")]
        public UserModelCompact User { get; set; }

        [JsonProperty("topic")]
        public ForumTopicModel Topic { get; set; }
    }

    public class ForumBookmarkModel
    {
        [JsonProperty("user")]
        public UserModelCompact User { get; set; }

        [JsonProperty("topic")]
        public ForumTopicModel Topic { get; set; }
    }

    public class ForumTopicCreateRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("notify")]
        public bool Notify { get; set; }

        [JsonProperty("icon")]
        public int? Icon { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ForumPostCreateRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("icon")]
        public int? Icon { get; set; }
    }

    public class ForumPostUpdateRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("lock")]
        public bool Lock { get; set; }
    }
}
