using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using VolunteerMgt.Server.Authorization;
using VolunteerMgt.Server.Common.Config;
using VolunteerMgt.Server.Common.Logger;
using VolunteerMgt.Server.Endpoints;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Exceptions;
using VolunteerMgt.Server.Persistence;

//Init the logger and get the active config
using var logger = new SerilogLogger(ConfigurationHelper.ActiveConfiguration);

try
{
    logger.LogInformation("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hc, lc) => lc.ReadFrom.Configuration(hc.Configuration));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<DatabaseContext>(options =>
    {
        options.UseSqlServer(connectionString,
           b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName));
    });

    // Add Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<DatabaseContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication().AddJwtBearer();
    builder.Services.AddAuthorizationBuilder();

    // Add Anti-CSRF/XSRF services
    //builder.Services.AddAntiforgery();

    var jwtSettings = new JwtSettings();
    builder.Configuration.Bind(nameof(JwtSettings), jwtSettings);
    builder.Services.AddSingleton(jwtSettings);

    // Configure Identity options and password complexity here
    builder.Services.Configure<IdentityOptions>(options =>
    {
        // User settings
        options.User.RequireUniqueEmail = true;

        // Password settings
        /*
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = false;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        */

        // Configure Identity to use the same JWT claims as OpenIddict
        options.ClaimsIdentity.UserNameClaimType = Claims.Name;
        options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
        options.ClaimsIdentity.RoleClaimType = Claims.Role;
        options.ClaimsIdentity.EmailClaimType = Claims.Email;
    });

    // Add cors
    builder.Services.AddCors();

    //Register services
    builder.Services.AddHealthChecks();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Volunteer management", Version = "v1" });
        c.OperationFilter<SwaggerAuthorizeOperationFilter>();
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/connect/token", UriKind.Relative)
                }
            }
        });
    });

    var app = builder.Build();

    //add exception handler to the pipeline
    app.UseExceptionHandler();

    //app.UseAntiforgery();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Swagger UI - QuickApp";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Volunteer management V1");
        });
    }

    app.UseHttpsRedirection();

    //Add health checks - ref: https://www.milanjovanovic.tech/blog/health-checks-in-asp-net-core
    app.MapHealthChecks("/health");

    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapTodoEndpoints();

    app.MapFallbackToFile("/index.html");

    app.Run();
}
catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType().Name != "StopTheHostException")
{
    logger.LogFatal(ex, "An unhandled exception occurred during application startup.");
}
finally
{
    logger.LogInformation("Application shutdown complete");
}