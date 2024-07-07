using FluentValidation;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;
using VolunteerReport.Common.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.Domain;
using VolunteerReport.Infrastructure.Services.Interfaces;
using VolunteerReport.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly);

AdminSettings adminSettings = builder.Configuration.GetSection("AdminSettings").Get<AdminSettings>() ??
                              throw new Exception("Admin configuration is missing");

builder.Services.AddSingleton(adminSettings);

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"),
        o => o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

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
builder.Services.AddSwaggerGen();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandlerMiddleware();

app.MapControllers();

app.Run();



static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    // ...

    builder.EnableLowerCamelCase();
    return builder.GetEdmModel();
}
