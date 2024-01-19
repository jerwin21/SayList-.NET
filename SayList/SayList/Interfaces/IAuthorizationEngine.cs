using SayList.Things;

namespace SayList.Interfaces;

public interface IAuthorizationEngine
{
    public Task<SpotifyAccessTokenResponse> GetAccessTokenAsync();
}