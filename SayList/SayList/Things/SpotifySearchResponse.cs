using System.Text.Json.Serialization;

namespace SayList.Things;

public class SpotifySearchResponse
{
    [JsonPropertyName("tracks")]
    public SpotifyTrackSearchResponse Tracks { get; set; }
}
