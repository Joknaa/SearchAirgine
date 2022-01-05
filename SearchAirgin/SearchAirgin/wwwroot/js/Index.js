// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function(){
});
                

function GetFlights_EDDF(){
    $.getJSON("/Flight/GetFlights_EDDF",function(flights){
        $("#tblFlights tbody tr").remove();
        $.map(flights, function(flight){
            $("#tblFlights tbody").append("<tr>"
                + "<td>" + flight.icao24 + "</td>"
                + "<td>" + flight.estDepartureAirport + "</td>"
                + "<td>" + flight.estArrivalAirport + "</td>"
                + "</tr>"
            )
        });
    })
}