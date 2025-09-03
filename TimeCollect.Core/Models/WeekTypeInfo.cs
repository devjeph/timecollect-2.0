// TimeCollect.Core/Models/WeekTypeInfo.cs

namespace TimeCollect.Core.Models;

/// <summary>
/// Represents the date range for a specific week type.
/// </summary>
public class WeekTypeInfo
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int WeekNumber { get; set; }
    public string WeekType { get; set; }
}