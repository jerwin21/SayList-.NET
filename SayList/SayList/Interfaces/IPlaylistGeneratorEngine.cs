using SayList.Things;

namespace SayList.Interfaces;

public interface IPlaylistGeneratorEngine
{
    public Task<SpotifyTrackSearchResponse> SearchSpotify(SpotifyAccessTokenResponse authToken, SpotifySearchRequest searchRequest);
}