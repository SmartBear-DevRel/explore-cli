using System.Text.Json;
using Explore.Cli.Models.Explore;
using Explore.Cli.Models.Postman;
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

        // if we have urlencoded body then force the content type header as plaintext (Explore doesn't support urlencoded natively)
        if(request?.Body != null && request.Body.Mode != null && request.Body.Mode.Equals("urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Add(new Parameter()
            {
                In = "header",
                Name = "Content-Type",
                Schema = new Schema()
                {
                    type = "string"
                },
                Examples = new Examples()
                {
                    Example = new Example()
                    {
                        Value = "application/x-www-form-urlencoded"
                    }
                }
            });
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
            if(request.Body != null)
            {
                if(request.Body.Raw != null)
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
                else if(request.Body.Urlencoded != null)
                {
                    var examplesJson = new Dictionary<string, object>
                    {
                        { "examples", MapUrlEncodedBodyToContentExamples(request.Body.Urlencoded) }
                    };

                    var contentJson = new Dictionary<string, object>
                    {
                        { "application/x-www-form-urlencoded", examplesJson }
                    };

                    pathsContent.RequestBody = new RequestBody()
                    {
                        Content = contentJson
                    };
                }
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

    public static Examples MapUrlEncodedBodyToContentExamples(List<Urlencoded>? urlEncodedBody)
    {
        var rawBody = string.Empty;

        if(urlEncodedBody != null)
        {
            foreach(var param in urlEncodedBody)
            {
                rawBody += $"{param.Key}={param.Value}&";
            }
        }

        return new Examples()
        {
            Example = new Example()
            {
                Value = rawBody
            }
        };        
    }
    
    public static List<Item> FlattenItems(List<Item> items)
    {
        var result = new List<Item>();

        foreach (var item in items)
        {
            // Add the current item to the list if it has request data
            if(item.Request != null)
            {
                result.Add(item);
            }            

            // If the item has nested items, flatten each one and add it to the list
            if (item.ItemList != null)
            {
                result.AddRange(FlattenItems(item.ItemList));
            }
        }

        return result;
    }

    public static bool IsItemRequestModeSupported(Request request)
    {
        if(request.Body != null && request.Body.Mode != null && request.Url != null && request.Url.Protocol != null)
        {
            // if the request body mode is not raw or urlencoded and the protocol is not http or https, return false
            if(!(request.Body.Mode.Equals("raw", StringComparison.OrdinalIgnoreCase) || request.Body.Mode.Equals("urlencoded", StringComparison.OrdinalIgnoreCase) || request.Body.Mode.Equals("formdata", StringComparison.OrdinalIgnoreCase)) && request.Url.Protocol.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }

    public static bool IsCollectionVersion2_1(string json)
    {
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if(jsonObject != null && jsonObject.ContainsKey("info"))
        {
            
            var info = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject["info"].ToString() ?? string.Empty);
            

            if(info != null && info.ContainsKey("schema"))
            {
                if(info["schema"] != null && info["schema"].ToString() != null)
                {
                    var schema = info["schema"].ToString();
                    if(string.Equals(schema, "https://schema.getpostman.com/json/collection/v2.1.0/collection.json", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                
            }
        }

        return false;
    }

    public static List<Connection> MapPostmanCollectionItemsToExploreConnections(Item collectionItem)
    {
        var connections = new List<Connection>();

        if(collectionItem.Request != null && IsItemRequestModeSupported(collectionItem.Request))
        {
            connections.Add(MapPostmanCollectionItemToExploreConnection(collectionItem));
        }

        // if nested item exists, then add it to the connections list if it has request data
        if (collectionItem.ItemList != null)
        {
            foreach(var item in collectionItem.ItemList)
            {
                if(item.Request != null && IsItemRequestModeSupported(item.Request))
                {
                    connections.Add(MapPostmanCollectionItemToExploreConnection(item));
                }
            }
        }

        return connections;
    }

    public static List<StagedAPI> MapPostmanCollectionToStagedAPI(PostmanCollection postmanCollection, string rootName)
    {
        var stagedAPIs = new List<StagedAPI>();

        stagedAPIs.Add(new StagedAPI()
        {
            APIName = rootName,
        });


        if(postmanCollection.Item != null)
        {
            foreach(var item in postmanCollection.Item)
            {

                if(item.Request != null && IsItemRequestModeSupported(item.Request))
                {
                    StagedAPI api = new StagedAPI()
                    {
                        APIName = item.Name ?? string.Empty,
                        APIUrl = GetServerUrlFromItemRequest(item.Request),
                        Connections = MapPostmanCollectionItemsToExploreConnections(item)
                    };

                    //if an API with same name already exists, add the connection to the existing API
                    var existingAPI = stagedAPIs.FirstOrDefault(x => x.APIName == rootName);
                    if(existingAPI != null)
                    {
                        existingAPI.Connections.AddRange(api.Connections);
                    }
                    else
                    {
                        stagedAPIs.Add(new StagedAPI()
                        {
                            APIUrl = api.APIUrl,
                            APIName = api.APIName,
                            Connections = api.Connections
                        });
                    }
                }

                // if nested item exists, then add it to the staged API list
                if (item.ItemList != null)
                {
                    PostmanCollection tempCollection = new PostmanCollection()
                    {
                        Item = item.ItemList,
                        Info = postmanCollection.Info
                    };

                    if (tempCollection.Info != null)
                    {
                        tempCollection.Info.Name = item.Name;
                    }

                    stagedAPIs.AddRange(MapPostmanCollectionToStagedAPI(tempCollection, $"{rootName} - {item.Name}"));
                }
            }
        }

        return stagedAPIs;
    }
}