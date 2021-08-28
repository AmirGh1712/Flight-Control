using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class serversController : ControllerBase
    {
        // GET: api/servers
        [HttpGet]
        public ActionResult<IEnumerable<server>> Get()
        {
            List<string> ids = (List<string>)FlightsHandler._cache.Get("ids");
            List<server> servers = new List<server>();

            foreach (var id in ids)
            {
                server s = (server)FlightsHandler._cache.Get(id);
                servers.Add(s);
            }
            return Ok(servers);
        }

        // POST: api/servers
        [HttpPost]
        public void Post([FromBody] server s)
        {
            string id = s.generateID();
            FlightsHandler._cache.Set(id, s);
            List<string> ids = (List<string>)FlightsHandler._cache.Get("ids");
            ids.Add(id);

        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            ((List<string>)FlightsHandler._cache.Get("ids")).Remove(id);
            FlightsHandler._cache.Remove(id);
        }
    }
}
