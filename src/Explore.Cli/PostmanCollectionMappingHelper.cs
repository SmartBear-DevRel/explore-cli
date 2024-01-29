using Explore.Cli.Models;

public static class PostmanCollectionMappingHelper
{
       public static Connection MapPostmanCollectionItemToExploreConnection(Item postmanCollectionItem)
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
                        Url = GetServerUrlFromItemRequest(postmanCollectionItem.Request)
                    }
                },
                Paths = CreatePathsDictionary(postmanCollectionItem.Request),
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

   public static string GetServerUrlFromItemRequest(Request? request)
   {
        if(request == null || request.Url == null || string.IsNullOrEmpty(request.Url.Raw))
        {
            return string.Empty;
        }
        
        var host = string.Join(".", request.Url?.Host ?? Enumerable.Empty<string>());
        var serverUrl = $"{request.Url?.Protocol}://{host}";
        
        if(!string.IsNullOrEmpty(request.Url?.Port))
        {
            serverUrl += $":{request.Url.Port}";
        }

        return serverUrl;
   }

    public static List<Parameter> MapHeaderAndQueryParams(Request? request)
    {
        List<Parameter> parameters = new List<Parameter>();

        if(request?.Header != null && request.Header.Any())
        {
            // map the headers
            foreach(var hdr in request.Header)
            {
                parameters.Add(new Parameter()
                {
                    In = "header",
                    Name = hdr.Key,
                    Examples = new Examples()
                    {
                        Example = new Example()
                        {
                            Value = hdr.Value
                        }
                    }
                });
            }
        }

        // parse and map the query string
        if(request?.Url != null)
        {
            if(request.Url.Query != null)
            {
                foreach(var param in request.Url.Query)
                {
                    parameters.Add(new Parameter()
                    {
                        In = "query",
                        Name = param.Key,
                        Examples = new Examples()
                        {
                            Example = new Example()
                            {
                                Value = param.Value
                            }
                        }
                    });
                }
            }
        }

        return parameters;
    }   

    public static Dictionary<string, object> CreatePathsDictionary(Request? request)
    {        

        if(request?.Url != null && request.Url.Path != null)
        {
            var pathsContent = new PathsContent()
            {
                Parameters = MapHeaderAndQueryParams(request)
            };

            //add request body
            if(request.Body != null && request.Body.Raw != null)
            {
                var examplesJson = new Dictionary<string, object>
                {
                    { "examples", MapEntryBodyToContentExamples(request.Body.Raw) }
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
                    { request.Method.ToLowerInvariant(), pathsContent }
                };

                var json = new Dictionary<string, object>
                {
                    { $"/{string.Join("/", request.Url.Path)}", methodJson }
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
}