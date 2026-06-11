using Microsoft.Extensions.Logging;
using ProductApi.Application.Common;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mappings;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
    private const int NotFoundStatusCode = 404;
    private readonly ILogger<ProductService> _logger;
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<ProductResponse>> GetProductsAsync(ProductQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetPagedAsync(queryParameters, cancellationToken);

        return new PagedResult<ProductResponse>
        {
            Items = result.Items.Select(product => product.ToResponse()).ToArray(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken)
            ?? throw new ApiException(NotFoundStatusCode, $"Product with id {id} was not found.");

        return product.ToResponse();
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var product = new Product
        {
            ProductName = request.ProductName.Trim(),
            CreatedBy = request.CreatedBy.Trim(),
            CreatedOn = now,
            Items = request.Items
                .Select(item => new Item
                {
                    Quantity = item.Quantity
                })
                .ToList()
        };

        var createdProduct = await _productRepository.AddAsync(product, cancellationToken);

        _logger.LogInformation("Created product {ProductId} with {ItemCount} items", createdProduct.Id, createdProduct.Items.Count);

        return createdProduct.ToResponse();
    }

    public async Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new ApiException(NotFoundStatusCode, $"Product with id {id} was not found.");

        product.ProductName = request.ProductName.Trim();
        product.ModifiedBy = request.ModifiedBy.Trim();
        product.ModifiedOn = DateTime.UtcNow;

        product.Items.Clear();
        foreach (var item in request.Items)
        {
            product.Items.Add(new Item
            {
                ProductId = product.Id,
                Quantity = item.Quantity
            });
        }

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Updated product {ProductId}", product.Id);

        return product.ToResponse();
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new ApiException(NotFoundStatusCode, $"Product with id {id} was not found.");

        await _productRepository.DeleteAsync(product, cancellationToken);

        _logger.LogInformation("Deleted product {ProductId}", id);
    }
}
