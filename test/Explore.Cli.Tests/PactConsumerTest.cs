using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Explore.Cli.Models.Explore;
using Explore.Cli.ExploreHttpClient;
using PactNet;
using PactNet.Output.Xunit;
using Xunit.Abstractions;
using static Explore.Cli.ExploreHttpClient.ExploreHttpClient;
using System.Text.Json.Nodes;

namespace Consumer.Tests
{
    public class ExploreCliTests
    {
        private readonly IPactBuilderV4 pact;

        public ExploreCliTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                PactDir = "../../../pacts/",
                Outputters = new[]
                {
                    new XunitOutput(output)
                },
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                LogLevel = PactLogLevel.Debug
            };

            this.pact = Pact.V4("explore-cli", "api-explore-connectionservice", config).WithHttpInteractions();
        }

        [Fact]
        public async Task ChecksExploreSpaceExists()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var spaceName = "new space";
            var spaceContent = new StringContent(JsonSerializer.Serialize(new SpaceRequest() { Name = spaceName }), Encoding.UTF8, "application/json");
            var spaceId = new Guid();
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to check if a space exists")
                    .Given("a space with name {name} does not exist", new Dictionary<string, string> { ["name"] = spaceName })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithHeader("Content-Type", "application/hal+json")
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var response = await client.CheckSpaceExists(exploreCookie, spaceId.ToString(), false);

                Assert.True(response);
            });
        }
        [Fact]
        public async Task CreateSpace()
        {
            var expectedStatusCode = HttpStatusCode.Created;
            var expectedId = new Guid();
            var spaceName = "new space";
            var spaceContent = new SpaceRequest() { Name = spaceName };
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to create a new space")
                    .Given("a space with name {name} does not exist", new Dictionary<string, string> { ["name"] = spaceName })
                    .WithRequest(HttpMethod.Post, "/spaces")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                    .WithJsonBody(spaceContent)
                .WillRespond()
                    .WithStatus(expectedStatusCode)
                    .WithJsonBody(new SpaceResponse()
                    {
                        Id = expectedId,
                        Name = spaceName,
                        Links = new Links
                        {
                            Self = new()
                            {
                                Href = new Uri("http://test"),
                            }
                        }
                    });

            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CreateSpace(exploreCookie, spaceName);

                Assert.Equal(expectedId, spaceResponse.Id);
            });
        }
        [Fact]
        public async Task HandlesUnknownFailuresWhenCreatingANewExploreSpace()
        {
            var expectedStatusCode = HttpStatusCode.Conflict;
            var spaceName = "new space";
            var spaceContent = new SpaceRequest() { Name = spaceName };
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to create a new space")
                    .Given("the request will not be processed")
                    .WithRequest(HttpMethod.Post, "/spaces")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                    .WithJsonBody(spaceContent)
                .WillRespond()
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CreateSpace(exploreCookie, spaceName);

                Assert.Equal(expectedStatusCode, spaceResponse.StatusCode);
            });
        }

        [Fact]
        public async Task CreateApiEntry()
        {
            var expectedStatusCode = HttpStatusCode.Created;
            var expectedId = new Guid();
            var spaceId = new Guid();
            var apiName = "new space";
            var importer = "xunit";
            var description = "sample desc";
            var apiContent = new ApiRequest()
            {
                Name = apiName,
                Type = "REST",
                Description = $"imported from {importer} on {DateTime.UtcNow.ToShortDateString()}\n{description}"
            };
            var expectedResponse = new CreateApiEntryResult
            {
                Result = true,
                Id = new Guid(),
                StatusCode = expectedStatusCode
            };
            var expectedApiResponse = new ApiResponse
            {
                Id = new Guid(),
                Name = "foo",
                Type = "TEST",
                Servers = new List<Server>
                {

                },
                Description = "foo"
            };

            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to create a new api entry")
                    .Given("a space {spaceId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString() })
                    .WithRequest(HttpMethod.Post, $"/spaces/{spaceId}/apis")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                    .WithJsonBody(apiContent)
                .WillRespond()
                    .WithStatus(expectedStatusCode)
                    .WithJsonBody(expectedApiResponse);

            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CreateApiEntry(exploreCookie, spaceId, apiName, importer, description);

                Assert.Equal(expectedId, spaceResponse.Id);
            });
        }

        [Fact]
        public async Task CreateApiConnection()
        {
            var expectedStatusCode = HttpStatusCode.Created;
            var spaceId = new Guid();
            var apiId = new Guid();
            var apiConnectionContent = new JsonObject
            {

            };
            var expectedResponse = new CreateApiConnectionResult
            {
                Result = true,
                Id = new Guid(),
                StatusCode = expectedStatusCode
            };

            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to create a new api connection")
                    .Given("a space {spaceId} with api {apiId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString() })
                    .WithRequest(HttpMethod.Post, $"/spaces/{spaceId}/apis/{apiId}/connections")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                    .WithJsonBody(apiConnectionContent)
                .WillRespond()
                    .WithStatus(expectedStatusCode);

            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CreateApiConnection(exploreCookie, spaceId, apiId, apiConnectionContent.ToString());

                Assert.True(spaceResponse.Result);
            });
        }

        [Fact]
        public async Task CheckApiExists()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var spaceId = new Guid();
            var apiId = new Guid();
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to check an api exists")
                    .Given("a space {spaceId} with api {apiId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis/{apiId}")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CheckApiExists(exploreCookie, spaceId.ToString(), apiId.ToString(), false);

                Assert.True(spaceResponse);
            });
        }
        [Fact]
        public async Task CheckApiDoesNotExist()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var spaceId = new Guid();
            var apiId = new Guid();
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to check an api exists")
                    .Given("a space {spaceId} with api {apiId} does not exist", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis/{apiId}")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CheckApiExists(exploreCookie, spaceId.ToString(), apiId.ToString(), false);

                Assert.False(spaceResponse);
            });
        }

        [Fact]
        public async Task CheckConnectionExists()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var spaceId = new Guid();
            var apiId = new Guid();
            var connectionId = new Guid();
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to check an api connection exists")
                    .Given("a space {spaceId} with api {apiId} and connection {connectionId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString(), ["connectionId"] = connectionId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis/{apiId}/connections/{connectionId}")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CheckConnectionExists(exploreCookie, spaceId.ToString(), apiId.ToString(), connectionId.ToString(), false);

                Assert.True(spaceResponse);
            });
        }

        [Fact]
        public async Task CheckConnectionDoesNotExist()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var spaceId = new Guid();
            var apiId = new Guid();
            var connectionId = new Guid();
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            pact
                .UponReceiving("a request to check an api connection exists")
                    .Given("a space {spaceId} with api {apiId} and connection {connectionId} does not exist", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString(), ["connectionId"] = connectionId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis/{apiId}/connections/{connectionId}")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithStatus(expectedStatusCode);


            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CheckConnectionExists(exploreCookie, spaceId.ToString(), apiId.ToString(), connectionId.ToString(), false);

                Assert.False(spaceResponse);
            });
        }

        [Fact]
        public async Task GetSpaces()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";
            var expectedResult = new GetSpacesResult
            {
                Result = true,
                Spaces = new PagedSpaces
                {
                    Embedded = new EmbeddedSpaces
                    {
                        Spaces = new List<SpaceResponse>{
                            new() {
                                Id = new Guid(),
                                Name = "foo",
                                Links = new Links
                                {
                                    Self = new()
                                    {
                                        Href = new Uri("http://test"),
                                    }
                                }
                        },
                            new() {
                                Id = new Guid(),
                                Name = "foo2",
                                Links = new Links
                                {
                                    Self = new()
                                    {
                                        Href = new Uri("http://test"),
                                    }
                                }
                        }
                       }
                    }
                },
                StatusCode = expectedStatusCode
            };
            pact
                .UponReceiving("a request to get spaces")
                    .Given("some spaces exist")
                    .WithRequest(HttpMethod.Get, $"/spaces")
                    .WithQuery("page", "0")
                    .WithQuery("size", "2000")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithHeader("Content-Type", "application/hal+json")
                    .WithStatus(expectedStatusCode)
                    .WithJsonBody(expectedResult.Spaces);



            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var response = await client.GetSpaces(exploreCookie);
                Assert.Equal(expectedResult.Result, response.Result);
                Assert.Equal(expectedResult.StatusCode, response.StatusCode);
                Assert.Equal(expectedResult.Spaces.Embedded.Spaces[0].Id, response?.Spaces?.Embedded?.Spaces?[0].Id);
                Assert.Equal(expectedResult.Spaces.Embedded.Spaces[0].Name, response?.Spaces?.Embedded?.Spaces?[0].Name);
                Assert.Equal(expectedResult.Spaces.Embedded.Spaces[0].Links?.Self?.Href, response?.Spaces?.Embedded?.Spaces?[0].Links?.Self?.Href);
            });
        }

        [Fact]
        public async Task GetSpaceApis()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";

            var spaceId = new Guid();
            var apiId = new Guid();
            var expectedResult = new GetSpaceApisResult
            {
                Result = true,
                Apis = new PagedApis
                {
                    Embedded = new EmbeddedApis
                    {
                        Apis = new List<ApiResponse>{
                            new() {
                                Id = new Guid(),
                                Name = "foo",
                                Type = "TEST",
                                Servers = new List<Server>{

                                },
                                Description = "foo"
                        }
                       }
                    }
                },
                StatusCode = expectedStatusCode
            };
            pact
                .UponReceiving("a request to get apis for a space")
                    .Given("a space {spaceId} with api {apiId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis")
                    .WithQuery("page", "0")
                    .WithQuery("size", "2000")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithHeader("Content-Type", "application/hal+json")
                    .WithStatus(expectedStatusCode)
                    .WithJsonBody(expectedResult.Apis);



            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var response = await client.GetSpaceApis(exploreCookie, spaceId);
                Assert.Equal(expectedResult.Result, response.Result);
                Assert.Equal(expectedResult.StatusCode, response.StatusCode);
                Assert.Equal(expectedResult.Apis.Embedded.Apis[0].Id, response?.Apis?.Embedded?.Apis?[0].Id);
                Assert.Equal(expectedResult.Apis.Embedded.Apis[0].Name, response?.Apis?.Embedded?.Apis?[0].Name);
            });
        }
        [Fact]
        public async Task GetApiConnectionsForSpace()
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var exploreXsrfToken = "bar";
            var exploreCookie = $"foo;XSRF-TOKEN={exploreXsrfToken}";

            var spaceId = new Guid();
            var apiId = new Guid();
            var connectionId = new Guid();
            var expectedResult = new GetApiConnectionsForSpaceResult
            {
                Result = true,
                Connections = new PagedConnections
                {
                    Embedded = new EmbeddedConnections
                    {
                        Connections = new List<Connection>{
                            // we get a fair bit back in the response that isn't shown in the 
                            // openapi document, so adding them in fails BDCT verificatiom
                            // excluding properties and making them nullable resolves cross-compat checks
                            new() {
                                Id = new Guid().ToString(),
                                Name = "REST",
                                Schema = "OpenAPI",
                                SchemaVersion = "3.0.1",
                                // Type = "foo",
                                // ConnectionDefinition = new ConnectionDefinition{
                                //     Paths = new Dictionary<string, object>{{"/me",new object{}}},
                                //     Servers = new List<Server>{
                                //             new() {
                                //                 Url = "http://test"
                                //             }

                                //     }
                                // },
                                ConnectionDefinition = new ConnectionDefinition{
                                },
                                // Settings = new Settings{
                                //     Type = "RestConnectionSettings",
                                //     EncodeUrl = true,
                                //     ConnectTimeout = 30,
                                //     FollowRedirects = true,
                                // },
                                // Credentials = new Credentials{

                                // },
                        }
                       }
                    }
                },
                StatusCode = expectedStatusCode
            };
            pact
                .UponReceiving("a request to get connections for an api")
                    .Given("a space {spaceId} with api {apiId} and connection {connectionId} exists", new Dictionary<string, string> { ["spaceId"] = spaceId.ToString(), ["apiId"] = apiId.ToString(), ["connectionId"] = connectionId.ToString() })
                    .WithRequest(HttpMethod.Get, $"/spaces/{spaceId}/apis/{apiId}/connections")
                    .WithQuery("page", "0")
                    .WithQuery("size", "2000")
                    .WithHeader("Cookie", exploreCookie)
                    .WithHeader("X-Xsrf-Token", exploreXsrfToken)
                .WillRespond()
                    .WithHeader("Content-Type", "application/hal+json")
                    .WithStatus(expectedStatusCode)
                    .WithJsonBody(expectedResult.Connections);



            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var response = await client.GetApiConnectionsForSpace(exploreCookie, spaceId, apiId);
                Assert.Equal(expectedResult.Result, response.Result);
                Assert.Equal(expectedResult.StatusCode, response.StatusCode);
                Assert.Equal(expectedResult.Connections.Embedded.Connections[0].Id, response?.Connections?.Embedded?.Connections?[0].Id);
                Assert.Equal(expectedResult.Connections.Embedded.Connections[0].Name, response?.Connections?.Embedded?.Connections?[0].Name);
            });
        }
    }
}