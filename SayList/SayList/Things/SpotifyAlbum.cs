using System.Text.Json.Serialization;

namespace SayList.Things;

public class SpotifyAlbum
{
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("images")]
    public IEnumerable<SpotifyImage> Images { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}