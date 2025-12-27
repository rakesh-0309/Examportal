namespace Assessment.DTO
{
    public class FormDtos
    {
    public record CreateFormRequest(string Title, string? Description, string? FieldsJson, decimal Price, string Currency);
    public record UpdateFormRequest(string Title, string? Description, string? FieldsJson, bool IsActive, decimal Price, string Currency);
    }
}