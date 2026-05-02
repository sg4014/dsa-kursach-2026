using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AirlineTicketing.Models;
using AirlineTicketing.Services;

namespace AirlineTicketing;

public class Program
{
    private static SystemManager _manager = new SystemManager();
    private const string PassportPattern = "^\\d{4}-\\d{6}$";
    private const string FlightPattern = "^[A-Z]{3}-\\d{3}$";
    private const string TicketPattern = "^\\d{9}$";
    private const string DateFormat = "dd.MM.yyyy";
    private const string DateHint = "dd.mm.yyyy";

    public static void Main(string[] args)
    {
        SeedMockData();

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Система продажи авиабилетов ===");
            Console.WriteLine("1.  Зарегистрировать пассажира");
            Console.WriteLine("2.  Удалить пассажира");
            Console.WriteLine("3.  Показать всех пассажиров");
            Console.WriteLine("4.  Найти пассажира по паспорту");
            Console.WriteLine("5.  Найти пассажира по ФИО");
            Console.WriteLine("6.  Очистить список пассажиров");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("7.  Добавить авиарейс");
            Console.WriteLine("8.  Удалить авиарейс");
            Console.WriteLine("9.  Показать все авиарейсы");
            Console.WriteLine("10. Найти авиарейс по номеру");
            Console.WriteLine("11. Найти рейсы по фрагменту аэропорта прибытия");
            Console.WriteLine("12. Очистить список авиарейсов");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("13. Продать билет");
            Console.WriteLine("14. Вернуть билет");
            Console.WriteLine("15. Показать все билеты");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("0.  Выход");
            Console.Write("\nВыберите пункт: ");
            
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1": AddPassenger(); break;
                    case "2": DeletePassenger(); break;
                    case "3": ViewPassengers(); break;
                    case "4": SearchPassengerByPassport(); break;
                    case "5": SearchPassengerByFullName(); break;
                    case "6": _manager.ClearPassengers(); Console.WriteLine("Очищено."); break;

                    case "7": AddFlight(); break;
                    case "8": DeleteFlight(); break;
                    case "9": ViewFlights(); break;
                    case "10": SearchFlightByNumber(); break;
                    case "11": SearchFlightByArrival(); break;
                    case "12": _manager.ClearFlights(); Console.WriteLine("Очищено."); break;

                    case "13": SellTicket(); break;
                    case "14": ReturnTicket(); break;
                    case "15": ViewTickets(); break;

                    case "0": running = false; break;
                    default: Console.WriteLine("Неверный пункт."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            if (running)
            {
                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }
    }

    private static void SeedMockData()
    {
        _manager.AddPassenger(new Passenger { PassportNumber = "1234-567890", LastName = "Иванов", FirstName = "Иван", Patronymic = "Иванович", BirthDate = "01.01.1990", IssueDate = "01.01.2010", IssuePlace = "Москва" });
        _manager.AddPassenger(new Passenger { PassportNumber = "4321-098765", LastName = "Петров", FirstName = "Петр", Patronymic = "Петрович", BirthDate = "02.02.1995", IssueDate = "02.02.2015", IssuePlace = "Санкт-Петербург" });

        _manager.AddFlight(new Flight { FlightNumber = "SSJ-001", Airline = "Аэрофлот", DepartureAirport = "Москва (SVO)", ArrivalAirport = "Дубай (DXB)", DepartureDate = "10.05.2026", ArrivalDate = "10.05.2026", TotalSeats = 200, FreeSeats = 200 });
        _manager.AddFlight(new Flight { FlightNumber = "SSJ-002", Airline = "Аэрофлот", DepartureAirport = "Дубай (DXB)", ArrivalAirport = "Москва (SVO)", DepartureDate = "15.05.2026", ArrivalDate = "15.05.2026", TotalSeats = 150, FreeSeats = 150 });
    }

    private static string Prompt(string msg)
    {
        Console.Write($"{msg}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static string PromptLetters(string msg)
    {
        while (true)
        {
            Console.Write($"{msg}: ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (Regex.IsMatch(input, "^\\p{L}+$", RegexOptions.CultureInvariant))
                return input;

            Console.WriteLine("Неверный формат. Введите только буквы без пробелов и цифр.");
        }
    }

    private static string PromptRegex(string msg, string pattern, string hint, bool toUpper = false)
    {
        while (true)
        {
            Console.Write($"{msg} ({hint}): ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (Regex.IsMatch(input, pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
            {
                return toUpper ? input.ToUpperInvariant() : input;
            }

            Console.WriteLine($"Неверный формат. Ожидается: {hint}.");
        }
    }

    private static string PromptDate(string msg)
    {
        while (true)
        {
            Console.Write($"{msg} ({DateHint}): ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                return dt.ToString(DateFormat, CultureInfo.InvariantCulture);
            }

            Console.WriteLine($"Неверный формат. Ожидается: {DateHint}.");
        }
    }

    private static int PromptPositiveInt(string msg)
    {
        while (true)
        {
            Console.Write($"{msg}: ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (int.TryParse(input, out int value) && value > 0)
                return value;

            Console.WriteLine("Неверное число. Введите положительное целое.");
        }
    }

    private static int PromptIntInRange(string msg, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write($"{msg} ({minValue}-{maxValue}): ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (int.TryParse(input, out int value) && value >= minValue && value <= maxValue)
                return value;

            Console.WriteLine($"Неверное число. Введите значение от {minValue} до {maxValue}.");
        }
    }

    // -- Passenger Operations --
    private static void AddPassenger()
    {
        Console.WriteLine("\n[Регистрация пассажира]");
        string passportNumber = PromptRegex("Паспорт", PassportPattern, "NNNN-NNNNNN");

        var existingPassenger = _manager.SearchPassengerByPassport(passportNumber).passenger;
        if (existingPassenger != null)
        {
            Console.WriteLine("Пассажир с таким номером паспорта уже есть.");
            return;
        }

        var p = new Passenger
        {
            PassportNumber = passportNumber,
            LastName = PromptLetters("Фамилия"),
            FirstName = PromptLetters("Имя"),
            Patronymic = PromptLetters("Отчество"),
            BirthDate = PromptDate("Дата рождения"),
            IssueDate = PromptDate("Дата выдачи"),
            IssuePlace = Prompt("Место выдачи")
        };
        _manager.AddPassenger(p);
        Console.WriteLine("Добавлено.");
    }

    private static void DeletePassenger()
    {
        string p = PromptRegex("\nПаспорт для удаления", PassportPattern, "NNNN-NNNNNN");
        bool res = _manager.DeletePassenger(p);
        Console.WriteLine(res ? "Удалено." : "Не найдено.");
    }

    private static void ViewPassengers()
    {
        Console.WriteLine("\n[Все пассажиры]");
        foreach (var p in _manager.GetAllPassengers())
            Console.WriteLine(p);
    }

    private static void SearchPassengerByPassport()
    {
        string p = PromptRegex("\nПаспорт для поиска", PassportPattern, "NNNN-NNNNNN");
        var (passenger, flights) = _manager.SearchPassengerByPassport(p);
        if (passenger != null)
        {
            Console.WriteLine($"\nНайдено: {passenger.FullData()}");
            if (flights.Count > 0)
                Console.WriteLine($"Купленные рейсы: {string.Join(", ", flights)}");
        }
        else Console.WriteLine("Не найдено.");
    }

    private static void SearchPassengerByFullName()
    {
        string name = Prompt("\nФрагмент ФИО для поиска");
        var res = _manager.SearchPassengerByFullName(name);
        if (res.Count == 0) Console.WriteLine("Не найдено.");
        foreach (var p in res)
            Console.WriteLine($"- {p.PassportNumber}: {p.LastName} {p.FirstName} {p.Patronymic}");
    }

    // -- Flight Operations --
    private static void AddFlight()
    {
        Console.WriteLine("\n[Добавление рейса]");
        string flightNumber = PromptRegex("Номер рейса", FlightPattern, "AAA-NNN", true);

        var existingFlight = _manager.SearchFlightByNumber(flightNumber).flight;
        if (existingFlight != null)
        {
            Console.WriteLine("Рейс с таким номером уже есть.");
            return;
        }

        string departureDate;
        string arrivalDate;

        while (true)
        {
            departureDate = PromptDate("Дата отправления");
            arrivalDate = PromptDate("Дата прибытия");

            DateTime departure = DateTime.ParseExact(departureDate, DateFormat, CultureInfo.InvariantCulture);
            DateTime arrival = DateTime.ParseExact(arrivalDate, DateFormat, CultureInfo.InvariantCulture);

            if (arrival >= departure)
                break;

            Console.WriteLine("Дата прибытия не может быть раньше даты отправления.");
        }

        var f = new Flight
        {
            FlightNumber = flightNumber,
            Airline = Prompt("Авиакомпания"),
            DepartureAirport = Prompt("Аэропорт отправления"),
            ArrivalAirport = Prompt("Аэропорт прибытия"),
            DepartureDate = departureDate,
            ArrivalDate = arrivalDate,
            TotalSeats = PromptIntInRange("Всего мест", 1, 600)
        };
        _manager.AddFlight(f);
        Console.WriteLine("Добавлено.");
    }

    private static void DeleteFlight()
    {
        string fn = PromptRegex("\nНомер рейса для удаления", FlightPattern, "AAA-NNN", true);
        bool res = _manager.DeleteFlight(fn);
        Console.WriteLine(res ? "Удалено." : "Не найдено.");
    }

    private static void ViewFlights()
    {
        Console.WriteLine("\n[Все рейсы]");
        foreach (var f in _manager.GetAllFlights())
            Console.WriteLine(f);
    }

    private static void SearchFlightByNumber()
    {
        string fn = PromptRegex("\nНомер рейса для поиска", FlightPattern, "AAA-NNN", true);
        var (flight, pInfo) = _manager.SearchFlightByNumber(fn);
        if (flight != null)
        {
            Console.WriteLine($"\nНайдено: {flight}");
            foreach (var p in pInfo)
                Console.WriteLine($"  Купил: {p.Passport} | {p.FullName}");
        }
        else Console.WriteLine("Не найдено.");
    }

    private static void SearchFlightByArrival()
    {
        string frag = Prompt("\nФрагмент аэропорта прибытия");
        var res = _manager.SearchFlightByArrivalFragment(frag);
        if (res.Count == 0) Console.WriteLine("Не найдено.");
        foreach (var t in res)
            Console.WriteLine($"- Рейс: {t.FlightNum}, Аэропорт: {t.ArrAirport}, Отпр: {t.DepDate}, Приб: {t.ArrDate}");
    }

    // -- Ticket Operations --
    private static void SellTicket()
    {
        Console.WriteLine("\n[Продажа билета]");
        string tn = PromptRegex("Номер билета", TicketPattern, "9 цифр");
        string pn = PromptRegex("Номер паспорта", PassportPattern, "NNNN-NNNNNN");

        var passenger = _manager.SearchPassengerByPassport(pn).passenger;
        if (passenger == null)
        {
            Console.WriteLine("Пассажир не найден.");
            return;
        }

        string fn = PromptRegex("Номер рейса", FlightPattern, "AAA-NNN", true);
        bool ok = _manager.SellTicket(tn, pn, fn);
        Console.WriteLine(ok ? "Продано." : "Не удалось продать.");
    }

    private static void ReturnTicket()
    {
        string tn = PromptRegex("\nНомер билета для возврата", TicketPattern, "9 цифр");
        bool res = _manager.ReturnTicket(tn);
        Console.WriteLine(res ? "Возврат выполнен." : "Не удалось вернуть.");
    }

    private static void ViewTickets()
    {
        Console.WriteLine("\n[Все билеты]");
        foreach (var t in _manager.GetAllTickets())
            Console.WriteLine(t);
    }
}

