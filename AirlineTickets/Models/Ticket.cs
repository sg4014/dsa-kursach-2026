namespace AirlineTicketing.Models;

public class Ticket
{
    public string TicketNumber { get; set; } = string.Empty; // Format NNNNNNNNN
    public string PassportNumber { get; set; } = string.Empty; // Format NNNN-NNNNNN
    public string FlightNumber { get; set; } = string.Empty; // Format AAA-NNN

    public override string ToString()
    {
        return $"Билет №{TicketNumber} | Пассажир: {PassportNumber} | Рейс: {FlightNumber}";
    }
}
