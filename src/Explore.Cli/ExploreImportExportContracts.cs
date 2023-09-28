using System.Text.Json.Serialization;
using Explore.Cli.Models;

public partial class ExportSpaces
{
    [JsonRequired]
    [JsonPropertyName("info")]
    public Info? Info { get; set; }
    
    [JsonRequired]
    [JsonPropertyName("exploreSpaces")]
    public List<ExploreSpace>? ExploreSpaces { get; set; }
}

public partial class Info
{
    [JsonRequired]
    [JsonPropertyName("version")]
    public string version { get; set; } = "0.0.1";

    [JsonPropertyName("exportedAt")]
    public string? ExportedAt { get; set; }
}

public partial class ExploreSpace
{

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonRequired]
    [JsonPropertyName("apis")]
    public List<ExploreApi>? apis { get; set; }

}

public class ExploreApi : ApiResponse
{
    [JsonPropertyName("connections")]
    public List<Connection>? connections { get; set; }
}


