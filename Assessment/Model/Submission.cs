namespace Assessment.Model
{
    public class Submission
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FormId { get; set; }
        public Form Form { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public string DataJson { get; set; } = "{}";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}
