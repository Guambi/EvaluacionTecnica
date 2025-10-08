using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PaymentWebhookService.Api.Middlewares;
using PaymentWebhookService.Business.Implementation;
using PaymentWebhookService.Business.Interfaces;
using PaymentWebhookService.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Webhook Service API",
        Version = "v1",
        Description = "Microservicio de gestión de pagos",
        Contact = new OpenApiContact
        {
            Name = "Payment Team",
            Email = "support@payments.com"
        }
    });

    // Webhook API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key for webhook authentication",
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey" // Must match the Id used in AddSecurityDefinition
                }
            },
            new string[] { } // Scope, empty for API key
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});


builder.Services.AddDbContext<PaymentWebhookDbContext>(options =>
    options.UseSqlite("Data Source=payments.db"));

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<WebhookApiKeyFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
