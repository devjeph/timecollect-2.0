// TimeCollect.Core/Utils/ProjectHelper.cs

namespace TimeCollect.Core.Utils;

/// <summary>
/// Helper class to manage and retrieve project data.
/// </summary>
public class ProjectHelper
{
    private readonly Dictionary<string, string> _projectClients;

    public ProjectHelper(IList<IList<object>> rawProjectData)
    {
        _projectClients = new Dictionary<string, string>();
        if (rawProjectData != null)
        {
            foreach (var project in rawProjectData)
            {
                if (project.Count >= 3)
                {
                    var projectCode = project[0].ToString();
                    var client = project[2].ToString();
                    if (!string.IsNullOrEmpty(projectCode))
                    {
                        _projectClients[projectCode] = client;
                    }
                }
            }
        }
    }

    public string GetClient(string projectCode)
    {
        return _projectClients.TryGetValue(projectCode, out var client) ? client : "YTP";
    }
}