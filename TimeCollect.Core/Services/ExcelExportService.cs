// TimeCollect.Core/Services/ExcelExportService.cs

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace TimeCollect.Core.Services;

/// <summary>
/// Handles exporting data to an Excel file.
/// </summary>
public class ExcelExportService
{
    private readonly ILogger<ExcelExportService> _logger;
    private readonly string _outputDirectory;
    private readonly string _fileName = "TimeCollect.xlsx";

    public ExcelExportService(ILogger<ExcelExportService> logger, IConfiguration config)
    {
        _logger = logger;
        _outputDirectory = config["Output:Directory"];
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public void Export(List<List<object>> sheetData, string sheetName)
    {
        var filePath = Path.Combine(_outputDirectory, _fileName);
        Directory.CreateDirectory(_outputDirectory);
        
        var fileInfo = new FileInfo(filePath);
        using (var package = new ExcelPackage(fileInfo))
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet != null)
            {
                package.Workbook.Worksheets.Delete(sheetName);
            }
            worksheet = package.Workbook.Worksheets.Add(sheetName);
            
            // Add Headers
            var headers = new List<string>
            {
                "対応",
                "行番号",
                "年",
                "月",
                "日",
                "WeekType",
                "名前",
                "工号",
                "種別",
                "直接/間接",
                "原寸/3D/管理",
                "時間"
            };

            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }
            
            // Add data
            if (sheetData.Any())
            {
                worksheet.Cells[2, 1].LoadFromArrays(sheetData.Select(row => row.ToArray()));
            }
            
            package.Save();
        }
        
        _logger.LogInformation($"💾 Filename: {_fileName} Saved to {_outputDirectory}.");
    }
}