using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {

        [JsonPropertyName("flight_id")]
        [JsonProperty("flight_id")]
        public string Id { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
        public string DateTimee { get; set; }

        [JsonPropertyName("is_external")]
        [JsonProperty("is_external")]
        public bool IsExternal { get; set; }

        public Flight(string id, double longitude, double latitude, int passengers,
            string companyName, string dataTime, bool isExternal)
        {
            Id = id;
            Longitude = longitude;
            Latitude = latitude;
            Passengers = passengers;
            CompanyName = companyName;
            DateTimee = dataTime;
            IsExternal = isExternal;
        }
        public Flight()
        {

        }
    }
}