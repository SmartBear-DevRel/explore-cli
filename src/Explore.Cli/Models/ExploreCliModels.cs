using Explore.Cli.Models.Explore;

namespace Explore.Cli.Models;


public partial class SchemaValidationResult {
    public bool isValid { get; set; } = false;
    public string? Message { get; set; }
}

public partial class StagedAPI
{
    public string APIName { get; set; } = string.Empty;
    public string APIUrl { get; set; } = string.Empty;
    public List<Connection> Connections { get; set; } = new List<Connection>();
    public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
}