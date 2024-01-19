using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SayList.Things;

public class SpotifyTrackSearchResponse
{
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("next")]
    public string NextPageUrl { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public IEnumerable<SpotifyTrack> Items { get; set; }

}