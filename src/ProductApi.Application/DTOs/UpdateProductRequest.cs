namespace ProductApi.Application.DTOs;

public class UpdateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public List<UpdateItemRequest> Items { get; set; } = new();
}
