//Combined code
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using NLog;
using System.Linq;
using CsvHelper;
using System.Globalization;

public class VehicleDataProcessor
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private string ApiKey;

    /*public async Task ProcessVehicleData()
    {
        try
        {
            Configure();
            DataTable vehicleIdTable = await GetAllVehicleIdsAsync();

            logger.Info("Vehicle IDs fetched:");
            PrintVehicleIdTable(vehicleIdTable);

            DataTable vehicleLocationTable = CreateVehicleLocationDataTable();

            DataRow targetVehicleRow = null;
            foreach (DataRow row in vehicleIdTable.Rows)
            {
                if (row["Id"].ToString() == "49386" && row["Number"].ToString() == "303")
                {
                    targetVehicleRow = row;
                    break;
                }
            }

            if (targetVehicleRow != null)
            {
                string vehicleId = targetVehicleRow["Id"].ToString();
                string unitNumber = targetVehicleRow["Number"].ToString();
                logger.Info($"Processing location history for Vehicle ID: {vehicleId} and Number: {unitNumber}");

                var locationHistory = await GetVehicleLocationHistoryAsync(vehicleId, "2025-01-01", "2025-03-15");
                if (locationHistory != null)
                {
                    foreach (var location in locationHistory)
                    {
                        vehicleLocationTable.Rows.Add(unitNumber, location.LocatedAt, location.Lat, location.Lon, location.Bearing, location.Speed, location.Description, location.Type);
                    }
                    PrintVehicleLocationTable(vehicleLocationTable);
                }
            }
            else
            {
                logger.Warn("Target vehicle ID 49366 and number 305 not found.");
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred during vehicle data processing.");
        }
    }
    */
 
    public async Task ProcessVehicleData()
    {
        try
        {
            Configure();
            DataTable vehicleIdTable = await GetAllVehicleIdsAsync();

            logger.Info("Vehicle IDs fetched:");
            PrintVehicleIdTable(vehicleIdTable);

            foreach (DataRow row in vehicleIdTable.Rows)
            {
                string vehicleId = row["Id"].ToString();
                string unitNumber = row["Number"].ToString();

                logger.Info($"Processing location history for Vehicle ID: {vehicleId} and Number: {unitNumber}");

                DataTable vehicleLocationTable = CreateVehicleLocationDataTable();
                DateTime startDate = new DateTime(2025, 1, 1);
                DateTime endDate = new DateTime(2025, 3, 15);
                TimeSpan interval = TimeSpan.FromDays(5);

                while (startDate < endDate)
                {
                    DateTime rangeEnd = startDate.Add(interval);
                    if (rangeEnd > endDate)
                    {
                        rangeEnd = endDate;
                    }

                    var locationHistory = await GetVehicleLocationHistoryAsync(vehicleId, startDate.ToString("yyyy-MM-dd"), rangeEnd.ToString("yyyy-MM-dd"));

                    if (locationHistory != null)
                    {
                        foreach (var location in locationHistory)
                        {
                            vehicleLocationTable.Rows.Add(unitNumber, location.LocatedAt, location.Lat, location.Lon, location.Bearing, location.Speed, location.Description, location.Type);
                        }
                    }
                    else
                    {
                        logger.Warn($"Failed to retrieve location history for Vehicle ID: {vehicleId} and Number: {unitNumber} for date range: {startDate.ToString("yyyy-MM-dd")} to {rangeEnd.ToString("yyyy-MM-dd")}.");
                    }

                    startDate = rangeEnd;
                }
                PrintVehicleLocationTable(vehicleLocationTable);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred during vehicle data processing.");
        }
    }

    private void Configure()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        ApiKey = configuration["ApiSettings:ApiKey"];
        logger.Info("API Key retrieved from configuration.");
    }

    private async Task<DataTable> GetAllVehicleIdsAsync(int perPage = 100)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Id", typeof(string));
        dt.Columns.Add("Number", typeof(string));

        int currentPage = 1;
        bool morePages = true;
        using var client = new RestClient("https://api.gomotive.com/v1/vehicles");

        while (morePages)
        {
            var request = new RestRequest()
                .AddParameter("per_page", perPage)
                .AddParameter("page_no", currentPage)
                .AddHeader("x-api-key", ApiKey);

            try
            {
                var response = await client.GetAsync(request);

                if (!response.IsSuccessful)
                {
                    logger.Warn($"Error: {response.StatusCode} - Breaking pagination.");
                    break;
                }

                var vehicleData = JsonConvert.DeserializeObject<VehicleResponseModel>(response.Content);

                if (vehicleData?.vehicles == null || vehicleData.vehicles.Count == 0)
                {
                    morePages = false;
                    logger.Info($"Page {currentPage}: No more vehicles found");
                    break;
                }

                foreach (var vehicle in vehicleData.vehicles)
                {
                    dt.Rows.Add(vehicle.vehicle.id, vehicle.vehicle.number);
                }

                logger.Info($"Page {currentPage}: Added {vehicleData.vehicles.Count} vehicles");
                Console.WriteLine($"Page {currentPage}: Added {vehicleData.vehicles.Count} vehicles");
                currentPage++;
                await Task.Delay(500); //Basic rate limiting.
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error processing page {currentPage}. Breaking pagination.");
                break;
            }
        }

        return dt;
    }

    private async Task<List<VehicleLocation>> GetVehicleLocationHistoryAsync(string vehicleId, string startDate, string endDate)
    {
        var url = $"https://api.gomotive.com/v3/vehicle_locations/{vehicleId}?start_date={startDate}&end_date={endDate}";
        var client = new RestClient(url);
        var request = new RestRequest();

        request.AddHeader("accept", "application/json");
        request.AddHeader("x-api-key", ApiKey);

        try
        {
            var response = await client.GetAsync(request);

            if (response.IsSuccessful)
            {
                var vehicleData = JsonConvert.DeserializeObject<ApiResponseModel>(response.Content);
                List<VehicleLocation> locations = vehicleData?.VehicleLocations?.Select(vl => new VehicleLocation
                {
                    LocatedAt = vl.VehicleLocation.LocatedAt,
                    Lat = vl.VehicleLocation.Lat,
                    Lon = vl.VehicleLocation.Lon,
                    Bearing = vl.VehicleLocation.Bearing,
                    Speed = vl.VehicleLocation.Speed,
                    Description = vl.VehicleLocation.Description,
                    Type = vl.VehicleLocation.Type
                }).ToList() ?? new List<VehicleLocation>();

                return locations;
            }
            else
            {
                logger.Warn($"Failed to fetch data for Vehicle ID: {vehicleId} with status: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Error fetching location history for vehicle {vehicleId}");
            return null;
        }
    }

    private DataTable CreateVehicleLocationDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("unitNumber", typeof(string));
        dt.Columns.Add("LocatedAt", typeof(DateTime));
        dt.Columns.Add("Latitude", typeof(double));
        dt.Columns.Add("Longitude", typeof(double));
        dt.Columns.Add("Bearing", typeof(double));
        dt.Columns.Add("SpeedMph", typeof(double));
        dt.Columns.Add("Location", typeof(string));
        dt.Columns.Add("Type", typeof(string));
        return dt;
    }

    void PrintVehicleIdTable(DataTable dt)
    {
        Console.WriteLine("\nVehicle ID Map:");
        foreach (DataRow row in dt.Rows)
        {
            Console.WriteLine($"Id: {row["Id"]}, Number: {row["Number"]}");
        }
        Console.WriteLine($"Total vehicles: {dt.Rows.Count}");
    }

    void PrintVehicleLocationTable(DataTable dt)
    {
        Console.WriteLine($"Vehicle Location Data:");
        foreach (DataRow row in dt.Rows)
        {
            Console.WriteLine($"unitNumber: {row["unitNumber"]}, Located At: {row["LocatedAt"]}, Latitude: {row["Latitude"]}, Longitude: {row["Longitude"]}, Bearing: {row["Bearing"]}, Speed: {row["SpeedMph"]}, Location: {row["Location"]}, Type: {row["Type"]}");
        }

        Console.WriteLine($"Total Locations: {dt.Rows.Count}");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var processor = new VehicleDataProcessor();
        await processor.ProcessVehicleData();
    }
}