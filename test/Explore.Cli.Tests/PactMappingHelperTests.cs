using Explore.Cli.Models;
using System.Text.Json;

public class PactMappingHelperTests
{
    [Fact]
    public void hasPactVersion()
    {
        // Arrange
        
        var mockPactContractV3 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";

        // Act
        
        var result = PactMappingHelper.hasPactVersion(mockPactContractV3);
        
        // Assert

        Assert.True(result);
    }    
    [Fact]
    public void getPactVersion()
    {
        // Arrange
        
        var mockPactContractV3 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";
        string expectedResult = "pact-v3";        

        // Act
        
        var result = PactMappingHelper.getPactVersion(mockPactContractV3);
        
        // Assert

        Assert.Contains(expectedResult, result);
    }    
    [Fact]
    public async Task IsNotMatchingPactV1Schema()
    {
        // Arrange
        
        var mockPactContractV1 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";
        string expectedError = "3 total errors";        

        // Act
        
        var validationResult = await UtilityHelper.ValidateSchema(mockPactContractV1, "pact-v1");
        
        // Assert
        Assert.False(validationResult.isValid);
        Assert.Contains(expectedError, validationResult.Message);
    }    
    [Fact]
    public async Task IsNotMatchingPactV2Schema()
    {
        // Arrange
        
        var mockPactContractV2 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";
        string expectedError = "3 total errors";        

        // Act
        
        var validationResult = await UtilityHelper.ValidateSchema(mockPactContractV2, "pact-v2");
        
        // Assert
        Assert.False(validationResult.isValid);
        Assert.Contains(expectedError, validationResult.Message);
    }    
    [Fact]
    public async Task IsMatchingPactV3Schema()
    {
        // Arrange
        
        var mockPactContractV3 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";

        // Act
        
        var validationResult = await UtilityHelper.ValidateSchema(mockPactContractV3, "pact-v3");
        
        // Assert
        Assert.True(validationResult.isValid);
    }    
    [Fact]
    public async Task IsNotMatchingPactV4Schema()
    {
        // Arrange
        
        var mockPactContractV4 = "{\"consumer\":{\"name\":\"swaggerhub-pactflow-consumer-codegen\"},\"interactions\":[{\"description\":\"a request to get a product\",\"providerStates\":[{\"name\":\"a product with ID 10 exists\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/product\\/10\"},\"response\":{\"body\":{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request for to create product 1234\",\"providerStates\":[{\"name\":\"a product with id 1234 does not exist\"}],\"request\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}}},\"method\":\"POST\",\"path\":\"\\/products\"},\"response\":{\"body\":{\"id\":\"1234\",\"name\":\"burger\",\"price\":42,\"type\":\"food\"},\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}},{\"description\":\"a request to get all products\",\"providerStates\":[{\"name\":\"products exist\"}],\"request\":{\"method\":\"GET\",\"path\":\"\\/products\"},\"response\":{\"body\":[{\"id\":\"10\",\"name\":\"28 Degrees\",\"type\":\"CREDIT_CARD\"}],\"headers\":{\"Content-Type\":\"application\\/json\"},\"matchingRules\":{\"body\":{\"$\":{\"combine\":\"AND\",\"matchers\":[{\"match\":\"type\"}]}},\"header\":{}},\"status\":200}}],\"metadata\":{\"pact-js\":{\"version\":\"10.1.4\"},\"pactRust\":{\"ffi\":\"0.3.12\",\"models\":\"0.4.5\"},\"pactSpecification\":{\"version\":\"3.0.0\"}},\"provider\":{\"name\":\"swaggerhub-pactflow-provider\"}}";
        string expectedError = "3 total errors";        

        // Act
        
        var validationResult = await UtilityHelper.ValidateSchema(mockPactContractV4, "pact-v4");
        
        // Assert
        Assert.False(validationResult.isValid);
        Assert.Contains(expectedError, validationResult.Message);

    }    
}