using Assessment;
using Assessment.Data;
using Assessment.Model;
using Assessment.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext (SQL Server LocalDB by default)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Identity + Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "PLEASE_SET_A_LONG_RANDOM_KEY";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ExamPortal";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ExamPortalAudience";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

//aqi
builder.Services.AddHttpClient<AQIService>();

//pdf
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = signingKey,

        // ? ADD THESE TWO LINES ?
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization();

// CORS for SPA (adjust origins as needed)
builder.Services.AddCors(o =>
{
    o.AddPolicy("Frontend", p =>
        p.WithOrigins("http://localhost:5173", "http://localhost:3000")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

// Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"] ?? "sk_test_replace";

// Services
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<PdfService>();

builder.Services.AddControllers();

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ExamPortal API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});




var app = builder.Build();
builder.WebHost.UseUrls(
    $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}"
);


app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Apply migrations and seed admin/roles
using (var scope = app.Services.CreateScope())
{
    //var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //await ctx.Database.MigrateAsync();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await RoleSeeder.EnsureSeedAsync(roleMgr, userMgr,
        builder.Configuration["Admin:Email"] ?? "admin@email.com",
        builder.Configuration["Admin:Password"] ?? "Admin@12345");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/", () => "ExamPortal API is running...");

app.Run();


