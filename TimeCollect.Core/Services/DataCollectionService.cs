// TimeCollect.Core/Services/DataCollectionService.cs

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Logging;

namespace TimeCollect.Core.Services;

/// <summary>
/// Responsible for fetching data from Google Sheets.
/// </summary>
public class DataCollectionService
{
    private readonly ILogger<DataCollectionService> _logger;

    public DataCollectionService(ILogger<DataCollectionService> logger)
    {
        _logger = logger;
    }

    public async Task<IList<IList<object>>?> GetDataAsync(UserCredential userCredential, string spreadsheetId,
        string range)
    {
        try
        {
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential,
                ApplicationName = "TimeCollect 2.0",
            });
            
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var values = response.Values;

            if (values is { Count: > 0 })
            {
                //Replace blank values with "0.00"
                foreach (var row in values)
                {
                    for (int j = 0; j < row.Count; j++)
                    {
                        if (string.IsNullOrWhiteSpace(row[j]?.ToString()))
                        {
                            row[j] = "0.00";
                        }
                    }
                }
            }
            
            return values;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}