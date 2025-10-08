using AuthService.Api.Services.Jwt;
using AuthService.Business.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMockJwtService jwtService) : ControllerBase
    {
        #region Services

        private readonly IMockJwtService _jwtService = jwtService;

        #endregion Services

        #region APIS

        /// <summary>
        /// Imita la generación de un token JWT
        /// </summary>
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateToken([FromBody] TokenRequestDto request)
        {
            // Mock authentication - siempre autoriza por la imitación
            // Al ser un microservicio debería se obtenido desde otro servicio
            var token = _jwtService.GenerateToken();

            return Ok(new TokenResponseDto
            {
                Token = token,
                ExpiresIn = 7200,
                TokenType = "Bearer"
            });
        }

        #endregion APIS
    }
}