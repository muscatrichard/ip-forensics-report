using IPForensics.Api;
using IPForensics.Api.Extensions;
using IPForensics.Core.Interfaces.Services;
using IPForensics.Core.Services;
using IPForensics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IIpLookupService, IpLookupService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddJwtAuthentication();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
