using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {

        [JsonProperty("passengers")]
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }

        [JsonProperty("company_name")]
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("initial_location")]
        [JsonPropertyName("initial_location")]
        public InitialLocation InitialLocationn { get; set; }

        [JsonProperty("segments")]
        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; }

        public FlightPlan(int passemgers, string companyName, InitialLocation initialLocation, List<Segment> segments)
        {
            Passengers = passemgers;
            CompanyName = companyName;
            InitialLocationn = initialLocation;
            Segments = segments;
        }
        public FlightPlan()
        {

        }
    }
}