// TimeCollect.Core/Utils/DataCleaner.cs

namespace TimeCollect.Core.Utils;

/// <summary>
/// Utility class for data cleaning operations.
/// </summary>
public static class DataCleaner
{
    public static IList<IList<object>> DeleteColumns(IList<IList<object>> data, List<int> columnIndices)
    {
        var result = new List<IList<object>>();
        var indicesToRemove = new HashSet<int>(columnIndices);

        foreach (var row in data)
        {
            var newRow = new List<object>();
            for (int i = 0; i < row.Count; i++)
            {
                if (!indicesToRemove.Contains(i))
                {
                    newRow.Add(row[i]);
                }
            }
            result.Add(newRow);
        }
        return result;
    }
}