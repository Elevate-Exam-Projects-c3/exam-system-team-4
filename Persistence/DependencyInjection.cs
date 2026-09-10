using exam_system.Domain.Entities.Diplomas;
using exam_system.Features;
using exam_system.Features.Diplomas.EnrollDiploma.Interfaces;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;
using MediatR;
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

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
     
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(typeof(Program).Assembly);
        return services;
    }

}
