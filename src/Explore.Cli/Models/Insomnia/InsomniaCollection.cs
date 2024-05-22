using System.Text.Json.Serialization;

namespace Explore.Cli.Models.Insomnia;

public class InsomniaCollection
{
    [JsonPropertyName("_type")]
    public string? Type { get; set; }

    [JsonPropertyName("__export_format")]
    public int? ExportFormat { get; set; }

    [JsonPropertyName("__export_date")]
    public string? ExportDate { get; set; }

    [JsonPropertyName("__export_source")]
    public string? ExportSource { get; set; }

    [JsonPropertyName("resources")]
    public List<Resource>? Resources { get; set; }
}