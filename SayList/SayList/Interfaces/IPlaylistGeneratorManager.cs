using SayList.Things;

namespace SayList.Interfaces;

public interface IPlaylistGeneratorManager
{
    /// <summary>
    /// given a message from a user, returns list of song titles and ids that make up a playlist that spells their message
    /// </summary>
    /// <returns></returns>
    public Task<SaylistBuildResult> BuildPlaylist(string userMessage);


    /// <summary>
    /// given a list of spotify tracks, creates playlist and returns a shareable link
    /// </summary>
    /// <param name="tracks"></param>
    /// <returns></returns>
    public Task<SpotifyPlaylist> CreatePlaylist(IEnumerable<SpotifyTrack> tracks);
}