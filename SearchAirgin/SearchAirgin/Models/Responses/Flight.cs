using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SearchAirgin.Models.Responses; 

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Flight {
    public string icao24 { get; set; } = "";
    public int firstSeen { get; set; }
    public string estDepartureAirport { get; set; } = "";
    public int lastSeen { get; set; }
    public string estArrivalAirport { get; set; } = "";
    public string callsign { get; set; } = "";
    //[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonProperty("estDepartureAirportHorizDistance",NullValueHandling = NullValueHandling.Ignore)]
    public int? estDepartureAirportHorizDistance;
    //[System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonProperty("estDepartureAirportVertDistance",NullValueHandling = NullValueHandling.Ignore)]
    public int? estDepartureAirportVertDistance;
    public int estArrivalAirportHorizDistance { get; set; }
    public int estArrivalAirportVertDistance { get; set; }
    public int departureAirportCandidatesCount { get; set; }
    public int arrivalAirportCandidatesCount { get; set; }
}