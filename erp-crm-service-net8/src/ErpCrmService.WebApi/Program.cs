using System.Text.Json.Serialization;
using ErpCrmService.Application.Interfaces;
using ErpCrmService.Application.Mappers;
using ErpCrmService.Application.Services;
using ErpCrmService.Domain.Repositories;
using ErpCrmService.Domain.Services;
using ErpCrmService.Infrastructure.Data;
using ErpCrmService.Infrastructure.Repositories;
using ErpCrmService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) =>
        config.WriteTo.Console().ReadFrom.Configuration(context.Configuration));

    // Database
    var connectionString = builder.Configuration["DATABASE_URL"]
        ?? throw new InvalidOperationException("DATABASE_URL is not configured");

    builder.Services.AddDbContext<ErpCrmDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Repositories
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();

    // Domain services
    builder.Services.AddScoped<ICustomerDomainService, CustomerDomainService>();

    // Mappers
    builder.Services.AddScoped<IAddressMapper, AddressMapper>();
    builder.Services.AddScoped<ICustomerMapper, CustomerMapper>();

    // Application services
    builder.Services.AddScoped<ICustomerService, CustomerService>();

    // Controllers
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "ERP CRM Service", Version = "v1" });
    });

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

    var app = builder.Build();

    // Auto-apply migrations on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ErpCrmDbContext>();
        db.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP CRM Service v1"));

    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

    app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "erp-crm" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
