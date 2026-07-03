using Newtonsoft.Json;

namespace Titanic.API.Models
{
    public class BenchmarkHardwareModel
    {
        [JsonProperty("renderer")]
        public string Renderer { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("fullscreen")]
        public bool Fullscreen { get; set; }

        [JsonProperty("letterboxing")]
        public bool Letterboxing { get; set; }

        [JsonProperty("cpu")]
        public string CPU { get; set; }

        [JsonProperty("cores")]
        public int Cores { get; set; }

        [JsonProperty("threads")]
        public int Threads { get; set; }

        [JsonProperty("gpu")]
        public string GPU { get; set; }

        [JsonProperty("ram")]
        public int RAM { get; set; }

        [JsonProperty("os")]
        public string OS { get; set; }

        [JsonProperty("motherboard_manufacturer")]
        public string MotherboardManufacturer { get; set; }

        [JsonProperty("motherboard")]
        public string Motherboard { get; set; }
    }
}
