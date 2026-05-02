using AirlineTicketing.Models;
using AirlineTicketing.DataStructures;
using AirlineTicketing.Algorithms;
using System.Globalization;

namespace AirlineTicketing.Services;

public class SystemManager
{
    private readonly QuadraticHashTable _passengers = new QuadraticHashTable();
    private readonly AVLTree _flights = new AVLTree();
    private readonly SkipList _tickets = new SkipList();
    private const string DateFormat = "dd.MM.yyyy";

    // === Passengers ===
    public void AddPassenger(Passenger p)
    {
        if (_passengers.Search(p.PassportNumber) != null)
            throw new Exception("Пассажир с таким номером паспорта уже есть.");

        ValidatePassportIssueDate(p.BirthDate, p.IssueDate);

        _passengers.Insert(p.PassportNumber, p);
    }

    private void ValidatePassportIssueDate(string birthDate, string issueDate)
    {
        if (!DateTime.TryParseExact(birthDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDateTime))
            throw new Exception("Неверный формат даты рождения.");

        if (!DateTime.TryParseExact(issueDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime issueDateTime))
            throw new Exception("Неверный формат даты выдачи паспорта.");

        DateTime minIssueDate = birthDateTime.AddYears(14);

        if (issueDateTime < minIssueDate)
            throw new Exception($"Дата выдачи паспорта не может быть раньше чем через 14 лет с даты рождения. Минимальная дата выдачи: {minIssueDate:dd.MM.yyyy}.");
    }

    public bool DeletePassenger(string passportNumber)
    {
        var passenger = _passengers.Search(passportNumber);
        if (passenger == null) return false;

        // Cascade delete: Return tickets for this passenger
        var ticketsToReturn = _tickets.GetAll().Where(t => t.PassportNumber == passportNumber).ToList();
        foreach (var t in ticketsToReturn)
        {
            ReturnTicket(t.TicketNumber);
        }

        return _passengers.Remove(passportNumber);
    }

    public IEnumerable<Passenger> GetAllPassengers()
    {
        return _passengers.GetAll();
    }

    public void ClearPassengers()
    {
        var allPassPassports = _passengers.GetAll().Select(p => p.PassportNumber).ToList();
        foreach (var p in allPassPassports)
        {
            DeletePassenger(p);
        }
        _passengers.Clear();
    }

    public (Passenger? passenger, List<string> flightNumbers) SearchPassengerByPassport(string passportNumber)
    {
        var p = _passengers.Search(passportNumber);
        if (p == null) return (null, new List<string>());

        var flightNums = _tickets.GetAll()
                                 .Where(t => t.PassportNumber == passportNumber)
                                 .Select(t => t.FlightNumber)
                                 .Distinct()
                                 .ToList();

        return (p, flightNums);
    }

    public List<Passenger> SearchPassengerByFullName(string fullName)
    {
        var result = new List<Passenger>();
        string normalized = string.Join(' ', fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (string.IsNullOrWhiteSpace(normalized))
            return result;
        
        foreach (var p in _passengers.GetAll())
        {
            string full = $"{p.LastName} {p.FirstName} {p.Patronymic}";
            if (full.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(p);
            }
        }
        return result;
    }

    // === Flights ===
    public void AddFlight(Flight f)
    {
        if (_flights.Search(f.FlightNumber) != null)
            throw new Exception("Рейс с таким номером уже есть.");

        f.FreeSeats = f.TotalSeats;
        _flights.Insert(f.FlightNumber, f);
    }

    public bool DeleteFlight(string flightNumber)
    {
        var flight = _flights.Search(flightNumber);
        if (flight == null) return false;

        // Cascade delete: Return tickets for this flight
        var ticketsToReturn = _tickets.GetAll().Where(t => t.FlightNumber == flightNumber).ToList();
        foreach (var t in ticketsToReturn)
        {
            _tickets.Remove(t.TicketNumber); // Hard remove here, returning money basically
        }

        return _flights.Remove(flightNumber);
    }

    public IEnumerable<Flight> GetAllFlights()
    {
        return _flights.GetAll();
    }

    public void ClearFlights()
    {
        var allFlights = _flights.GetAll().Select(f => f.FlightNumber).ToList();
        foreach (var f in allFlights)
        {
            DeleteFlight(f);
        }
        _flights.Clear();
    }

    public (Flight? flight, List<(string Passport, string FullName)> passengersInfo) SearchFlightByNumber(string flightNumber)
    {
        var f = _flights.Search(flightNumber);
        if (f == null) return (null, new List<(string, string)>());

        var pInfo = new List<(string, string)>();
        var tickets = _tickets.GetAll().Where(t => t.FlightNumber == flightNumber);

        foreach (var t in tickets)
        {
            var p = _passengers.Search(t.PassportNumber);
            if (p != null)
            {
                string fullName = $"{p.LastName} {p.FirstName} {p.Patronymic}";
                pInfo.Add((p.PassportNumber, fullName));
            }
        }

        return (f, pInfo);
    }

    public List<(string FlightNum, string ArrAirport, string DepDate, string ArrDate)> SearchFlightByArrivalFragment(string fragment)
    {
        var result = new List<(string, string, string, string)>();
        
        // PostOrder traversal
        var nodes = _flights.PostOrderTraversal(); 
        
        foreach (var f in nodes)
        {
            if (BoyerMoore.Contains(f.ArrivalAirport, fragment))
            {
                result.Add((f.FlightNumber, f.ArrivalAirport, f.DepartureDate, f.ArrivalDate));
            }
        }

        return result;
    }

    // === Tickets ===
    public bool SellTicket(string ticketNumber, string passportNumber, string flightNumber)
    {
        if (_tickets.Contains(ticketNumber))
            throw new Exception("Билет с таким номером уже есть.");

        var passenger = _passengers.Search(passportNumber);
        if (passenger == null) throw new Exception("Пассажир не найден.");

        var flight = _flights.Search(flightNumber);
        if (flight == null) throw new Exception("Рейс не найден.");

        if (flight.FreeSeats <= 0)
        {
            throw new Exception("Свободных мест нет.");
        }

        // Create and add
        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            PassportNumber = passportNumber,
            FlightNumber = flightNumber
        };

        _tickets.Insert(ticket);
        flight.FreeSeats--;

        return true;
    }

    public bool ReturnTicket(string ticketNumber)
    {
        var ticketsList = _tickets.GetAll().ToList();
        var ticket = ticketsList.FirstOrDefault(t => t.TicketNumber == ticketNumber);

        if (ticket == null) return false;

        var flight = _flights.Search(ticket.FlightNumber);
        if (flight != null)
        {
            ++flight.FreeSeats;
        }

        return _tickets.Remove(ticketNumber);
    }

    public IEnumerable<Ticket> GetAllTickets()
    {
        var list = _tickets.GetAll().ToList();
        MergeSort.Sort(list);
        return list;
    }
}
