using System.Text.Json.Serialization;

namespace SayList.Things;

public class SpotifyArtist
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("images")]
    public IEnumerable<SpotifyImage> Image { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}