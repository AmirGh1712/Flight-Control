using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;

namespace UnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            // Run another web api on localhost:5400
            // Our server runs on localhost:58084
            // connect our server to localhost:5400

            postJson("http://localhost:58084/api/servers", @"{""ServerId"":""12345"", ""ServerURL"":""http://localhost:5400""}");

            // fakeFlight is used for faking a flight object and posting it to the outside server
            string fakeFlight = @"{
                                 ""passengers"": 300,
                                 ""company_name"": ""UnitTest"",
                                 ""initial_location"": {
                                    ""longitude"": 73.214,
                                    ""latitude"": 61.52,
                                    ""date_time"": ""2020-06-24T09:54:21Z""
                                 },
                                 ""segments"": [
                                 {
                                    ""longitude"": 33.234,
                                    ""latitude"": 31.18,
                                    ""timespan_seconds"": 650
                                 }
                                 ]
                                }";

            // post the fakeFlight to the external server
            postJson("http://localhost:5400/api/FlightPlan", fakeFlight);

            // get the fakeFlight back
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:58084/api/Flights?relative_to=2020-06-24T09:54:21Z&sync_all");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string res = reader.ReadToEnd();

                // check that we have the same list

                Assert.IsTrue(res.Contains("UnitTest"));
                //Assert.IsTrue(res == @"[{ ""flight_id"":""91ITU85"",""longitude"":73.214,""latitude"":61.52,""passengers"":300,""company_name"":""UnitTest"",""date_time"":""2020-06-24T09:54:21Z"",""is_external"":false}]");
            }
        }


        private string postJson(string url, string json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}