using Core.Interface.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.Helpers;

// sauce https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/minimal-apis/samples/MinApiTestsSample/IntegrationTests/Helpers/TestWebApplicationFactory.cs
// more reading: https://andrewlock.net/exploring-dotnet-6-part-6-supporting-integration-tests-with-webapplicationfactory-in-dotnet-6/
// more: https://github.com/DamianEdwards/MinimalApiPlayground/blob/main/tests/MinimalApiPlayground.Tests/PlaygroundApplication.cs
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PetTrackerDbContext>>();
            var dbPath = Path.Combine(Path.GetTempPath(), $"pettracker-tests-{Guid.NewGuid():N}.db");
            services.AddDbContext<PetTrackerDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });

        return base.CreateHost(builder);
    }
}