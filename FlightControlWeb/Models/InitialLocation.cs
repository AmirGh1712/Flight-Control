using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {

        [JsonProperty("longitude")]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("date_time")]
        [JsonPropertyName("date_time")]
        public string DateTimee { get; set; }

        public InitialLocation(double longitude, double latitude, string dateTime)
        {
            Longitude = longitude;
            Latitude = latitude;
            DateTimee = dateTime;
        }
        public InitialLocation()
        {

        }
    }
}