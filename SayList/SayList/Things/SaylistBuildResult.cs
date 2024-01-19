namespace SayList.Things;

public class SaylistBuildResult
{

    public bool IsSuccess { get; set; }

    /// <summary>
    /// Boolean representing whether song titles were found for each word of user message (true) or if
    /// some word(s) had to be skipped (false)
    /// </summary>
    public bool IsComplete { get; set; }

    public IEnumerable<SpotifyTrack> Tracks { get; set; }

    public string Message { get; set; }


}