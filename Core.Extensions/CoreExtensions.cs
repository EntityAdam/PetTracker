using Core.Access;
using Core.Interface;
using Core.Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddPetTracker(this IServiceCollection services)
    {
        services.AddDbContext<PetTrackerDbContext>(options =>
            options.UseSqlite("Data Source=pettracker.db"));

        services.AddTransient<IDomainFacade, Facade>();

        services.AddTransient<IShelterFacade, ShelterAccessAdapter>();
        services.AddTransient<IShelterPetsFacade>(sp => (IShelterPetsFacade)sp.GetRequiredService<IShelterFacade>());
        services.AddTransient<IShelterHistoryFacade>(sp => (IShelterHistoryFacade)sp.GetRequiredService<IDomainFacade>());


        services.AddScoped<IHistoryProvider, EfCoreHistoryProvider>();
        services.AddScoped<IDataFacade, EfCoreDataFacade>();
        services.AddSingleton<IUserRolesStore, UserRoleStoreInMemory>();
        services.AddScoped<User>(_ => new User("anonymous"));
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IAccessRoleManager, AccessRoleManager>();
        return services;
    }
}