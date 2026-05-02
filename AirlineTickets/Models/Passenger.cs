namespace AirlineTicketing.Models;

public class Passenger
{
    public string PassportNumber { get; set; } = string.Empty; // Format NNNN-NNNNNN
    public string IssuePlace { get; set; } = string.Empty;
    public string IssueDate { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Пассажир: {LastName} {FirstName} {Patronymic}, Паспорт: {PassportNumber}";
    }

    public string FullData()
    {
        return $"{ToString()}, Выдан: {IssuePlace} {IssueDate}, Дата рождения: {BirthDate}";
    }
}
