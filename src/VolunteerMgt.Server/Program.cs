using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Common;
using VolunteerMgt.Server.Common.Authorization;
using VolunteerMgt.Server.Common.Config;
using VolunteerMgt.Server.Common.Exceptions;
using VolunteerMgt.Server.Common.Logger;
using VolunteerMgt.Server.Common.Middleware;
using VolunteerMgt.Server.Common.Settings;
using VolunteerMgt.Server.Endpoints;
using VolunteerMgt.Server.Entities.Identity;
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
    var commandTimeout = (int)TimeSpan.FromMinutes(20).TotalSeconds;

    builder.Services.AddDbContext<DatabaseContext>(options =>
    {
        options.UseSqlServer(connectionString, sqlOpt =>
        {
            sqlOpt.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName);

            sqlOpt.CommandTimeout(commandTimeout);
            sqlOpt.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);

        });
#if DEBUG
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
#endif
    });
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);


    // Add Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<DatabaseContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)));
    JwtConfiguration? jwtConfiguration = builder.Configuration.GetSection(nameof(JwtConfiguration)).Get<JwtConfiguration>();

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = jwtConfiguration?.ValidIssuer,
                ValidAudience = jwtConfiguration?.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration?.Secret!))
            };
        });
    builder.Services.AddAuthorizationBuilder();

    // Add Anti-CSRF/XSRF services
    //builder.Services.AddAntiforgery();

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
    });

    // Add cors
    builder.Services.AddCors();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

    // Add application service
    builder.Services.AddServices();

    builder.Services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUserService>());

    //builder.Services.AddScoped<ExceptionMiddleware>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddScoped<CurrentUserMiddleware>();
    builder.Services.AddScoped<CustomSecurityHeaderMiddleware>();

    //Register services
    builder.Services.AddHealthChecks();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = $"Volunteer management {builder.Environment.EnvironmentName}", Version = "v1" });
        //option.SwaggerDoc("v2", new OpenApiInfo { Title = "Kompublic Cloud auth API", Version = "v2" });
        option.OperationFilter<SwaggerAuthorizeOperationFilter>();
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    In = ParameterLocation.Header,
                },
                Array.Empty<string>()
            }
        });
        //option.EnableAnnotations();            
        // Add custom header for language
        //option.OperationFilter<AddCustomHeaderParameter>();
    });

    var app = builder.Build();


    //add exception handler to the pipeline
    app.UseExceptionHandler();
    //app.UseMiddleware<ExceptionMiddleware>();

    //app.UseAntiforgery();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Swagger UI - Volunteer management";
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

    app.UseMiddleware<CurrentUserMiddleware>();
    app.UseMiddleware<CustomSecurityHeaderMiddleware>();

    // Add all endpoints here.
    app.MapAuthenticationEndpoints();
    app.MapVolunteerEndpoints();
    app.MapServiceEndpoints();
    app.MapVolunteerServiceEndpoints();
    app.MapCouponsEndpoints();

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