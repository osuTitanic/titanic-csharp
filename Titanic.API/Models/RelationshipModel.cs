using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Titanic.API.Models
{
    public class RelationshipModel
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("target_id")]
        public int TargetId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class RelationshipResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
