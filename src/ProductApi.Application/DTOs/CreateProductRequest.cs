namespace ProductApi.Application.DTOs;

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public List<CreateItemRequest> Items { get; set; } = new();
}
