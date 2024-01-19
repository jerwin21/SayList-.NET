using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SayList.Interfaces;
using SayList.Things;
using SayList.Utilities;

namespace SayList.Engines;

public class SpotifyAuthorizationEngine : IAuthorizationEngine
{
    private IMemoryCache _authTokenCache;
    private readonly HttpRequestUtility _requestUtility;

    private const string _SpotifyAuthTokenUrl = "https://accounts.spotify.com/api/token";
    private const string _SpotifyAuthTokenGrantType = "client_credentials";
    private readonly string _SpotifyClientId;
    private readonly string _SpotifyClientSecret;

    private const string _AuthTokenKey = $"{nameof(SpotifyAuthorizationEngine)}-AuthTokenKey";

    public SpotifyAuthorizationEngine(IConfiguration configuration, IMemoryCache authTokenCache, HttpRequestUtility requestUtility)
    {
        _SpotifyClientId = configuration["SpotifyClientId"];
        _SpotifyClientSecret = configuration["SpotifyClientSecret"];

        _authTokenCache = authTokenCache;
        _requestUtility = requestUtility;
    }
    public async Task<SpotifyAccessTokenResponse> GetAccessTokenAsync()
    {

        if (!_authTokenCache.TryGetValue(_AuthTokenKey, out SpotifyAccessTokenResponse token))
        {
            var requestData = new Dictionary<string, string>
            {
                {"grant_type", _SpotifyAuthTokenGrantType},
                {"client_id", _SpotifyClientId},
                {"client_secret", _SpotifyClientSecret}
            };

            try
            {
                token = await _requestUtility.PostFormUrlEncodedAsync<SpotifyAccessTokenResponse>(_SpotifyAuthTokenUrl, requestData);
                
                _authTokenCache.Set(_AuthTokenKey, token, TimeSpan.FromMinutes(59));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception: {e.Message}");
                throw;
            }
        }

        return token;
    }
}