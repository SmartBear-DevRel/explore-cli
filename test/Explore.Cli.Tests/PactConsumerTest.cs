using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Explore.Cli.Models.Explore;
using Explore.Cli.ExploreHttpClient;
using PactNet;
using PactNet.Output.Xunit;
using Xunit.Abstractions;

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
        public async Task CreatesANewExploreSpace()
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
                        Links = new Links{
                            Self = new(){
                            Href = new Uri("http://test"),
                            }} 
                        });

            await pact.VerifyAsync(async ctx =>
            {
                var client = new ExploreHttpClient(ctx.MockServerUri.ToString());

                var spaceResponse = await client.CreateSpace(exploreCookie,spaceName);

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

                var spaceResponse = await client.CreateSpace(exploreCookie,spaceName);

                Assert.Equal(expectedStatusCode, spaceResponse.StatusCode);
            });
        }
    }
}