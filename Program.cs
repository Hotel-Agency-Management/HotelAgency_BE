using Booking.Models;
using Booking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Booking.Interfaces.Services;
using Booking.Interfaces.Repositories;
using Booking.Repositories;
using Booking.Services;
using Booking.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Booking.Data.Seeders;
using Booking.Middleware;
using Booking.Clients;
using Hangfire;
using Hangfire.MySql;
using Booking.Strategies;
using Booking.Factories;
using System.Text.Json.Serialization;
using Booking.Converters;
using Booking.Repositories.Implementations;
using Booking.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
               .Add(new RegisterRequestJsonConverter());
    });


// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

// Repositories 
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IAgencyDocumentRepository, AgencyDocumentRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IFacilityPhotoRepository, FacilityPhotoRepository>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomPhotoRepository, RoomPhotoRepository>();


//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAgencyService, AgencyService>();
builder.Services.AddScoped<IAgencyDocumentService, AgencyDocumentService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IFacilityPhotoService, FacilityPhotoService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomPhotoService, RoomPhotoService>();


// Strategies
builder.Services.AddScoped<CustomerRegistrationStrategy>();
builder.Services.AddScoped<AgencyOwnerRegistrationStrategy>();
builder.Services.AddScoped<BaseProfileStrategy>();
builder.Services.AddScoped<AgencyOwnerProfileStrategy>();
builder.Services.AddScoped<HotelStaffProfileStrategy>();

//Factories
builder.Services.AddScoped<IRegistrationStrategyFactory, RegistrationStrategyFactory>();
builder.Services.AddScoped<IProfileStrategyFactory, ProfileStrategyFactory>();


// Email
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection("EmailOptions"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailJobService, EmailJobService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

// Hangfire with MySQL
var hangfireConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddHangfire(config =>
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseStorage(new MySqlStorage(hangfireConnection, new MySqlStorageOptions
        {
            TablesPrefix = "Hangfire",
            QueuePollInterval = TimeSpan.FromSeconds(15),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            PrepareSchemaIfNecessary = true,
            TransactionTimeout = TimeSpan.FromMinutes(1)
        }))
);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHangfireServer(options => options.WorkerCount = 2);

builder.Services.AddAuthorization();

var app = builder.Build();

await SeedManager.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Hangfire Dashboard — only in dev, no auth needed
    app.UseHangfireDashboard("/hangfire");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
