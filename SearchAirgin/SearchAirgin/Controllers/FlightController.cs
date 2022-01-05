using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using SearchAirgin.Models;
using SearchAirgin.Models.Responses;

namespace SearchAirgin.Controllers;

public class FlightController : Controller {
    private readonly HttpClientHandler _clientHandler = new();
    private Root _oFlight = new();
    private List<Root> _oFlights = new();

    public FlightController() {
        _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => {
            return true;
        };
    }

    public IActionResult Index() {
        return View();
    }

    [HttpGet] // Get all flights departing at Frankfurt International Airport (EDDF) from 12pm to 1pm on Jan 29 2018
    public async Task<List<Root>> GetFlights_EDDF() {
        _oFlights = new List<Root>();

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response =
                   await httpClient.GetAsync(
                       "https://opensky-network.org/api/flights/departure?airport=EDDF&begin=1517227200&end=1517230800")) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(apiResponse);
                _oFlights = JsonConvert.DeserializeObject<List<Root>>(apiResponse); 
                Console.WriteLine(apiResponse);
            }
        }

        return _oFlights;
    }

    [HttpGet]
    public async Task<List<Root>> GetByDepartureAirportID(string departureAirportID) {
        _oFlights = new List<Root>();

        using (var httpClient = new HttpClient(_clientHandler)) {
            using (var response =
                   await httpClient.GetAsync(
                       "https://opensky-network.org/api/flights/departure?airport=" + departureAirportID +
                       "&begin=1517227200&end=1517230800")) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                _oFlights = JsonConvert.DeserializeObject<List<Root>>(apiResponse);
            }
        }

        return _oFlights;
    }
}