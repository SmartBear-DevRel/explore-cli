using System.Net.Http.Headers;

namespace Explore.Cli.Tests;

public class UtilityHelperTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("Frank Test", "Frank Test")]
    [InlineData("Frank's APIs", "Franks APIs")]
    [InlineData("Frank_s_APIs", "Frank_s_APIs")]
    [InlineData("Franks-APIs", "Franks-APIs")]
    [InlineData("AbC|@123", "AbC123")]
    [InlineData("**Collection**", "Collection")]
    public void CleanStrings_IllegalCharsShouldBeRemoved(string input, string expected)
    {
        var actual = UtilityHelper.CleanString(input);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void SchemaValidation_ExploreSpaces_Invalid_Should_Fail()
    {
        string entryAsJsonString = @"{""info"": {""version"": ""0.0.1"", ""exportedAt"": ""10:17:50 AM"" }}";

        string expectedError = "1 total errors";        
        var validationResult = await UtilityHelper.ValidateSchema(entryAsJsonString, "explore");

        Assert.False(validationResult.isValid);
        Assert.Contains(expectedError, validationResult.Message);
    }

    [Fact]
    public void IsContentTypeExpected_Should_Pass()
    {
        HttpResponseMessage message = new HttpResponseMessage();
        message.Content.Headers.TryAddWithoutValidation("Content-Type", new MediaTypeHeaderValue("application/json").MediaType);
        var count = message.Headers.Count();
        

        var actual = UtilityHelper.IsContentTypeExpected(message.Content.Headers, "application/json");

        Assert.True(actual);
    }

    [Fact]
    public void IsContentTypeExpected_Should_Fail()
    {
        HttpResponseMessage message = new HttpResponseMessage();
        message.Content.Headers.TryAddWithoutValidation("Content-Type", new MediaTypeHeaderValue("application/json").MediaType);
        var count = message.Headers.Count();
        

        var actual = UtilityHelper.IsContentTypeExpected(message.Content.Headers, "text/html");

        Assert.False(actual);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("test/")]
    [InlineData("test.")]
    [InlineData("test.txt")]
    public void IsValidFileName_Should_Fail(string input)
    {
        Assert.False(UtilityHelper.IsValidFileName(ref input));
    }

    [Theory]
    [InlineData("test", "test.json")]
    [InlineData("test-test", "test-test.json")]
    [InlineData("test_test", "test_test.json")]
    [InlineData("test.json", "test.json")]
    [InlineData("test.JSON", "test.JSON")]
    public void IsValidFileName_Should_Pass(string input, string expected)
    {
        Assert.True(UtilityHelper.IsValidFileName(ref input));
        Assert.Equal(input, expected);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValidFilePath_Should_Fail_With_Invalid_Chars(string input)
    {
        Assert.False(UtilityHelper.IsValidFilePath(ref input));
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test/test")]
    [InlineData("test\\test")]
    [InlineData("test.test")]
    public void IsValidFilePath_Should_Pass(string input)
    {
        Assert.True(UtilityHelper.IsValidFilePath(ref input));
    }

    [Theory]
    [InlineData("XSRF-TOKEN=be05885a-41fc-4820-83fb-5db17015ed4a", "be05885a-41fc-4820-83fb-5db17015ed4a")]
    [InlineData("xsrf-token=dd3424c9-17ec-4b20-a89c-ca89d98bbd3b", "dd3424c9-17ec-4b20-a89c-ca89d98bbd3b")]
    [InlineData("Xsrf-Token=dd3424c9-17ec-4b20-a89c-ca89d98bbd3b", "dd3424c9-17ec-4b20-a89c-ca89d98bbd3b")]
    [InlineData("bf936dc3-6c70-43a0-a4c5-ddb42569a9c8", null)]
    public void ExtractXSRFTokenFromCookie_Tests(string cookie, string expected)
    {
        var actual = UtilityHelper.ExtractXSRFTokenFromCookie(cookie);
        Assert.Equal(expected, actual);
    }

}