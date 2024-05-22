// using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Newtonsoft.Json.Schema;
using Explore.Cli.Models;
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
                    var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0", "4.0" };
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
                    var validVersions = new List<string> { "1.0.0", "2.0.0", "3.0.0", "4.0.0", "4.0" };
                    if (version != null && validVersions.Contains(version))
                    {
                        var formattedVersion = version.Split('.')[0];
                        return $"pact-v{formattedVersion}";
                    }
                }
            }
        }

        return string.Empty;
    }


   public static Connection MapPactInteractionToExploreConnection(PactV3.Interaction pactInteraction)
   {
        return new Connection()
        {
            Type = "ConnectionRequest",
            Name = "REST",
            Schema = "OpenAPI",
            SchemaVersion = "3.0.1",
            ConnectionDefinition = new ConnectionDefinition()
            {
                Servers = new List<Server>()
                {
                    new Server()
                    {
                        Url = ""
                    }
                },
                Paths = CreatePathsDictionary(pactInteraction.Request),
            },
            Settings = new Settings()
            {
                Type = "RestConnectionSettings",
                ConnectTimeout = 30,
                FollowRedirects = true,
                EncodeUrl = true
            },
        };
   }


    public static Dictionary<string, object> CreatePathsDictionary(PactV3.Request? request)
    {        

        if(request?.Path != null)
        {
            var pathsContent = new PathsContent()
            {
                Parameters = MapHeaderAndQueryParams(request)
            };

            // //add request body
            if(request.Body != null)
            {
                var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(request.Body.ToString()) }
                };

                var contentJson = new Dictionary<string, object>
                {
                    { "*/*", examplesJson }
                };

                pathsContent.RequestBody = new RequestBody()
                {
                    Content = contentJson
                };
            }


            // add header and query params
            if(request.Method != null)
            {
                var methodJson = new Dictionary<string, object>
                {
                    { request.Method.ToString().Replace("Method", string.Empty), pathsContent }
                };

                var json = new Dictionary<string, object>
                {
                    { request.Path, methodJson }
                };

                return json;                
            }
        }

        return new Dictionary<string, object>();
    }

    public static Examples MapEntryBodyToContentExamples(string? rawBody)
    {
        return new Examples()
        {
            Example = new Example()
            {
                Value = rawBody
            }
        };        
    }

    public static List<Parameter> MapHeaderAndQueryParams(PactV3.Request? request)
    {
        List<Parameter> parameters = new List<Parameter>();

        if(request?.Headers != null && request.Headers.Any())
        {
            // map the headers
            foreach(var hdr in request.Headers)
            {
                parameters.Add(new Parameter()
                {
                    In = "header",
                    Name = hdr.Key,
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = hdr.Value.ToString()
                        }
                    }
                });
            }
        }

        // if we have urlencoded body then force the content type header as plaintext (Explore doesn't support urlencoded natively)
        // if(request?.Body != null && request.Body.Mode != null && request.Body.Mode.Equals("urlencoded", StringComparison.OrdinalIgnoreCase))
        // {
        //     parameters.Add(new Parameter()
        //     {
        //         In = "header",
        //         Name = "Content-Type",
        //         Schema = new Schema()
        //         {
        //             type = "string"
        //         },
        //         Examples = new Examples()
        //         {
        //             Example = new Example()
        //             {
        //                 Value = "application/x-www-form-urlencoded"
        //             }
        //         }
        //     });
        // }

        // parse and map the query string
        if(request?.Query != null)
        {
            if(request.Query != null)
            {
                foreach(var param in request.Query)
                {
                    parameters.Add(new Parameter()
                    {
                        In = "query",
                        Name = param.Key,
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = param.Value.ToString()
                            }
                        }
                    });
                }
            }
        }

        return parameters;
    }   

}

