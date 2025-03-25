using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LocationHistory
{
    class location
    {
        public record MotiveVehicleLocationHistory(
    [property: JsonPropertyName("vehicle_locations")] List<MotiveVehicleLocation> Locations);
        public record MotiveVehicleLocation(
            [property: JsonPropertyName("vehicle_location")] MotiveVehicleLocationDetail Location);
        public record MotiveVehicleLocationDetail(
            [property: JsonPropertyName("located_at")] DateTime LocatedAt,
            double Lat,
            double Lon,
            Guid Id,
            string Type,
            string Description,
            double? Speed,
            double Bearing,
            [property: JsonPropertyName("battery_voltage")] double? BatteryVoltage,
            double? Odometer,
            [property: JsonPropertyName("true_odometer")] double? TrueOdometer,
            double? EngineHours,
            [property: JsonPropertyName("true_engine_hours")] double? TrueEngineHours,
            double? Fuel,
            [property: JsonPropertyName("fuel_primary_remaining_percentage")] double? FuelPrimaryRemainingPercentage,
            [property: JsonPropertyName("fuel_secondary_remaining_percentage")] double? FuelSecondaryRemainingPercentage,
            MotiveVehicleLocationDriver? Driver,
            [property: JsonPropertyName("veh_range")] double? VehicleRange,
            [property: JsonPropertyName("hvb_state_of_charge")] double? HighVoltageBatteryStateOfCharge,
            [property: JsonPropertyName("hvb_charge_status")] string? HighVoltageBatteryChargeStatus,
            [property: JsonPropertyName("hvb_charge_source")] string? HighVoltageBatteryChargeSource,
            [property: JsonPropertyName("hvb_lifetime_energy_output")] double? HighVoltageBatteryLifetimeEnergyOutput,
            MotiveVehicleLocationEldDevice EldDevice);
        public record MotiveVehicleLocationDriver(
            int Id,
            string FirstName,
            string LastName,
            string Username,
            string? Email,
            string CompanyID,
            string Status,
            string Role
            );
        public record MotiveVehicleLocationEldDevice(int Id, string Identifier, string Model);
    }
}

//OLD CODE

//Improved vehicle IDs

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using NLog;

//// Create a logger instance
//Logger logger = LogManager.GetCurrentClassLogger();

//string ApiKey = null;

//// Configure the application to read from appsettings.json
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();

//// Read the API key from appsettings.json
//ApiKey = configuration["ApiSettings:ApiKey"];
//logger.Info("API Key retrieved from configuration.");

//using HttpClient client = new HttpClient();
//client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

//// Setup cancellation token
//CancellationTokenSource cts = new CancellationTokenSource();
//CancellationToken token = cts.Token;

//try
//{
//// Get all vehicle IDs with pagination
//var vehicleIdMap = await GetAllVehicleIdsAsync(client, token);

//// Convert to DataTable
//DataTable dt = ConvertToDataTable(vehicleIdMap);

//// Print the DataTable
//PrintDataTable(dt);
//}
//catch (OperationCanceledException)
//{
//logger.Info("Operation was cancelled.");
//}
//catch (Exception ex)
//{
//logger.Error(ex, "An error occurred while processing vehicle data.");
//}

//async Task<List<Dictionary<string, object>>> GetAllVehicleIdsAsync(HttpClient client, CancellationToken token, int perPage = 100)
//{
//List<Dictionary<string, object>> vehicleIdMap = new List<Dictionary<string, object>>();
//int currentPage = 1;
//bool morePages = true;

//while (morePages && !token.IsCancellationRequested)
//{
//try
//{
//var response = await client.GetAsync($"https://api.gomotive.com/v1/vehicles?per_page={perPage}&page_no={currentPage}", token);

//// Log status code
//logger.Info($"Page {currentPage}: HTTP {response.StatusCode}");

//if (!response.IsSuccessStatusCode)
//{
//logger.Warn($"Error: {response.StatusCode} - Breaking pagination.");
//break;
//}

//var content = await response.Content.ReadAsStringAsync();
//var vehicleData = JsonConvert.DeserializeObject<VehicleResponse>(content);

//if (vehicleData?.vehicles == null || vehicleData.vehicles.Count == 0)
//{
//morePages = false;
//logger.Info($"Page {currentPage}: No more vehicles found");
//break;
//}

//foreach (var vehicle in vehicleData.vehicles)
//{
//vehicleIdMap.Add(new Dictionary<string, object>
//                {
//                    { "Id", vehicle.vehicle.id },
//                    { "Number", vehicle.vehicle.number }
//                });
//}

//logger.Info($"Page {currentPage}: Added {vehicleData.vehicles.Count} vehicles");

//// Increment the page number for the next iteration
//Console.WriteLine($"Page {currentPage}: Added {vehicleData.vehicles.Count} vehicles");
//currentPage++;

//// Rate limit handling: adaptive delay based on API rate limit headers
//if (response.Headers.Contains("X-RateLimit-Remaining"))
//{
//int remainingRequests = int.Parse(response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault() ?? "1");
//int delay = remainingRequests > 0 ? 500 : 1000; // Adjust delay if remaining requests are low
//logger.Info($"Remaining API calls: {remainingRequests}, Delaying for {delay}ms");
//await Task.Delay(delay, token); // Adjust delay dynamically
//}
//else
//{
//await Task.Delay(500, token); // Default delay
//}

//}
//catch (HttpRequestException httpEx)
//{
//logger.Error(httpEx, $"Network error processing page {currentPage}. Breaking pagination.");
//break;
//}
//catch (JsonException jsonEx)
//{
//logger.Error(jsonEx, $"JSON parsing error processing page {currentPage}. Breaking pagination.");
//break;
//}
//catch (Exception ex)
//{
//logger.Error(ex, $"Error processing page {currentPage}. Breaking pagination.");
//break;
//}
//}

//return vehicleIdMap;
//}

//DataTable ConvertToDataTable(List<Dictionary<string, object>> vehicleIdMap)
//{
//DataTable dt = new DataTable();
//dt.Columns.Add("Id", typeof(string));
//dt.Columns.Add("Number", typeof(string));

//foreach (var item in vehicleIdMap)
//{
//dt.Rows.Add(item["Id"], item["Number"]);
//}

//return dt;
//}

//void PrintDataTable(DataTable dt)
//{
//    Console.WriteLine("\nVehicle ID Map:");
//    foreach (DataRow row in dt.Rows)
//    {
//        Console.WriteLine($"Id: {row["Id"]}, Number: {row["Number"]}");
//    }
//    Console.WriteLine($"Total vehicles: {dt.Rows.Count}");
//}

//public class VehicleResponse
//{
//    public List<Vehicle> vehicles { get; set; }
//}

//public class Vehicle
//{
//    public VehicleDetails vehicle { get; set; }
//}

//public class VehicleDetails
//{
//    public string id { get; set; }
//    public string number { get; set; }
//}


//Vehicle Locations Code

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using RestSharp;
//using Newtonsoft.Json;
//using System.IO;

//string ApiKey = null;

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();

//ApiKey = configuration["ApiSettings:ApiKey"];
//Console.WriteLine("API Key retrieved from configuration.");

//List<Dictionary<string, object>> vehicleIdMap = new List<Dictionary<string, object>>
//        {
//            new Dictionary<string, object> { { "Id", "49366" }, { "Number", "305" } }, // Add more vehicles here
//        };

//string startDate = "2025-01-01";
//string endDate = "2025-01-01";

////DataTable to store vehicle location history
//DataTable vehicleLocationTable = CreateVehicleLocationDataTable();

//foreach (var vehicle in vehicleIdMap)
//{
//    string vehicleId = vehicle["Id"].ToString();
//    string unitNumber = vehicle["Number"].ToString();
//    Console.WriteLine($"Fetching location history for Vehicle ID: {vehicleId}");

//    var locationHistory = await GetVehicleLocationHistoryAsync(vehicleId, startDate, endDate, ApiKey);
//    if (locationHistory != null)
//    {
//        foreach (var location in locationHistory)
//        {
//            vehicleLocationTable.Rows.Add(unitNumber, location.LocatedAt, location.Lat, location.Lon, location.Bearing, location.Speed, location.Description, location.Type);
//        }
//    }
//}

//// Print the DataTable
//PrintDataTable(vehicleLocationTable);

//async Task<List<VehicleLocation>> GetVehicleLocationHistoryAsync(string vehicleId, string startDate, string endDate, string apiKey)
//{
//    //URL for the specific vehicle location API
//    var url = $"https://api.gomotive.com/v3/vehicle_locations/{vehicleId}?start_date={startDate}&end_date={endDate}";

//    //RestClient
//    var client = new RestClient(url);
//    var request = new RestRequest();

//    //headers
//    request.AddHeader("accept", "application/json");
//    request.AddHeader("x-api-key", apiKey);

//    //request
//    var response = await client.GetAsync(request);

//    // Check response
//    if (response.IsSuccessful)
//    {
//        // Deserialize the response content to a structured object
//        var vehicleData = JsonConvert.DeserializeObject<ApiResponse>(response.Content);

//        // Map the response to a list of VehicleLocation objects with only the necessary fields
//        List<VehicleLocation> locations = new List<VehicleLocation>();

//        foreach (var vehicleLocation in vehicleData.VehicleLocations)
//        {
//            var location = new VehicleLocation
//            {
//                LocatedAt = vehicleLocation.VehicleLocation.LocatedAt,
//                Lat = vehicleLocation.VehicleLocation.Lat,
//                Lon = vehicleLocation.VehicleLocation.Lon,
//                Bearing = vehicleLocation.VehicleLocation.Bearing,
//                Speed = vehicleLocation.VehicleLocation.Speed,
//                Description = vehicleLocation.VehicleLocation.Description,
//                Type = vehicleLocation.VehicleLocation.Type
//            };
//            locations.Add(location);
//        }

//        return locations;
//    }
//    else
//    {
//        //log error code
//        Console.WriteLine($"Failed to fetch data for Vehicle ID: {vehicleId} with status: {response.StatusCode}");
//        return null;
//    }
//}

//// Create a DataTable with the desired columns for vehicle location data
//DataTable CreateVehicleLocationDataTable()
//{
//    DataTable dt = new DataTable();

//    // columns that match the fields
//    dt.Columns.Add("unitNumber", typeof(string));
//    dt.Columns.Add("LocatedAt", typeof(DateTime));
//    dt.Columns.Add("Latitude", typeof(double));
//    dt.Columns.Add("Longitude", typeof(double));
//    dt.Columns.Add("Bearing", typeof(double));
//    dt.Columns.Add("SpeedMph", typeof(double));
//    dt.Columns.Add("Location", typeof(string));
//    dt.Columns.Add("Type", typeof(string));

//    return dt;
//}

// Print DataTable
//void PrintDataTable(DataTable dt)
//{
//    Console.WriteLine("Vehicle Location Data:");
//    foreach (DataRow row in dt.Rows)
//    {
//        Console.WriteLine($"unitNumber: {row["unitNumber"]}, Located At: {row["LocatedAt"]}, Latitude: {row["Latitude"]}, Longitude: {row["Longitude"]}, Bearing: {row["Bearing"]}, Speed: {row["SpeedMph"]}, Location: {row["Location"]}, Type: {row["Type"]}");
//    }

//    Console.WriteLine($"Total Locations: {dt.Rows.Count}");
//}