using System.Text.Json.Serialization;

namespace SayList.Things;

public class SpotifyTrack
{
    [JsonPropertyName("album")]
    public SpotifyAlbum Album { get; set; }

    [JsonPropertyName("artists")]
    public IEnumerable<SpotifyArtist> Artists { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }

    /// <summary>
    /// spotify id for track
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }


}