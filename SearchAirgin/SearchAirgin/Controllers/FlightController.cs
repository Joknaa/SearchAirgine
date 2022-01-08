using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SearchAirgin.Models;
using SearchAirgin.Models.Responses;

namespace SearchAirgin.Controllers;

public class FlightController : Controller {
    private readonly HttpClientHandler _clientHandler = new();

    public FlightController() {
        _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => {
            return true;
        };
    }

    public IActionResult Index() {
        return View();
    }

    [HttpGet]
    public async Task<List<Flight>> GetFlights_EDDF() {
        List<Flight> Flights = new List<Flight>();
        const string requestURI =
            "https://opensky-network.org/api/flights/departure?airport=EDDF&begin=1517227200&end=1517230800";

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response = await httpClient.GetAsync(requestURI)) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
        }

        return Flights;
    }

    [HttpGet]
    public async Task<List<Flight>> GetFlights(string departure, string destination, long departureTime) {
        /*
         *  Set arrivalTime = departureTime + MaxSize
         *  Get all flights from DepartureLocation
         *  Get all flights to ArrivalLocation
         *  Filter flights and get the needed result
        */
        const long maxRequestInterval = 600000; // 6 days and 22h in Unix.
        long arrivalTime = departureTime + maxRequestInterval;

        List<Flight>? Flights_Dep, Flights_Des;

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response = await httpClient.GetAsync(
                       $"https://opensky-network.org/api/flights/departure?airport={departure}&begin={departureTime}&end={arrivalTime}"
                   )) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights_Dep = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }

            using (var response = await httpClient.GetAsync(
                       $"https://opensky-network.org/api/flights/arrival?airport={destination}&begin={departureTime}&end={arrivalTime}"
                   )) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights_Des = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
        }

        var searchResult = FilterFlights(Flights_Dep, Flights_Des, departure, destination);

        if (searchResult.Count == 0) {
            
        }

        return searchResult;
    }

    private static List<Flight> FilterFlights(
        List<Flight>? departingFlights,
        List<Flight>? arrivingFlights,
        string departure,
        string destination
    ) {
        List<Flight> Flights = new List<Flight>();

        //string destination = arrivingFlights[0].estArrivalAirport;
        //string departure = departingFlights[0].estDepartureAirport;

        departingFlights?.ForEach(flight => {
            if (flight.estArrivalAirport.Equals(destination)) {
                Flights.Add(flight);
            }
        });

        arrivingFlights?.ForEach(flight => {
            if (flight.estDepartureAirport.Equals(departure)) {
                Flights.Add(flight);
            }
        });

        Flights.Reverse();
        return Flights;
    }
}