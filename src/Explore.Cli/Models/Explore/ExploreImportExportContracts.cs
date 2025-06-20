using System.Text.Json.Serialization;

namespace Explore.Cli.Models.Explore;

public partial class ExportSpaces
{
    [JsonRequired]
    [JsonPropertyName("info")]
    public Info? Info { get; set; }

    [JsonRequired]
    [JsonPropertyName("exploreSpaces")]
    public List<ExploreSpace>? ExploreSpaces { get; set; }
}

public partial class ExportSpacesV2
{
    [JsonRequired]
    [JsonPropertyName("info")]
    public Info? Info { get; set; }
    
    [JsonRequired]
    [JsonPropertyName("exploreSpaces")]
    public List<ExploreSpaceV2>? ExploreSpaces { get; set; }
}

public partial class Info
{
    [JsonRequired]
    [JsonPropertyName("version")]
    public string version { get; set; } = "1.0.0";

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

public partial class ExploreSpaceV2
{

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonRequired]
    [JsonPropertyName("apis")]
    public List<ExploreApiV2>? Apis { get; set; }

}

public class ExploreApiV2 : ApiResponseV2
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endpoints")]
    public List<Endpoint>? Endpoints { get; set; }
}


