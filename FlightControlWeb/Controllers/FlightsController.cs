using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {

        //private IMemoryCache _cache;
        private IFlightsHandler _flightsHandler;

        public FlightsController(IFlightsHandler flightsHandler)
        {

            
            _flightsHandler = flightsHandler;
        }

        // GET: api/Flights?relative_to=<DATA_TIME>
        [HttpGet]
        public IEnumerable<Flight> Get([FromQuery(Name = "relative_to")] string relativeTo)
        {
            string s = Request.QueryString.Value;
            return _flightsHandler.allFlights(relativeTo, s.Contains("sync_all"));
        }

        

        public void updateJson(List<Flight> json)
        {
            foreach (Flight fl in json)
            {
                fl.IsExternal = true;
            }
        }

        


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            ((List<string>)FlightsHandler._cache.Get("keys")).Remove(id);
            FlightsHandler._cache.Remove(id);
        }

    }
}