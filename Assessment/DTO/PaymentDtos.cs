namespace Assessment.DTO
{
    public class PaymentDtos
    {

        public record CreatePaymentRequest(Guid SubmissionId);
        public record CreatePaymentResponse(Guid PaymentId, string ClientSecret);
    }
}
