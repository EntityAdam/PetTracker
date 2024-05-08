using Core.Access;
using Core.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddPetTracker(this IServiceCollection services)
    {
        services.AddTransient<IDomainFacade, Facade>();
        services.AddSingleton<IHistoryProvider, HistoryProviderInMemeory>();
        services.AddSingleton<IDataFacade, DataFacadeInMemory>();
        services.AddSingleton<IUserRolesStore, UserRoleStoreInMemory>();
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IAccessRoleManager, AccessRoleManager>();
        return services;
    }
}