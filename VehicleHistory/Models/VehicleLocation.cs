using Newtonsoft.Json;
using System;

namespace VehicleTrackingApp.Models
{
    public class VehicleLocation
    {
        [JsonProperty("located_at")]
        public DateTime LocatedAt { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("bearing")]
        public double Bearing { get; set; }

        [JsonProperty("speed")]
        public double? Speed { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }
}
