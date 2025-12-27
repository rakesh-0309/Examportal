namespace Assessment.DTO
{
    public class AutoDtos
    {

        public record RegisterRequest(string Email, string Password, string? FullName);
        public record LoginRequest(string Email, string Password);
        public record LoginResponse(string Token, string UserId, string Email, string Role);
    }
}
