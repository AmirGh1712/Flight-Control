using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {

        //private IMemoryCache _cache;
        public FlightPlanController()
        {
            //_cache = cache;
            //_cache.Set("keys", new List<string>());


            //InitialLocation l = new InitialLocation(10, 10, "2020-12-26T23:56:21Z");
            //List<Segment> list = new List<Segment>();
            //list.Add(new Segment(60, 60, 650));
            //_cache.Set("12-ABC-34", new FlightPlan(180, "yuval", l, list));
            //((List<String>)_cache.Get("keys")).Add("12-ABC-34");
            //_cache.Set("11-kVJ-22", new FlightPlan(180, "sapir", l, list));
            //((List<String>)_cache.Get("keys")).Add("11-kVJ-22");
            //_cache.Set("88-kVJ-77", new FlightPlan(180, "oren", l, list));
            //((List<String>)_cache.Get("keys")).Add("88-kVJ-77");




        }


        //// GET: api/FlightPlan
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/FlightPlan/12-ABC-34

        [HttpGet("{id}", Name = "Get")]
        public ActionResult<FlightPlan> Get(string id)
        {

            bool isOK = FlightsHandler._cache.TryGetValue(id, out FlightPlan flightPlan);
            if (isOK)
            {
                return Ok(flightPlan);
            } else
            {
                FlightPlan f = GetAllExternal(id);
                if (f != null)
                    return Ok(f);
            }
            return NotFound(id);
        }

        HttpClient client = new HttpClient();
        async Task<FlightPlan> GetProductAsync(string path)
        {
            FlightPlan product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<FlightPlan>();
            }
            return product;
        }
        private FlightPlan GetExternal(server server, string id)
        {
            FlightPlan  result = null;
            if (server != null)
            {
                result = GetProductAsync(server.ServerURL + "/api/FlightPlan/" + id).GetAwaiter().GetResult();
            }
            return result;
        }

        private FlightPlan GetAllExternal(string id)
        {
            if (FlightsHandler._cache.Get<List<string>>("ids") != null)
            {
                foreach (string key in FlightsHandler._cache.Get<List<string>>("ids"))
                {
                    server s = FlightsHandler._cache.Get<server>(key);
                    if (s != null)
                    {
                        FlightPlan fp = GetExternal(s, id);
                        if (fp != null)
                            return fp;
                    }
                }
            }
            return null;
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult Post([FromBody] FlightPlan flightPlan)
        {
            List<string> list = (List<string>)FlightsHandler._cache.Get("keys");

            Console.WriteLine("before");
            foreach (var id in list)
            {
                Console.WriteLine(id);
            }
            Console.WriteLine("\n");

            string ID = generateID();
            FlightsHandler._cache.Set(ID, flightPlan);
            list.Add(ID);

            //ID = generateID();
            //FlightsHandler._cache.Set(ID, flightPlan);
            //list.Add(ID);

            Console.WriteLine("after");
            foreach (var id in list)
            {
                Console.WriteLine(id);
            }
            Console.WriteLine("\n");

            return Ok(flightPlan);
        }


        private string generateID()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            int num = random.Next(10, 100);
            sb.Append(num);
            char c = Convert.ToChar(random.Next(65, 90));
            sb.Append(c);
            c = Convert.ToChar(random.Next(65, 90));
            sb.Append(c);
            c = Convert.ToChar(random.Next(65, 90));
            sb.Append(c);
            num = random.Next(10, 100);
            sb.Append(num);
            return sb.ToString();
        }

        //// PUT: api/FlightPlan/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}