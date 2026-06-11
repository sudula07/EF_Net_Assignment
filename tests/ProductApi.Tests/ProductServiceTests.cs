using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ProductApi.Application.Common;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;

namespace ProductApi.Tests;

public class ProductServiceTests
{
    private const int NotFoundStatusCode = 404;

    [Fact]
    public async Task CreateProductAsync_ShouldCreateProductWithItems()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository, NullLogger<ProductService>.Instance);

        var request = new CreateProductRequest
        {
            ProductName = "Keyboard",
            CreatedBy = "tester",
            Items =
            [
                new CreateItemRequest { Quantity = 2 },
                new CreateItemRequest { Quantity = 3 }
            ]
        };

        var result = await service.CreateProductAsync(request, CancellationToken.None);

        result.Id.Should().BeGreaterThan(0);
        result.ProductName.Should().Be("Keyboard");
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowWhenProductDoesNotExist()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository, NullLogger<ProductService>.Instance);

        var act = async () => await service.GetProductByIdAsync(99, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ApiException>();
        exception.Which.StatusCode.Should().Be(NotFoundStatusCode);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReplaceItemsAndSetModifiedFields()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository, NullLogger<ProductService>.Instance);
        var created = await service.CreateProductAsync(new CreateProductRequest
        {
            ProductName = "Mouse",
            CreatedBy = "seed",
            Items = [new CreateItemRequest { Quantity = 1 }]
        }, CancellationToken.None);

        var result = await service.UpdateProductAsync(created.Id, new UpdateProductRequest
        {
            ProductName = "Gaming Mouse",
            ModifiedBy = "editor",
            Items = [new UpdateItemRequest { Quantity = 5 }]
        }, CancellationToken.None);

        result.ProductName.Should().Be("Gaming Mouse");
        result.ModifiedBy.Should().Be("editor");
        result.Items.Should().ContainSingle().Which.Quantity.Should().Be(5);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = [];
        private int _nextProductId = 1;
        private int _nextItemId = 1;

        public Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters queryParameters, CancellationToken cancellationToken)
        {
            var items = _products
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToArray();

            return Task.FromResult(new PagedResult<Product>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                TotalCount = _products.Count
            });
        }

        public Task<Product?> GetByIdAsync(int id, bool asNoTracking, CancellationToken cancellationToken)
        {
            var product = _products.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(product is null ? null : Clone(product));
        }

        public Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
        {
            product.Id = _nextProductId++;
            foreach (var item in product.Items)
            {
                item.Id = _nextItemId++;
                item.ProductId = product.Id;
            }

            var stored = Clone(product);
            _products.Add(stored);
            return Task.FromResult(Clone(stored));
        }

        public Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            var index = _products.FindIndex(x => x.Id == product.Id);
            _products[index] = Clone(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            _products.RemoveAll(x => x.Id == product.Id);
            return Task.CompletedTask;
        }

        private Product Clone(Product product)
        {
            return new Product
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn,
                Items = product.Items.Select(item => new Item
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}
