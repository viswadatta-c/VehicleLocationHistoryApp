using Newtonsoft.Json;
//map the API response structure
public class ApiResponseModel
{
    [JsonProperty("vehicle_locations")]
    public List<VehicleLocationWrapper>? VehicleLocations { get; set; }
}

