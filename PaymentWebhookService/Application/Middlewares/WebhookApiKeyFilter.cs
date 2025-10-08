using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentWebhookService.Api.Middlewares
{
    public class WebhookApiKeyFilter : IAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookApiKeyFilter> _logger;

        public WebhookApiKeyFilter(IConfiguration configuration, ILogger<WebhookApiKeyFilter> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogWarning("Missing API Key in webhook request from {IP}",
                    context.HttpContext.Connection.RemoteIpAddress);
                context.Result = new UnauthorizedObjectResult(new { message = "API Key is missing" });
                return;
            }

            var apiKey = _configuration["Webhook:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Webhook API Key not configured");
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            if (!apiKey.Equals(extractedApiKey))
            {
                _logger.LogWarning("Invalid API Key in webhook request from {IP}",
                    context.HttpContext.Connection.RemoteIpAddress);
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid API Key" });
                return;
            }
        }
    }
}