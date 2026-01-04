using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Titanic.API.Models
{
    public class ChannelModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}
