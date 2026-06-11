using ProductApi.Application.Common;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductResponse>> GetProductsAsync(ProductQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<ProductResponse> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task DeleteProductAsync(int id, CancellationToken cancellationToken);
}
