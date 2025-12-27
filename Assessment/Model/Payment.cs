namespace Assessment.Model
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SubmissionId { get; set; }
        public Submission Submission { get; set; } = default!;

        public long AmountMinor { get; set; } // amount in the smallest currency unit (e.g., cents)
        public string Currency { get; set; } = "usd";

        public string Provider { get; set; } = "Stripe";
        public string? ProviderPaymentId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }


}
