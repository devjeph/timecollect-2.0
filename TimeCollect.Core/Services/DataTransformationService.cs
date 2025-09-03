// TimeCollect.Core/Services/DataTransformationService.cs

using TimeCollect.Core.Models;
using TimeCollect.Core.Utils;

namespace TimeCollect.Core.Services;

/// <summary>
/// Contains the logic to transform the raw sheet data into a structured format.
/// </summary>
public class DataTransformationService
{
    private readonly DateService _dateService;

    public DataTransformationService(DateService dateService)
    {
        _dateService = dateService;
    }

    public List<List<object>> TransformData(List<WeekTypeInfo> weekTypeSchedule, IList<IList<object>> rawTimesheetData, Employee employee, IList<IList<object>> rawProjectData)
    {
        var transformedData = new List<List<object>>();
        if (rawTimesheetData == null || !rawTimesheetData.Any() || employee == null) return transformedData;
        
        // Ensure the first row has enough columns
        while (rawTimesheetData[0].Count < 52)
        {
            rawTimesheetData[0].Add("0.00");
        }
        
        // Define columns to be removed
        var columnsToRemove = new List<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 21, 26, 31, 36, 41, 46, 51, 56, 61, 66, 71 };
        var data = DataCleaner.DeleteColumns(rawTimesheetData, columnsToRemove);
        
        // Clean data: Replace first 9 elements of the second row with the first row
        for (int i = 0; i < 9 && i < data[0].Count && i < data[1].Count; i++)
        {
            data[1][i] = data[0][i];
        }
        
        // Define work types
        var workData = new List<string>();
        workData.AddRange(Enumerable.Repeat("日付", 3));
        workData.AddRange(Enumerable.Repeat("間接", 8));
        workData.AddRange(Enumerable.Repeat("直接", 40)); // for year 2025
        
        // Propagate project codes
        for (int i = 0; i < 3; i++)
        {
            for (int j = 12; j <= 48; j += 4)
            {
                if (j - 1 < data[0].Count && (i + j) < data[0].Count)
                {
                    data[0][i + j] = data[0][j - 1];
                }
            }
        }
        
        var projectHelper = new ProjectHelper(rawProjectData);

        for (int col = 0; col < data[0].Count - 3; col++)
        {
            for (int row = 2; row < data.Count; row++)
            {
                if (data[row].Count <= col + 3) continue;
                
                int.TryParse(data[row][0].ToString(), out var year);
                int.TryParse(data[row][1].ToString(), out var month);
                int.TryParse(data[row][2].ToString(), out var day);
                
                if (year == 0 || month == 0 || day == 0) continue;
                
                var date = new DateOnly(year, month, day);
                
                var weekType = _dateService.GetWeekTypeName(weekTypeSchedule, date);
                var taskType = data[1][col + 3].ToString();
                var projectCode = data[0][col + 3].ToString();
                var workType = workData[col + 3];
                double.TryParse(data[row][col + 3].ToString(), out var workedHours);
                var client = projectHelper.GetClient(projectCode);
                
                transformedData.Add(new List<object>
                {
                    client,
                    row - 1,
                    year,
                    month,
                    day,
                    weekType,
                    employee.Nickname,
                    projectCode,
                    taskType,
                    workType,
                    employee.Team,
                    Math.Round(workedHours, 2)
                });
            }
        }
        return transformedData;
    }

}