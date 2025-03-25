using Newtonsoft.Json;

namespace VehicleTrackingApp.Models
{
    public class VehicleLocationWrapper
    {
        [JsonProperty("vehicle_location")]
        public VehicleLocation VehicleLocation { get; set; }
    }
}
