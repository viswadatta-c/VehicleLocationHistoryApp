using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using VehicleTrackingApp.Models;
using Newtonsoft.Json;

namespace VehicleTrackingApp.Services
{
    public class VehicleService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public VehicleService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<List<Dictionary<string, object>>> GetAllVehicleIdsAsync(int perPage = 100)
        {
            var vehicleIdMap = new List<Dictionary<string, object>>();
            int currentPage = 1;
            bool morePages = true;

            while (morePages)
            {
                var response = await _httpClient.GetAsync($"https://api.gomotive.com/v1/vehicles?per_page={perPage}&page_no={currentPage}");

                if (!response.IsSuccessStatusCode)
                {
                    break;
                }

                var content = await response.Content.ReadAsStringAsync();
                var vehicleData = JsonConvert.DeserializeObject<dynamic>(content);

                if (vehicleData.vehicles == null || vehicleData.vehicles.Count == 0)
                {
                    morePages = false;
                    break;
                }

                foreach (var vehicle in vehicleData.vehicles)
                {
                    vehicleIdMap.Add(new Dictionary<string, object>
                    {
                        { "Id", vehicle.vehicle.id },
                        { "Number", vehicle.vehicle.number }
                    });
                }

                currentPage++;
            }

            return vehicleIdMap;
        }

        public async Task<List<VehicleLocation>> GetVehicleLocationHistoryAsync(string vehicleId, string startDate, string endDate)
        {
            var url = $"https://api.gomotive.com/v3/vehicle_locations/{vehicleId}?start_date={startDate}&end_date={endDate}";
            var client = new RestClient(url);
            var request = new RestRequest();

            request.AddHeader("accept", "application/json");
            request.AddHeader("x-api-key", _apiKey);

            var response = await client.GetAsync(request);

            if (response.IsSuccessful)
            {
                var vehicleData = JsonConvert.DeserializeObject<ApiResponse>(response.Content);
                List<VehicleLocation> locations = new List<VehicleLocation>();

                foreach (var vehicleLocation in vehicleData.VehicleLocations)
                {
                    locations.Add(vehicleLocation.VehicleLocation);
                }

                return locations;
            }
            else
            {
                return null;
            }
        }
    }
}
