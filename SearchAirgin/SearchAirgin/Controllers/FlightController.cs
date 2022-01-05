using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SearchAirgin.Models;

namespace SearchAirgin.Controllers;

public class FlightController : Controller {
    private readonly HttpClientHandler _clientHandler = new();
    private Flight _oFlight = new();
    private List<Flight> _oFlights = new();

    public FlightController() {
        _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => {
            return true;
        };
    }

    public IActionResult Index() {
        return View();
    }

    [HttpGet] // Get all flights departing at Frankfurt International Airport (EDDF) from 12pm to 1pm on Jan 29 2018
    public async Task<List<Flight>> GetFlights_EDDF() {
        _oFlights = new List<Flight>();

        using (var httpClient = new HttpClient()) {
            using (var response =
                   await httpClient.GetAsync(
                       "https://opensky-network.org/api/flights/departure?airport=EDDF&begin=1517227200&end=1517230800")) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                _oFlights = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
        }

        return _oFlights;
    }

    [HttpGet]
    public async Task<Flight> GetByDepartureAirportID(string departureAirportID) {
        _oFlight = new Flight();

        using (var httpClient = new HttpClient()) {
            using (var response =
                   await httpClient.GetAsync(
                       "https://opensky-network.org/api/flights/departure?airport=" + departureAirportID +
                       "&begin=1517227200&end=1517230800")) {
                var apiResponse = await response.Content.ReadAsStringAsync();
                _oFlights = JsonConvert.DeserializeObject<List<Flight>>(apiResponse);
            }
        }

        return _oFlight;
    }
}