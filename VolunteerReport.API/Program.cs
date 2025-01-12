using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;
using VolunteerReport.Common.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using VolunteerReport.Domain;
using VolunteerReport.Infrastructure.Services.Interfaces;
using EnRoute.Common.Configuration;
using EnRoute.Infrastructure.Services.Interfaces;
using EnRoute.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EnRoute.Infrastructure.Strategies;
using VolunteerReport.Domain.Models;
using VolunteerReport.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly);

AdminSettings adminSettings = builder.Configuration.GetSection("AdminSettings").Get<AdminSettings>() ??
                              throw new Exception("Admin configuration is missing");


JwtSettings jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ??
                          throw new Exception("JWT configuration is missing");

builder.Services.AddSingleton(adminSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IAccusationService, AccusationService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IGoogleJwtTokenParser, GoogleJwtTokenParser>();
builder.Services.AddScoped<IRoleStrategyFactory, RoleStrategyFactory>();
builder.Services.AddSingleton<IPhotoPlagiarismService, PhotoPlagiarizmService>();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"),
        o => o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidAudience = jwtSettings.ValidAudience,
            ValidIssuer = jwtSettings.ValidIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

const string CORS_POLICY = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin();
            corsPolicyBuilder.AllowAnyMethod();
            corsPolicyBuilder.AllowAnyHeader();
        });
});

builder.Services.AddControllers(c =>
{
    c.ModelValidatorProviders.Clear();
    c.ValidateComplexTypesIfChildValidationFails = false;
}).AddOData(opt =>
{
    opt.AddRouteComponents("odata", GetEdmModel());
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddAutoMapper(typeof(VolunteerReport.API.MappingProfile).Assembly);

//builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header

            },
            new List<string>()
        }
    });
});

//builder.Services.AddHostedService<AdminInitializerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CORS_POLICY);

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandlerMiddleware();

app.MapControllers();

app.Run();



static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<User>("Users").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Volunteer>("Volunteers").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Accusation>("Accusations").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Report>("Reports").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<ReportCategory>("Categories").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<ReportDetail>("ReportDetails").EntityType.Count().Filter().Expand().Select();

    builder.EnableLowerCamelCase();
    return builder.GetEdmModel();
}
