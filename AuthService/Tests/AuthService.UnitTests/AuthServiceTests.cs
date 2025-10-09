using AuthService.Api.Services.Jwt;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuthService.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly MockJwtService _service;

        public AuthServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForDevelopmentOnly123456789");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("AuthService");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("EnrollmentServices");

            _service = new MockJwtService(_configMock.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Act
            var token = _service.GenerateToken();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT format has dots
        }
    }
}