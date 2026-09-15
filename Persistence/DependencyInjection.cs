using System.Text;
using System.Globalization;
using System.Threading.RateLimiting;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.AdminCreateDiploma.Validators;
using exam_system.Features;
using exam_system.Features.Shared.Behaviors;
using exam_system.Features.Shared.Services;
using exam_system.Helper;
using exam_system.Infrastructure.Email;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace exam_system.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=ExaminationSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        // Inject Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
        // JWT access tokens authenticate API requests; refresh tokens use their own cookie.
        var jwtOption = configuration.GetRequiredSection("Jwt").Get<JwtOption>()
            ?? throw new InvalidOperationException("Jwt settings are missing.");

        if (string.IsNullOrWhiteSpace(jwtOption.Issuer) ||
            string.IsNullOrWhiteSpace(jwtOption.Audience) ||
            string.IsNullOrWhiteSpace(jwtOption.Key) 
            || Encoding.UTF8.GetByteCount(jwtOption.Key) < 32 ||
            jwtOption.DurationInMinutes <= 0)
        {
            throw new InvalidOperationException(
                "Configure Jwt:Issuer, Jwt:Audience, a positive Jwt:DurationInMinutes and a Jwt:Key of at least 32 UTF-8 bytes using user secrets or environment variables.");
        }

        services.AddSingleton(Options.Create(jwtOption));
        services.AddScoped<ITokenService, TokenService>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOption.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOption.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOption.Key)
                ),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "sub",
                RoleClaimType = "role",
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
            };
        });





        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
     
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(typeof(Program).Assembly);
        
        services.AddScoped<IEmailSender, EmailSender>();
        // hach otp
        services.AddScoped<IPasswordHasher<EmailVerificationOtp>, PasswordHasher<EmailVerificationOtp>>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
    {

        services.AddMediatR(typeof(Program).Assembly);



        services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>),typeof(TransactionBehavior<,>));

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    $"{context.Connection.RemoteIpAddress}:{context.GetEndpoint()?.DisplayName}",
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var retrySeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? Math.Max(1, Math.Ceiling(retryAfter.TotalSeconds))
                    : 60;
                context.HttpContext.Response.Headers.RetryAfter = retrySeconds.ToString(CultureInfo.InvariantCulture);

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new EndpointResponse(false, StatusCodes.Status429TooManyRequests,
                        "Too many requests. Please try again later."), cancellationToken);
            };
        });

        return services;
    }

}
