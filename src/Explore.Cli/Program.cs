using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Explore.Cli.Models;
using Spectre.Console;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand();
        rootCommand.Name = "Explore.CLI";
        rootCommand.Description = "Simple utility CLI for importing data into SwaggerHub Explore";

        var username = new Option<string>(name: "--username", description: "Username from Swagger Inspector.") { IsRequired = true };
        username.AddAlias("-u");

        var inspectorCookie = new Option<string>(name: "--inspector-cookie", description: "A valid and active Swagger Inspector session cookie associated with provided username") { IsRequired = true };
        inspectorCookie.AddAlias("-ic");

        var exploreCookie = new Option<string>(name: "--explore-cookie", description: "A valid and active SwaggerHub Explore session cookie") { IsRequired = true };
        exploreCookie.AddAlias("-ec");

        var importInspectorCollectionsCommand = new Command("import-inspector-collections")
        {
            username,
            inspectorCookie,
            exploreCookie
        };

        importInspectorCollectionsCommand.Description = "Import Swagger Inspector collections into SwaggerHub Explore";
        rootCommand.Add(importInspectorCollectionsCommand);

        importInspectorCollectionsCommand.SetHandler(async (u, ic, ec) => 
        {
            await ImportFromInspector(u, ic, ec);
        }, username, inspectorCookie, exploreCookie);


        AnsiConsole.Write( new FigletText("Explore.Cli").Color(new Color(133, 234, 45)) );
        
        return await rootCommand.InvokeAsync(args);  
    }

    internal static async Task ImportFromInspector(string inspectorUsername, string inspectorCookie, string exploreCookie)
    {

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://inspector-api-proxy.swagger.io")
        };

        var exploreHttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.explore.swaggerhub.com")
        };


        httpClient.DefaultRequestHeaders.Add("Cookie", inspectorCookie);
        var inspectorCollectionsResponse = await httpClient.GetAsync($"/repository/v1/{inspectorUsername}/collections");

        if(inspectorCollectionsResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            AnsiConsole.MarkupLine("[red]Could not authenticate with provided inspector cookie[/]");
            return;
        }

        if(inspectorCollectionsResponse.StatusCode == HttpStatusCode.OK)
        {
            var collections = await inspectorCollectionsResponse.Content.ReadFromJsonAsync<List<InspectorCollection>>();
            var panel = new Panel($"You have [green]{collections!.Count} collections[/] in inspector");
            panel.Width = 100;
            panel.Header = new PanelHeader("Swagger Inspector Data").Centered();
            AnsiConsole.Write(panel);
            Console.WriteLine("");
            

            var counter = 0;
            foreach(var collection in collections)
            {


                if(MappingHelper.CollectionEntriesNotLimitedToSoap(collection.CollectionEntries))
                {

                    var resultTable = new Table() { Title = new TableTitle(text: $"IMPORTING [green]{collection.Name}[/] TO EXPLORE"), Width = 100, UseSafeBorder = true};
                    resultTable.AddColumn("Result");
                    resultTable.AddColumn(new TableColumn("Details").Centered());

                    //prepare request body
                    var cleanedCollectionName = UtilityHelper.CleanString(collection.Name);
                    var spaceContent = new StringContent(JsonSerializer.Serialize(new SpaceRequest() { Name = cleanedCollectionName }), Encoding.UTF8, "application/json");   

                    exploreHttpClient.DefaultRequestHeaders.Clear();
                    exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                    exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{MappingHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                    var spacesResponse = await exploreHttpClient.PostAsync("/spaces-api/v1/spaces", spaceContent);
                    //Console.WriteLine(JsonSerializer.Serialize(new SpaceRequest() { Name = collection.Name }));

                    if(spacesResponse.StatusCode == HttpStatusCode.Created)
                    {
                        var apiImportResults = new Table() { Title = new TableTitle(text: $"SPACE [green]{cleanedCollectionName}[/] CREATED"), Width = 75, UseSafeBorder = true};
                        apiImportResults.AddColumn("Result");
                        apiImportResults.AddColumn("API Imported");
                        apiImportResults.AddColumn("Connection Imported");

                        //grab the space id
                        var spaceResponse = spacesResponse.Content.ReadFromJsonAsync<SpaceResponse>();

                        if(collection.CollectionEntries != null)
                        {
                            foreach(var entry in collection.CollectionEntries)
                            {
                                //now let's create an API entry in the space
                                var cleanedAPIName = UtilityHelper.CleanString(entry.Name);
                                var apiContent = new StringContent(JsonSerializer.Serialize(new ApiRequest() { Name = cleanedAPIName, Type = "REST", Description = $"imported from inspector on {DateTime.UtcNow.ToShortDateString()}" }), Encoding.UTF8, "application/json"); 
                            
                                exploreHttpClient.DefaultRequestHeaders.Clear();
                                exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                                exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{MappingHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                                var apiResponse = await exploreHttpClient.PostAsync($"/spaces-api/v1/spaces/{spaceResponse.Result?.Id}/apis", apiContent);

                                if(apiResponse.StatusCode == HttpStatusCode.Created)
                                {

                                    var createdApiResponse = apiResponse.Content.ReadFromJsonAsync<ApiResponse>();
                                    var connectionRequestBody = JsonSerializer.Serialize(MappingHelper.MapInspectorCollectionEntryToExploreConnection(entry));
                                    var connectionContent = new StringContent(connectionRequestBody, Encoding.UTF8, "application/json");

                                    //now let's do the work and import the connection
                                    exploreHttpClient.DefaultRequestHeaders.Clear();
                                    exploreHttpClient.DefaultRequestHeaders.Add("Cookie", exploreCookie);
                                    exploreHttpClient.DefaultRequestHeaders.Add("X-Xsrf-Token", $"{MappingHelper.ExtractXSRFTokenFromCookie(exploreCookie)}");
                                    var connectionResponse = await exploreHttpClient.PostAsync($"/spaces-api/v1/spaces/{spaceResponse.Result?.Id}/apis/{createdApiResponse.Result?.Id}/connections", connectionContent);

                                    if(connectionResponse.StatusCode == HttpStatusCode.Created)
                                    {
                                        apiImportResults.AddRow("[green]OK[/]", $"API '{cleanedAPIName}' created", "Connection created");
                                    }
                                    else
                                    {
                                        apiImportResults.AddRow("[orange3]OK[/]", $"API '{cleanedAPIName}' created", "[orange3]Connection NOT created[/]");
                                    }
                                }
                                else
                                {
                                    apiImportResults.AddRow("[red]NOK[/]", $"API creation failed. StatusCode {apiResponse.StatusCode}", "");
                                }
                            }

                            resultTable.AddRow(new Markup("[green]success[/]"), apiImportResults);  
                        }
                        
                        AnsiConsole.Write(resultTable);
                        Console.WriteLine("");
                    }
                    else 
                    {
                        switch(spacesResponse.StatusCode)
                        {
                            case(HttpStatusCode.OK):
                                // not expecting a 200 OK here - this would be returned for a failed auth and a redirect to SB ID
                                resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] Auth failed connecting to SwaggerHub Explore. Please review provided cookie.[/]"));  
                                AnsiConsole.Write(resultTable);
                                Console.WriteLine("");   
                                break;                          
                            
                            case(HttpStatusCode.Conflict):
                                var apiImportResults = new Table() { Title = new TableTitle(text: $"[orange3]SPACE[/] {cleanedCollectionName} [orange3]ALREADY EXISTS[/]")};
                                apiImportResults.AddColumn("Result");
                                apiImportResults.AddColumn("API Imported");
                                apiImportResults.AddColumn("Connection Imported");
                                apiImportResults.AddRow("skipped","","");

                                resultTable.AddRow(new Markup("[orange3]skipped[/]"), apiImportResults);      
                                AnsiConsole.Write(resultTable);  
                                Console.WriteLine("");
                                break;

                            default:
                                resultTable.AddRow(new Markup("[red]failure[/]"), new Markup($"[red] StatusCode: {spacesResponse.StatusCode}, Details: {spacesResponse.ReasonPhrase}[/]"));  
                                AnsiConsole.Write(resultTable);
                                Console.WriteLine("");
                                break;                                             
                        }        
                    }
                }
                else
                {
                    var resultTable = new Table() { Title = new TableTitle(text: $"IMPORTING [green]{collection.Name}[/] TO EXPLORE"), Width = 100, UseSafeBorder = true};
                    resultTable.AddColumn("Result");
                    resultTable.AddColumn(new TableColumn("Details").Centered());
                    resultTable.AddRow(new Markup("[orange3]skipped[/]"), new Markup("[orange3]No transactions to import[/]"));  
                    AnsiConsole.Write(resultTable);
                    Console.WriteLine("");
                }

                counter++;
                System.Threading.Thread.Sleep(200);
            }

            Console.WriteLine("");
            AnsiConsole.MarkupLine("[green] All done! We're finished importing collections[/]");
        }
        else 
        {
            AnsiConsole.WriteLine($"[red]StatusCode {inspectorCollectionsResponse.StatusCode} returned from the Inspector API[/]");
        }
    }
}









