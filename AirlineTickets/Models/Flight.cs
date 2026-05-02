namespace AirlineTicketing.Models;

public class Flight
{
    public string FlightNumber { get; set; } = string.Empty; // Format AAA-NNN
    public string Airline { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public string DepartureDate { get; set; } = string.Empty; // Format dd.mm.yyyy
    public string ArrivalDate { get; set; } = string.Empty;   // Format dd.mm.yyyy
    public int TotalSeats { get; set; }
    public int FreeSeats { get; set; }

    public override string ToString()
    {
        return $"Рейс {FlightNumber}: {DepartureAirport} -> {ArrivalAirport} (свободно {FreeSeats}/{TotalSeats}) {DepartureDate} - {ArrivalDate}, {Airline}";
    }
}
