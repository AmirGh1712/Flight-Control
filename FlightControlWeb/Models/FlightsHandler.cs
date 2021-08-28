using FlightControlWeb.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightsHandler : IFlightsHandler
    {
        public static IMemoryCache _cache;
        public FlightsHandler(IMemoryCache cache)
        {
            _cache = cache;
            _cache.Set("keys", new List<string>());
            _cache.Set("ids", new List<string>());
        }



        public List<Flight> allFlights(string relativeTo, bool syncAll)
        {
            List<Flight> flights = new List<Flight>();

            bool isOK = _cache.TryGetValue("keys", out List<string> keys);
            if (isOK)
            {
                foreach (var id in keys)
                {
                    Flight f = createFlight(id, (FlightPlan)_cache.Get(id), relativeTo);
                    if (f != null)
                        flights.Add(f);
                }
            }
            if (syncAll)
            {
                IEnumerable<Flight> external = GetExternal(relativeTo);
                if (external != null)
                {
                    foreach (Flight f in external)
                    {
                        flights.Add(f);
                    }
                }
            }

            return flights;

        }

        public Flight createFlight(string id, FlightPlan flightPlan, string relativeTo)
        {

            DateTime currentTime = DateTime.ParseExact(relativeTo, "yyyy-MM-ddTHH:mm:ssZ",
                null);

            DateTime initialTimeSegment = DateTime.ParseExact(flightPlan.InitialLocationn.DateTimee,
                "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

            if (DateTime.Compare(currentTime, initialTimeSegment) < 0)
            {
                return null;
            }
            double span = CalculateTimeSpan(flightPlan.InitialLocationn.DateTimee, relativeTo);
            Segment lastSegment = new Segment(flightPlan.InitialLocationn.Longitude, flightPlan.InitialLocationn.Latitude, 0);
            Segment current = null;
            int sum = 0;
            foreach (Segment s in flightPlan.Segments)
            {
                sum += (int)s.Seconds;
                if (sum > span)
                {
                    current = s;
                    break;
                }
                lastSegment = s;
            }
            if (current == null)
                return null;
            double secondsFromSegment = span + current.Seconds - sum;
            Segment location = CalculatePoint(lastSegment, current, secondsFromSegment);
            Flight flight = new Flight();
            flight.Latitude = location.Latitude;
            flight.Longitude = location.Longitude;
            flight.Passengers = flightPlan.Passengers;
            flight.CompanyName = flightPlan.CompanyName;
            flight.DateTimee = relativeTo;
            flight.IsExternal = false;
            flight.Id = id;
            return flight;
            
        }
        private static Segment CalculatePoint(Segment last, Segment current, double seconds)
        {
            Segment result = new Segment();
            double k = seconds;
            double l = current.Seconds - seconds;
            result.Latitude = (l * last.Latitude + k * current.Latitude) / (k + l);
            result.Longitude = (l * last.Longitude + k * current.Longitude) / (k + l);
            return result;
        }
        private static double CalculateTimeSpan(string begin, string end)
        {
            string pattern = "yyyy-MM-ddTHH:mm:ssZ";
            DateTime parsedDateBegin, parsedDateEnd;
            if (!DateTime.TryParseExact(begin, pattern, null,
                                   DateTimeStyles.None, out parsedDateBegin))
                return -1;
            if (!DateTime.TryParseExact(end, pattern, null,
                                   DateTimeStyles.None, out parsedDateEnd))
                return -1;
            TimeSpan span = parsedDateEnd - parsedDateBegin;
            return span.TotalSeconds;
        }


        public Tuple<double, double> calculateLocation(Segment segment, double seconds,
            double initialLongitude, double initialLatitude)
        {
            double numerator = seconds;
            double denominator = segment.Seconds - seconds;
            double resultLongitude = (denominator * initialLongitude +
                numerator * segment.Longitude) / segment.Seconds;
            double resultLatitude = (denominator * initialLatitude +
                numerator * segment.Latitude) / segment.Seconds;

            return Tuple.Create(resultLongitude, resultLatitude);
        }

        public IEnumerable<Flight> GetExternal(string relative_to)
        {
            List<server> servers = new List<server>();
            if (_cache.Get<List<string>>("ids") != null)
            {
                foreach (string key in _cache.Get<List<string>>("ids"))
                {
                    if (_cache.Get<server>(key) != null)
                        servers.Add(_cache.Get<server>(key));
                }
                return GetAll(servers, relative_to);
            }
            return new List<Flight>();
        }

        static HttpClient client = new HttpClient();
        static async Task<IEnumerable<Flight>> GetProductAsync(string path)
        {
            IEnumerable<Flight> product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<IEnumerable<Flight>>();
            }
            return product;
        }
        public static IEnumerable<Flight> Get(server server, string relativeTo)
        {
            IEnumerable<Flight> result = null;
            if (server != null)
            {
                result = GetProductAsync(server.ServerURL + "/api/Flights?relative_to=" + relativeTo).GetAwaiter().GetResult();
            }
            return result;
        }

        public static IEnumerable<Flight> GetAll(List<server> servers, string relativeTo)
        {
            List<Flight> result = new List<Flight>();
            foreach (server s in servers)
            {
                IEnumerable<Flight> serverFlights = Get(s, relativeTo);
                if (serverFlights == null)
                    continue;
                foreach (Flight f in serverFlights)
                {
                    f.IsExternal = true;
                    result.Add(f);
                }
            }
            return result;
        }
    }
}