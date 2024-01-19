using SayList.Interfaces;
using SayList.Things;
using SayList.Utilities;

namespace SayList.Engines;

public class SpotifyPlaylistGeneratorEngine : IPlaylistGeneratorEngine
{
    private const string _SpotifySearchUrl = "https://api.spotify.com/v1/search";

    private HttpRequestUtility _httpUtility;

    public SpotifyPlaylistGeneratorEngine(HttpRequestUtility httpUtility)
    {
        _httpUtility = httpUtility;
    }

    public async Task<SpotifyTrackSearchResponse> SearchSpotify(SpotifyAccessTokenResponse authToken, SpotifySearchRequest searchRequest)
    {
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"{authToken.TokenType} {authToken.AccessToken}" }
        };

        var queryParams = new Dictionary<string, string>
        {
            {"query", searchRequest.Query},
            {"type", searchRequest.Type},
            {"limit", searchRequest.Limit.ToString()}
        };

        try
        {
            SpotifySearchResponse searchResponse = await _httpUtility.GetAsync<SpotifySearchResponse>(_SpotifySearchUrl, headers, queryParams);
            return searchResponse.Tracks;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}