using exam_system.Domain.Entities.Diplomas;
using exam_system.Features;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Diplomas.AdminCreateDiploma.Validators;
using exam_system.Features;
using exam_system.Features.Shared.Behaviors;
using exam_system.Helper;
using exam_system.Infrastructure.Email;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

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
      

        return services;
    }

}
