using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using VehicleTrackingApp.Services;
using VehicleTrackingApp.Models;
using VehicleTrackingApp.Utilities;
// Load configuration and API Key

string ApiKey = null;

var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

var apiKey = configuration["ApiSettings:ApiKey"];

// Initialize services
var vehicleService = new VehicleService(apiKey);

try
{
    // Get all vehicle IDs and numbers
    var vehicleIdMap = await vehicleService.GetAllVehicleIdsAsync();

    // Create DataTable for vehicle location history
    DataTable vehicleLocationTable = CreateVehicleLocationDataTable();

    // Iterate over each vehicle and fetch its location history
    foreach (var vehicle in vehicleIdMap)
    {
        string vehicleId = vehicle["Id"].ToString();
        string unitNumber = vehicle["Number"].ToString();
        Console.WriteLine($"Fetching location history for Vehicle ID: {vehicleId} (Unit Number: {unitNumber})");

        var locationHistory = await vehicleService.GetVehicleLocationHistoryAsync(vehicleId, "2025-01-01", "2025-01-01");

        if (locationHistory != null)
        {
            // Add each location to the DataTable
            foreach (var location in locationHistory)
            {
                vehicleLocationTable.Rows.Add(unitNumber, location.LocatedAt, location.Lat, location.Lon, location.Bearing, location.Speed, location.Description, location.Type);
            }
        }
    }

    // Print the DataTable
    PrintDataTable(vehicleLocationTable);
}
catch (Exception ex)
{
    LoggerHelper.LogError("An error occurred while processing vehicle data.", ex);
}

DataTable CreateVehicleLocationDataTable()
{
    DataTable dt = new DataTable();
    dt.Columns.Add("UnitNumber", typeof(string));
    dt.Columns.Add("LocatedAt", typeof(DateTime));
    dt.Columns.Add("Latitude", typeof(double));
    dt.Columns.Add("Longitude", typeof(double));
    dt.Columns.Add("Bearing", typeof(double));
    dt.Columns.Add("SpeedMph", typeof(double));
    dt.Columns.Add("Location", typeof(string));
    dt.Columns.Add("Type", typeof(string));

    return dt;
}

void PrintDataTable(DataTable dt)
{
    Console.WriteLine("Vehicle Location Data:");
    foreach (DataRow row in dt.Rows)
    {
        Console.WriteLine($"UnitNumber: {row["UnitNumber"]}, LocatedAt: {row["LocatedAt"]}, Latitude: {row["Latitude"]}, Longitude: {row["Longitude"]}, Bearing: {row["Bearing"]}, Speed: {row["SpeedMph"]}, Location: {row["Location"]}, Type: {row["Type"]}");
    }
    Console.WriteLine($"Total Locations: {dt.Rows.Count}");
}
