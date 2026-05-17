using SafeConcierge.Application.DependencyInjection;
using SafeConcierge.Infrastructure.DependencyInjection;
using SafeConcierge.Api.Endpoints;
using SafeConcierge.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ── Camadas ───────────────────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddJwtAuth(builder.Configuration);

// ── API ───────────────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapGroup("/api/v1/auth")
   .WithTags("Auth")
   .MapAuthEndpoints();

app.MapGroup("/api/v1/condominiums")
   .WithTags("Condominiums")
   .MapCondominiumEndpoints();

app.MapGroup("/api/v1/towers")
   .WithTags("Towers")
   .MapTowerEndpoints();

app.MapGroup("/api/v1/apartments")
   .WithTags("Apartments")
   .MapApartmentEndpoints();

app.MapGroup("/api/v1/users")
   .WithTags("Users")
   .MapUserEndpoints();

app.MapGroup("/api/v1/delivery-companies")
   .WithTags("DeliveryCompanies")
   .MapDeliveryCompanyEndpoints();

app.MapGroup("/api/v1/packages")
   .WithTags("Packages")
   .MapPackageEndpoints();

app.MapGroup("/api/v1/packages")
   .WithTags("Packages")
   .MapPackageDeliveryLogEndpoints();

// PickupCode endpoints routed under /api/v1/packages/{packageId}
app.MapGroup("/api/v1/packages/{packageId:guid}")
   .WithTags("PickupCodes")
   .MapPickupCodeEndpoints();

app.MapGroup("/api/v1/delivery-logs")
   .WithTags("DeliveryLogs")
   .MapDeliveryLogEndpoints();

app.MapGroup("/api/v1/notifications")
   .WithTags("Notifications")
   .MapNotificationEndpoints();

app.MapGroup("/api/v1/audit-logs")
   .WithTags("AuditLogs")
   .MapAuditLogEndpoints();

app.MapGroup("/api/v1/dashboard")
   .WithTags("Dashboard")
   .MapDashboardEndpoints();

app.Run();
