using Newtonsoft.Json;
using System.Collections.Generic;

namespace VehicleTrackingApp.Models
{
    public class ApiResponse
    {
        [JsonProperty("vehicle_locations")]
        public List<VehicleLocationWrapper> VehicleLocations { get; set; }
    }
}
