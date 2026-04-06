using Core.Extensions;
using Core.Interface;
using Core.Interface.Events;
using Core.Interface.Models;
using Core.Access;
using Core.Interface.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();                                             //openapi https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0
builder.Services.AddSwaggerGen(options => options.ConfigureSwaggerToAcceptJwtBearer()); //openapi https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0
builder.Services.AddAuthentication().AddJwtBearer();                                    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0
builder.Services.AddAuthorizationBuilder().AddPolicy("shelter-policy", policy => policy.RequireRole("shelter", "Shelter"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddPetTracker();
builder.Services.AddScoped<User>(sp =>
{
    var user = sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.User;
    var principalId = user?.Identity?.Name
        ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? "anonymous";

    return new User(principalId);
});
builder.Services.AddScoped<ShelterApiViewModel>();
builder.Services.AddScoped<ShelterHistoryApiViewModel>();
builder.Services.AddScoped<ShelterPetsHistoryApiViewModel>();
builder.Services.AddScoped<ShelterPetsApiViewModel>();
builder.Services.AddScoped<ShelterPeopleApiViewModel>();

//next steps
//https://auth0.com/docs/get-started/applications/configure-application-metadata
//https://www.nuget.org/packages/Auth0.ManagementApi

/// JWT
/// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0uVmluY2VudCIsInN1YiI6IkFkYW0uVmluY2VudCIsImp0aSI6ImVkOTMzMDNiIiwicm9sZSI6InNoZWx0ZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDI1NCIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzczIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI2NyJdLCJuYmYiOjE2OTE4MTI5MjAsImV4cCI6MTY5OTc2MTcyMCwiaWF0IjoxNjkxODEyOTIwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ETEUXYOuMN1yY0GeqAw4aRcT0EfeXEBJ5lTHiJkZ_Gk
/// curl test
/// curl -i -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0uVmluY2VudCIsInN1YiI6IkFkYW0uVmluY2VudCIsImp0aSI6ImVkOTMzMDNiIiwicm9sZSI6InNoZWx0ZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDI1NCIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzczIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI2NyJdLCJuYmYiOjE2OTE4MTI5MjAsImV4cCI6MTY5OTc2MTcyMCwiaWF0IjoxNjkxODEyOTIwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ETEUXYOuMN1yY0GeqAw4aRcT0EfeXEBJ5lTHiJkZ_Gk" https://localhost:7267/secret
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetTrackerDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var principalId = context.User.Identity?.Name
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "anonymous";
        var user = new User(principalId);
        var roleManager = context.RequestServices.GetRequiredService<IAccessRoleManager>();

        foreach (var role in context.User.FindAll(ClaimTypes.Role).Concat(context.User.FindAll("role")).Select(r => r.Value).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (Program.TryMapRoleKind(role, out var roleKind))
            {
                roleManager.AssignRole(new AccessRoleAssignment(user, new Role(roleKind)));
            }
        }
    }

    await next();
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. My secret").RequireAuthorization();

app.MapGet("/claims", (ClaimsPrincipal user) =>
{
    var sb = new StringBuilder();
    sb.AppendLine("Claims:");
    foreach (var claim in user.Claims)
    {
        sb.AppendLine($"  - '{claim.Type}': '{claim.Value}' ");
    }
    return sb.ToString();
}).RequireAuthorization();

// --------------------------------------------------  Shelters -------------------------------------------------- //

app.MapPost("/shelters", async Task<Results<Created<Shelter>, BadRequest>> ([FromServices] ShelterApiViewModel viewModel, [FromBody] ShelterModel shelter) =>
    await viewModel.Create(shelter)
      is Shelter createdShelter
        ? TypedResults.Created($"/shelters/{createdShelter.Id.Id}", createdShelter)
                : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

app.MapGet("/shelters", async (ShelterApiViewModel viewModel) =>
    TypedResults.Ok(await viewModel.ListAll()));

app.MapGet("/shelters/{id}", async Task<Results<Ok<Shelter>, NotFound, BadRequest>> (string id, [FromServices] ShelterApiViewModel viewModel) =>
{
        if (!Ulid.TryParse(id, out _))
        {
                return TypedResults.BadRequest();
        }

        return await viewModel.GetById(id)
            is Shelter shelter
                ? TypedResults.Ok(shelter)
                : TypedResults.NotFound();
}).WithName("GetShelterById");

app.MapDelete("/shelters/{id}", async Task<Results<NoContent, BadRequest>> (string id, [FromServices] ShelterApiViewModel viewModel) =>
    await viewModel.Delete(id)
      is true
        ? TypedResults.NoContent()
                : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

// --------------------------------------------------  Shelters History -------------------------------------------------- //

app.MapGet("/shelters/{shelterId}/history", async Task<Results<Ok<IEnumerable<ShelterEvent>>, NotFound, BadRequest>> (string shelterId, [FromServices] ShelterHistoryApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.GetHistoryById(shelterId)
        is IEnumerable<ShelterEvent> shelterEvents
        && shelterEvents.Any()
        ? TypedResults.Ok(shelterEvents)
        : TypedResults.NotFound();
});

app.MapGet("/shelters/{shelterId}/history/{eventKind:int}", async Task<Results<Ok<IEnumerable<ShelterEvent>>, NotFound, BadRequest>> (string shelterId, int eventKind, [FromServices] ShelterHistoryApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Enum.IsDefined(typeof(ShelterEventKind), eventKind))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.GetHistoryEventTypeById(shelterId, eventKind)
        is IEnumerable<ShelterEvent> shelterEvents
        && shelterEvents.Any()
        ? TypedResults.Ok(shelterEvents)
        : TypedResults.NotFound();
});

app.MapGet("/shelters/{shelterId}/history/date-listed", async Task<Results<Ok<ShelterEvent>, NotFound, BadRequest>> (string shelterId, [FromServices] ShelterHistoryApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.GetListedDate(shelterId)
        is ShelterEvent shelterEvent
            ? TypedResults.Ok(shelterEvent)
            : TypedResults.NotFound();
});

// --------------------------------------------------  Shelter Pets -------------------------------------------------- //

app.MapPost("/shelters/{shelterId}/pets", async Task<Results<Created<ShelteredPet>, BadRequest>> (string shelterId, [FromServices] ShelterPetsApiViewModel viewModel, [FromBody] ListPetModel listPetModel) =>
    await viewModel.AddPet(shelterId, listPetModel)
       is ShelteredPet petModel
         ? TypedResults.Created($"/shelters/{shelterId}/pets/{petModel.Pet.Id}", petModel)
                 : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

app.MapGet("/shelters/{shelterId}/pets", async Task<Results<Ok<IEnumerable<ShelteredPet>>, BadRequest>> (string shelterId, [FromServices] ShelterPetsApiViewModel viewModel) =>
    await viewModel.ListAllPets(shelterId)
        is IEnumerable<ShelteredPet> shelteredPets
        ? TypedResults.Ok(shelteredPets)
        : TypedResults.BadRequest());

app.MapGet("/shelters/{shelterId}/pets/{petId}", async Task<Results<Ok<ShelteredPet>, NotFound, BadRequest>> (string shelterId, string petId, [FromServices] ShelterPetsApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Ulid.TryParse(petId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.GetPetById(shelterId, petId)
        is ShelteredPet shelteredPet
            ? TypedResults.Ok(shelteredPet)
            : TypedResults.NotFound();
});

app.MapPut("/shelters/{shelterId}/pets/{petId}/transfer", async Task<Results<Ok<ShelteredPetEvent>, BadRequest>> (string shelterId, string petId, [FromServices] ShelterPetsApiViewModel viewModel, [FromBody] string shelterIdTarget) =>
    await viewModel.TransferPet(shelterId, petId, shelterIdTarget)
        is ShelteredPetEvent transferEvent
        ? TypedResults.Ok(transferEvent)
        : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

// --------------------------------------------------  People -------------------------------------------------- //

app.MapPost("/fosterpersons", async Task<Results<Created<FosterPerson>, BadRequest>> ([FromServices] ShelterPeopleApiViewModel viewModel, [FromBody] CreateFosterPersonModel model) =>
    await viewModel.CreateFosterPerson(model)
        is FosterPerson fosterPerson
        ? TypedResults.Created($"/fosterpersons/{fosterPerson.Id.Id}", fosterPerson)
        : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

app.MapPost("/adopterpersons", async Task<Results<Created<AdopterPerson>, BadRequest>> ([FromServices] ShelterPeopleApiViewModel viewModel, [FromBody] CreateAdopterPersonModel model) =>
    await viewModel.CreateAdopterPerson(model)
        is AdopterPerson adopterPerson
        ? TypedResults.Created($"/adopterpersons/{adopterPerson.Id.Id}", adopterPerson)
        : TypedResults.BadRequest()).RequireAuthorization("shelter-policy");

app.MapPut("/shelters/{shelterId}/pets/{petId}/foster/{fosterPersonId}", async Task<Results<Ok<ShelteredPetEvent>, BadRequest>> (
    string shelterId,
    string petId,
    string fosterPersonId,
    [FromServices] ShelterPeopleApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Ulid.TryParse(petId, out _) || !Ulid.TryParse(fosterPersonId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.AssignToFosterPerson(shelterId, petId, fosterPersonId)
        is ShelteredPetEvent fosterEvent
        ? TypedResults.Ok(fosterEvent)
        : TypedResults.BadRequest();
}).RequireAuthorization("shelter-policy");

app.MapPut("/shelters/{shelterId}/pets/{petId}/adopt/{adopterPersonId}", async Task<Results<Ok<ShelteredPetEvent>, BadRequest>> (
    string shelterId,
    string petId,
    string adopterPersonId,
    [FromServices] ShelterPeopleApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Ulid.TryParse(petId, out _) || !Ulid.TryParse(adopterPersonId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.AssignToAdopterPerson(shelterId, petId, adopterPersonId)
        is ShelteredPetEvent adoptEvent
        ? TypedResults.Ok(adoptEvent)
        : TypedResults.BadRequest();
}).RequireAuthorization("shelter-policy");

app.MapPut("/shelters/{shelterId}/pets/{petId}/outcome", async Task<Results<Ok<ShelteredPetEvent>, BadRequest>> (
    string shelterId,
    string petId,
    [FromBody] RecordOutcomeModel model,
    [FromServices] ShelterPeopleApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Ulid.TryParse(petId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.RecordOutcome(shelterId, petId, model.OutcomeKind)
        is ShelteredPetEvent outcomeEvent
        ? TypedResults.Ok(outcomeEvent)
        : TypedResults.BadRequest();
}).RequireAuthorization("shelter-policy");

// --------------------------------------------------  Shelter Pets History -------------------------------------------------- //

app.MapGet("/shelters/{shelterId}/pets/{petId}/history", async Task<Results<Ok<IEnumerable<ShelteredPetEvent>>, NotFound, BadRequest>> (string shelterId, string petId, [FromServices] ShelterPetsHistoryApiViewModel viewModel) =>
{
    if (!Ulid.TryParse(shelterId, out _) || !Ulid.TryParse(petId, out _))
    {
        return TypedResults.BadRequest();
    }

    return await viewModel.GetShelteredPetHistory(shelterId, petId)
        is IEnumerable<ShelteredPetEvent> shelteredPetEvents
        && shelteredPetEvents.Count() > 0
        ? TypedResults.Ok(shelteredPetEvents)
        : TypedResults.NotFound();
});

// --------------------------------------------------  People -------------------------------------------------- //

// users can join with the following intents

// just browsing
// looking to adopt // can adopt from a shelter only
// looking to foster // can foster from a shelter only
// looking for a shelter // can transfer to a shelter

// shelter owners
//   add their shelter // stretch: some kind of approval process
//   add their pets
//   chip pets
//   medical history pets
//   remove pets
//   remove their shelter // stretch: soft delete and purge
//   transfer pets to adopter
//   transfer pets to foster
//   transfer pets to shelter





//// list foster persons
//app.MapGet("fosterpersons/", () => { });
//// get foster person details
//app.MapGet("fosterpersons/{fosterPersonId}", () => { });
//// get foster person events
//app.MapGet("fosterpersons/{fosterPersonId}/history", () => { });
//// get foster person events by event kind
//app.MapGet("fosterpersons/{fosterPersonId}/history/{eventKind}", () => { });

//// list foster person pets
//app.MapGet("fosterpersons/{fosterPersonId}/pets", () => { });
//// get foster person pet details
//app.MapGet("fosterpersons/{fosterPersonId}/pets/{petId}", () => { });
//// get foster person pet history
//app.MapGet("fosterpersons/{fosterPersonId}/pets/{petId}/history", () => { });
//// get foster person pet history by event kind
//app.MapGet("fosterpersons/{fosterPersonId}/pets/{petId}/history/{eventKind}", () => { });

//// list adopter person pets
//app.MapGet("adopterpersons/{adoperPersonId}/pets", () => { });
//// get adopter person pet details
//app.MapGet("adopterpersons/{aFdopterPersonId}/pets/{petId}", () => { });
//// get adopter person pet history
//app.MapGet("adopterpersons/{adopterPersonId}/pets/{petId}/history", () => { });
//// get adopter person pet history by event kind
//app.MapGet("adopterrpersons/{adopterPersonId}/pets/{petId}/history/{eventKind}", () => { });


//// create sheltered pet
//app.MapPost("/shelters/{shelterId}/pets", (IShelterViewModel viewModel, [FromBody] PetModel pet) => 
//    {

//    });
//// create foster person
//app.MapPost("/fosterpersons", () => { });
//// create adopter person
//app.MapPost("/adopterpersons", () => { });


//// transfer pet from shelter to shelter
//app.MapPut("/shelters/{originShelterId}/pets/{petId}/shelter/{targetShelterId}", () => { });
//// transfer pet from shelter to rescue gorup
//app.MapPut("/shelters/{originShelterId}/pets/{petId}/rescuegroup/{targetRescueGroupId}", () => { });
//// transfer pet from shelter to foster person
//app.MapPut("/shelters/{originShelterId}/pets/{petId}/fosterperson/{fosterPersonId}", () => { });
//// transfer pet from shelter to adopter person
//app.MapPut("/shelters/{originShelterId}/pets/{petId}/adopterperson/{targetAdopterPersonId}", () => { });

//// transfer pet from rescue group to shelter
//app.MapPut("/rescuegroup/{originRescueGroupId}/pet/{petId}/shelter/{targetShelterId}", () => { });
//// transfer pet from rescue group to rescue group
//app.MapPut("/rescuegroup/{originRescueGroupId}/pet/{petId}/rescuegroup/{targetRescueGroupId}", () => { });
//// transfer pet from rescue group to foster person
//app.MapPut("/rescuegroup/{originRescueGroupId}/pet/{petId}/fosterperson/{targetFosterPersonId}", () => { });
//// transfer pet from rescue group to adopter person
//app.MapPut("/rescuegroup/{originRescueGroupId}/pet/{petId}/adopterperson/{targetAdopterPersonId}", () => { });


//// transfer pet from foster person to shelter
//app.MapPut("/fosterperson/{originFosterPersonId}/pet/{petId}/shelter/{targetShelterId}", () => { });
//// transfer pet from foster person to rescue group
//app.MapPut("/fosterperson/{originFosterPersonId}/pet/{petId}/rescuegroup/{targetRescueGroupId}", () => { });
//// can't xfer to a foster person -- must go through shelter
//// can't xfer to a adopter person -- must go through shelter



app.Run();


public record ShelterModel(string Name);
public record PetModel(string Name);
public record ListPet(string PetName, string ShelterName);
public record TransferPetByIdToShelterName(Ulid PetId, string Shelter);
public record CreateFosterPersonModel(string Name, int MaxPets);
public record CreateAdopterPersonModel(string Name);
public record RecordOutcomeModel(OutcomeKind OutcomeKind);
public partial class Program
{
    public static bool TryMapRoleKind(string claimValue, out RoleKind roleKind)
    {
        roleKind = RoleKind.Anonymous;
        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return false;
        }

        return claimValue.Trim().ToLowerInvariant() switch
        {
            "anonymous" => (roleKind = RoleKind.Anonymous) == RoleKind.Anonymous,
            "fosterperson" => (roleKind = RoleKind.FosterPerson) == RoleKind.FosterPerson,
            "adopterperson" => (roleKind = RoleKind.AdopterPerson) == RoleKind.AdopterPerson,
            "shelter" => (roleKind = RoleKind.Shelter) == RoleKind.Shelter,
            _ => false
        };
    }
}



//builder.Services.AddProblemDetails(); //problem details: https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-7.0
//builder.Services.AddAuthentication(options =>
//{
//    //more https://www.infoworld.com/article/3669188/how-to-implement-jwt-authentication-in-aspnet-core-6.html#:~:text=To%20secure%20a%20minimal%20API%20using%20JWT%20authentication%2C,secret%20key%20in%20the%20appsettings.json%20file.%20More%20items
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//    .AddJwtBearer()
//    .AddJwtBearer("LocalAuthIssuer"); //see dotnet-user-jwts https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0#use-dotnet-user-jwts-for-development-testing


//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("shelter-policy", policy => policy.RequireRole("shelter")); //dotnet user-jwts create --role "shelter"



public record ListPetModel(string Name);
public record TransferPetModel();