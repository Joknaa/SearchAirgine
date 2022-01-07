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

        //var departingFlights = await GetFlights_FromDeparture(departure, departureTime, arrivalTime);
        //Console.WriteLine("Got the GetFlights_FromDeparture");
        //var arrivingFlights = await GetFlights_ToDestination(destination, departureTime, arrivalTime);
        //Console.WriteLine("Got the GetFlights_ToDestination");
        //var searchResult = FilterFlights(departingFlights, arrivingFlights);
        
        
        List<Flight>? Flights_Dep;
        List<Flight>? Flights_Des;
        string request_1_URI =
            $"https://opensky-network.org/api/flights/departure?airport={departure}&begin={departureTime}&end={arrivalTime}";
        string request_2_URI =
            $"https://opensky-network.org/api/flights/arrival?airport={destination}&begin={departureTime}&end={arrivalTime}";

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response = await httpClient.GetAsync(request_1_URI)) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights_Dep = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
            using (var response = await httpClient.GetAsync(request_2_URI)) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights_Des = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
        }
        
        
        var searchResult = FilterFlights(Flights_Dep, Flights_Des, departure, destination);
        
        return searchResult;
    }
    
    private async Task<List<Flight>?> GetFlights_FromDeparture(string departure, long departureTime, long arrivalTime) {
        List<Flight>? _oFlights;
        string requestURI =
            $"https://opensky-network.org/api/flights/departure?airport={departure}&begin={departureTime}&end={arrivalTime}";

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response = await httpClient.GetAsync(requestURI)) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                _oFlights = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
            httpClient.Dispose();
        }

        return _oFlights;
    }
    private async Task<List<Flight>?> GetFlights_ToDestination(string destination, long departureTime, long arrivalTime) {
        List<Flight>? Flights;
        string requestURI =
            $"https://opensky-network.org/api/flights/arrival?airport={destination}&begin={departureTime}&end={arrivalTime}";

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response = await httpClient.GetAsync(requestURI)) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Flights = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
            httpClient.Dispose();
        }

        return Flights;
    }

    private static List<Flight> FilterFlights(List<Flight> departingFlights, List<Flight> arrivingFlights, string departure, string destination) {
        List<Flight> Flights = new List<Flight>();
        
        //string destination = arrivingFlights[0].estArrivalAirport;
        //string departure = departingFlights[0].estDepartureAirport;

        departingFlights.ForEach(flight => {
            if (flight.estArrivalAirport.Equals(destination)) {
                Flights.Add(flight);
            }
        });
        
        arrivingFlights.ForEach(flight => {
            if (flight.estDepartureAirport.Equals(departure)) {
                Flights.Add(flight);
            }
        });
        
        return Flights;
    }
}