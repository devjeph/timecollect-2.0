// TimeCollect.Core/Services/GoogleApiService.cs

using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace TimeCollect.Core.Services;

/// <summary>
/// Handles authentication and connection to the Google Sheets API.
/// </summary>
public class GoogleApiService
{
    private readonly ILogger<GoogleApiService> _logger;
    private readonly IConfiguration _config;
    private readonly string[] _scopes;
    private readonly string _credentialsPath;
    private readonly string _tokenPath;

    public GoogleApiService(ILogger<GoogleApiService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _scopes = new[] { _config["GoogleApi:Scopes"] };
        _credentialsPath = config["GoogleApi:CredentialsPath"];
        _tokenPath = config["GoogleApi:TokenPath"];
    }

    public async Task<UserCredential> GetUserCredentialAsync()
    {
        try
        {
            await using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
            
            var credPath = Path.GetDirectoryName(_tokenPath);
            var dataStore = new FileDataStore(credPath, true);

            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                _scopes,
                "user",
                CancellationToken.None,
                dataStore
                );
            _logger.LogInformation("Google API credentials loaded successfully.");
            return credentials;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user credentials");
            return null;
        }
    }
    
}