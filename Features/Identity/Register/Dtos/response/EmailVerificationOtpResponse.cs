using System.Text.Json.Serialization;

namespace exam_system.Features.Identity.Register.Dtos.response
{
    public class EmailVerificationOtpResponse
    {
        public string Email { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        [JsonIgnore]
        public string PlainOtp { get; init; }
    }
}
