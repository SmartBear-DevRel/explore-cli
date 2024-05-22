namespace Explore.Cli.Models;

public partial class SchemaValidationResult {
    public bool isValid { get; set; } = false;
    public string? Message { get; set; }
}