// using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Newtonsoft.Json.Schema;
// using Explore.Cli.Models;
// using Namotion.Reflection;

public static class PactMappingHelper
{

    public static bool hasPactVersion(string json)
    {
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (jsonObject != null && jsonObject.ContainsKey("metadata"))
        {
            var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject["metadata"].ToString() ?? string.Empty);
            if (metadata != null && metadata.ContainsKey("pactSpecification"))
            {
                var pactSpecification = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata["pactSpecification"].ToString() ?? string.Empty);
                if (pactSpecification != null && pactSpecification["version"] != null && pactSpecification["version"].ToString() != null)
                {
                    var version = pactSpecification["version"].ToString();
                    var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0" };
                    if (version != null && validVersions.Contains(version))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

        public static string getPactVersion(string json)
        {
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (jsonObject != null && jsonObject.ContainsKey("metadata"))
            {
                var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject["metadata"].ToString() ?? string.Empty);
                if (metadata != null && metadata.ContainsKey("pactSpecification"))
                {
                    var pactSpecification = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata["pactSpecification"].ToString() ?? string.Empty);
                    if (pactSpecification != null && pactSpecification["version"] != null && pactSpecification["version"].ToString() != null)
                    {
                        var version = pactSpecification["version"].ToString();
                        var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0" };
                        if (version != null && validVersions.Contains(version))
                        {
                            return $"pact-v{version}";
                        }
                    }
                }
            }

            return string.Empty;
        }

 
}