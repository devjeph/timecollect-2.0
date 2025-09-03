// TimeCollect.Core/Models/Employee.cs

namespace TimeCollect.Core.Models;

/// <summary>
/// Represents an employee's data
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string SpreadsheetId { get; set; }
    public string Team { get; set; }

    public Employee(int id, string name, string nickname, string spreadsheetId, string team)
    {
        Id = id;
        Name = name;
        Nickname = nickname;
        SpreadsheetId = spreadsheetId;
        Team = team;
    }
}