namespace AuthService.Business.Dtos.Auth
{
    public class TokenResponseDto
    {
        public required string Token { get; set; }
        public required int ExpiresIn { get; set; }
        public required string TokenType { get; set; }
    }
}
