using System;
using System.Collections.Generic;

namespace FlightControlWeb.Models
{
    public interface IFlightsHandler
    {
        List<Flight> allFlights(string relativeTo, bool syncAll);
        Flight createFlight(string id, FlightPlan flightPlan, string relativeTo);
    }
}