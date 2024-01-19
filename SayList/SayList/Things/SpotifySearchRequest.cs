using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SayList.Things;

[DataContract]
public class SpotifySearchRequest
{
    [DataMember]
    public string Query { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public long Limit { get; set; }


}