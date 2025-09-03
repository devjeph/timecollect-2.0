// TimeCollect.Core/Services/DateService.cs

using TimeCollect.Core.Models;

namespace TimeCollect.Core.Services;

/// <summary>
/// Provides functionalities related to date calculations and week types.
/// </summary>
public class DateService
{
    public List<WeekTypeInfo> SetWeekTypes(int year, int month, int day)
    {
        var dates = SetDates(year, month, day);
        var weekTypeSchedule = new List<WeekTypeInfo>();

        for (int i = 0; i < dates.sundays.Count; i++)
        {
            weekTypeSchedule.Add(new WeekTypeInfo
            {
                StartDate = dates.sundays[i],
                EndDate = dates.saturdays[i],
                WeekNumber = dates.weekIndices[i],
                WeekType = dates.weekNames[i]
            });
        }
        return weekTypeSchedule;
    }

    public string GetWeekTypeName(List<WeekTypeInfo> weekTypeSchedule, DateOnly targetDate)
    {
        foreach (var data in weekTypeSchedule)
        {
            if (targetDate >= data.StartDate && targetDate <= data.EndDate)
            {
                return data.WeekType;
            }
        }
        return null;
    }

    private static (List<DateOnly> sundays, List<DateOnly> saturdays, List<int> weekIndices, List<string> weekNames) SetDates(
        int year, int month, int day)
    {
        var sundays = new List<DateOnly>();
        var saturdays = new List<DateOnly>();
        var weekIndices = new List<int>();
        var weekNames = new List<string>();
        
        var startDate = new DateOnly(year, month, day);
        if (startDate.DayOfWeek != DayOfWeek.Sunday)
        {
            return (sundays, saturdays, weekIndices, weekNames);
        }

        for (var weekIndex = 0; weekIndex < 104; weekIndex++)
        {
            var sunday = startDate.AddDays(weekIndex * 7);
            var saturday = sunday.AddDays(6);
            
            if (sunday.Year >= year + 2) break;
            
            var weekType = SetWeekName(sunday, saturday);
            
            sundays.Add(sunday);
            saturdays.Add(saturday);
            weekIndices.Add(weekIndex + 1);
            weekNames.Add(weekType);
        }

        if (weekNames.Count != 0)
        {
            weekNames[^1] = "12to1";
        }
        
        return (sundays, saturdays, weekIndices, weekNames);
    }

    private static string SetWeekName(DateOnly startDate, DateOnly endDate)
    {
        var startMonth = startDate.Month;
        var endMonth = endDate.Month;

        if (startMonth != endMonth && startDate.Year != endDate.Year)
        {
            return $"{endMonth}{'A'}";
        }

        if (startMonth != endMonth && startDate.Year == endDate.Year)
        {
            return $"{startMonth}to{endMonth}";
        }

        var letterIndex = (startDate.Day - 1) / 7;
        return startMonth == 1 ? $"{startMonth}{(char)('B' + letterIndex)}" : $"{startMonth}{(char)('A' + letterIndex)}";
    }
}