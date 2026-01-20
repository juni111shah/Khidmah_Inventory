using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Khidmah_Inventory.Application;
using Khidmah_Inventory.Infrastructure;
using Khidmah_Inventory.API.Middleware;
using Khidmah_Inventory.Infrastructure.Data;
using Khidmah_Inventory.API.Hubs;
using Khidmah_Inventory.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Khidmah Inventory API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
var issuer = jwtSettings["Issuer"] ?? "KhidmahInventory";
var audience = jwtSettings["Audience"] ?? "KhidmahInventory";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Configure SignalR JWT authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    // Permission-based policies will be created dynamically via AuthorizePermissionAttribute
});

// Register permission policy provider and handler
builder.Services.AddSingleton<IAuthorizationPolicyProvider, Khidmah_Inventory.API.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, Khidmah_Inventory.API.Authorization.PermissionHandler>();

// Add SignalR
builder.Services.AddSignalR();

// Register Analytics Broadcast Service
builder.Services.AddSingleton<Khidmah_Inventory.API.Services.AnalyticsBroadcastService>();
builder.Services.AddHostedService(provider => 
    provider.GetRequiredService<Khidmah_Inventory.API.Services.AnalyticsBroadcastService>());

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure file upload limits
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5242880; // 5MB
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // When using credentials, we must specify exact origins, not wildcard
        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200", "https://localhost:4200", "http://localhost:4204", "https://localhost:4204" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for SignalR with JWT tokens
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<MultiTenantMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Ensure uploads directory exists
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? app.Environment.ContentRootPath, "uploads");
var logosPath = Path.Combine(uploadsPath, "logos");
Directory.CreateDirectory(uploadsPath);
Directory.CreateDirectory(logosPath);

app.UseDefaultFiles();
app.UseStaticFiles();

// Serve uploaded files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.MapControllers();

// Map SignalR Hub
app.MapHub<Khidmah_Inventory.API.Hubs.AnalyticsHub>("/hubs/analytics");

app.MapFallbackToFile("/index.html");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// ============================================================================
// TEMPORARY: Generate password hash for seed data
// Remove this section after generating the hash and updating the seed file
// ============================================================================
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var identityService = scope.ServiceProvider.GetRequiredService<Khidmah_Inventory.Application.Common.Interfaces.IIdentityService>();
            var hash = await identityService.GeneratePasswordHashAsync("Admin@123");
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("🔐 PASSWORD HASH FOR ADMIN USER");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Password: Admin@123");
            Console.WriteLine($"Hash: {hash}");
            Console.WriteLine();
            Console.WriteLine("📋 COPY THIS SQL AND RUN IT IN SQL SERVER:");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"UPDATE Users");
            Console.WriteLine($"SET PasswordHash = '{hash}',");
            Console.WriteLine($"    UpdatedAt = GETUTCDATE()");
            Console.WriteLine($"WHERE Email = 'admin@khidmah.com'");
            Console.WriteLine($"  AND IsDeleted = 0;");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine();
            Console.WriteLine("✅ After running the SQL, you can login with:");
            Console.WriteLine("   Email: admin@khidmah.com");
            Console.WriteLine("   Password: Admin@123");
            Console.WriteLine(new string('=', 70) + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating hash: {ex.Message}");
        }
    }
}
// ============================================================================
// END TEMPORARY CODE
// ============================================================================

app.Run();

