using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Data;
using TaskManager.Api.Infrastructure;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var database = builder.Configuration.GetSection(TaskDatabaseOptions.SectionName).Get<TaskDatabaseOptions>() ?? new TaskDatabaseOptions();
var connectionString = builder.Configuration.GetConnectionString("TaskManager");
var signingKey = ResolveSigningKey(jwt);

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:TaskManager is required.");
}

if (string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");
}

if (signingKey.Length < 32)
{
    throw new InvalidOperationException("Jwt signing key must be at least 32 characters. Set Jwt:SigningKey or Jwt:SigningKeyPath.");
}

if (jwt.ExpirationMinutes < 1)
{
    throw new InvalidOperationException("Jwt:ExpirationMinutes must be at least 1.");
}

if (jwt.ClockSkewSeconds < 0)
{
    throw new InvalidOperationException("Jwt:ClockSkewSeconds cannot be negative.");
}

if (string.IsNullOrWhiteSpace(database.PersonsCatalog))
{
    throw new InvalidOperationException("TaskDatabase:PersonsCatalog is required.");
}

var trackerConnection = new SqlConnectionStringBuilder(connectionString);
if (!string.IsNullOrWhiteSpace(database.TrackerCatalog)
    && !string.Equals(trackerConnection.InitialCatalog, database.TrackerCatalog, StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("ConnectionStrings:TaskManager database must match TaskDatabase:TrackerCatalog.");
}

var personsConnection = new SqlConnectionStringBuilder(connectionString)
{
    InitialCatalog = database.PersonsCatalog
}.ConnectionString;

builder.Services.AddSingleton(jwt);
builder.Services.AddSingleton(database);
builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PersonsDbContext>(options => options.UseSqlServer(personsConnection));
builder.Services.AddScoped<TaskCreationService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(jwt.ClockSkewSeconds)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogWarning(context.Exception, "JWT authentication failed.");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var exp = context.Principal?.FindFirst("exp")?.Value;
                var iat = context.Principal?.FindFirst("iat")?.Value;
                if (long.TryParse(exp, out var expSeconds) && long.TryParse(iat, out var iatSeconds))
                {
                    var lifetime = TimeSpan.FromSeconds(expSeconds - iatSeconds);
                    if (lifetime > TimeSpan.FromMinutes(jwt.ExpirationMinutes))
                    {
                        context.Fail("Token lifetime exceeds the configured maximum.");
                    }
                }

                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                if (context.Response.HasStarted)
                {
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "A valid bearer token is required."
                });
            },
            OnForbidden = async context =>
            {
                if (context.Response.HasStarted)
                {
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "The authenticated user is not allowed to perform this action."
                });
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Task Manager API");
    options.RoutePrefix = "swagger";
});
app.MapControllers();
app.Run();

static string ResolveSigningKey(JwtOptions jwt)
{
    if (!string.IsNullOrWhiteSpace(jwt.SigningKey))
    {
        return jwt.SigningKey.Trim();
    }

    if (string.IsNullOrWhiteSpace(jwt.SigningKeyPath))
    {
        return "";
    }

    return File.ReadAllText(jwt.SigningKeyPath).Trim();
}
