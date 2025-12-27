namespace Assessment.Model
{
    public class Form
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        // Optional dynamic fields schema as JSON for flexibility
        public string? FieldsJson { get; set; }
        public bool IsActive { get; set; } = true;

        // Price for the exam form (if applicable)
        public decimal Price { get; set; } = 100m;
        public string Currency { get; set; } = "usd";

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }

    public class CreateFormRequest
    {   public string Title { get; set; } = default!;
        public string? Description { get; set; }
        // Optional dynamic fields schema as JSON for flexibility
        public string? FieldsJson { get; set; }
        public bool IsActive { get; set; } = true;

        // Price for the exam form (if applicable)
        public decimal Price { get; set; } = 100m;
        public string Currency { get; set; } = "usd";
    }

    public class UpdateFormRequest
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        // Optional dynamic fields schema as JSON for flexibility
        public string? FieldsJson { get; set; }
        public bool IsActive { get; set; } = true;

        // Price for the exam form (if applicable)
        public decimal Price { get; set; } = 100m;
        public string Currency { get; set; } = "usd";
    }

}
