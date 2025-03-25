using Newtonsoft.Json;

public class VehicleLocationWrapper
{
    [JsonProperty("vehicle_location")]
    public VehicleLocation? VehicleLocation { get; set; }
}

