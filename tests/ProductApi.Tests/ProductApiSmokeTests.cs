using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProductApi.Application.DTOs;

namespace ProductApi.Tests;

public class ProductApiSmokeTests : IClassFixture<ProductApiFactory>
{
    private readonly HttpClient _client;

    public ProductApiSmokeTests(ProductApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostGetPutDelete_ProductCrudFlow_ShouldSucceed()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", new CreateProductRequest
        {
            ProductName = "Laptop",
            CreatedBy = "integration-test",
            Items =
            [
                new CreateItemRequest { Quantity = 2 }
            ]
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v1/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/products/{created.Id}", new UpdateProductRequest
        {
            ProductName = "Laptop Pro",
            ModifiedBy = "integration-test",
            Items =
            [
                new UpdateItemRequest { Quantity = 4 }
            ]
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/products/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
