using Core.Extensions;
using Core.Interface.Events;
using Core.Interface.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();                                             //openapi https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0
builder.Services.AddSwaggerGen(options => options.ConfigureSwaggerToAcceptJwtBearer()); //openapi https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0
builder.Services.AddAuthentication().AddJwtBearer();                                    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0
builder.Services.AddAuthorization();                                                    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0
builder.Services.AddPetTracker();
builder.Services.AddSingleton<ShelterApiViewModel>();
builder.Services.AddSingleton<ShelterHistoryApiViewModel>();
builder.Services.AddSingleton<ShelterPetsHistoryApiViewModel>();
builder.Services.AddSingleton<ShelterPetsApiViewModel>();
builder.Services.AddSingleton<ShelterPetsHistoryApiViewModel>();

//next steps
//https://auth0.com/docs/get-started/applications/configure-application-metadata
//https://www.nuget.org/packages/Auth0.ManagementApi

/// JWT
/// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0uVmluY2VudCIsInN1YiI6IkFkYW0uVmluY2VudCIsImp0aSI6ImVkOTMzMDNiIiwicm9sZSI6InNoZWx0ZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDI1NCIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzczIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI2NyJdLCJuYmYiOjE2OTE4MTI5MjAsImV4cCI6MTY5OTc2MTcyMCwiaWF0IjoxNjkxODEyOTIwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ETEUXYOuMN1yY0GeqAw4aRcT0EfeXEBJ5lTHiJkZ_Gk
/// curl test
/// curl -i -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0uVmluY2VudCIsInN1YiI6IkFkYW0uVmluY2VudCIsImp0aSI6ImVkOTMzMDNiIiwicm9sZSI6InNoZWx0ZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDI1NCIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzczIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI2NyJdLCJuYmYiOjE2OTE4MTI5MjAsImV4cCI6MTY5OTc2MTcyMCwiaWF0IjoxNjkxODEyOTIwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ETEUXYOuMN1yY0GeqAw4aRcT0EfeXEBJ5lTHiJkZ_Gk" https://localhost:7267/secret
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

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
        : TypedResults.BadRequest());

app.MapGet("/shelters", async (ShelterApiViewModel viewModel) =>
    TypedResults.Ok(await viewModel.ListAll()));

app.MapGet("/shelters/{id}", async Task<Results<Ok<Shelter>, NotFound>> (string id, [FromServices] ShelterApiViewModel viewModel) =>
    await viewModel.GetById(id) 
      is Shelter shelter
        ? TypedResults.Ok(shelter)
        : TypedResults.NotFound()).WithName("GetShelterById");

app.MapDelete("/shelters/{id}", async Task<Results<NoContent, BadRequest>> (string id, [FromServices] ShelterApiViewModel viewModel) =>
    await viewModel.Delete(id)
      is true
        ? TypedResults.NoContent()
        : TypedResults.BadRequest());

// --------------------------------------------------  Shelters History -------------------------------------------------- //

app.MapGet("/shelters/{shelterId}/history/date-listed", async (string shelterId, [FromServices] ShelterHistoryApiViewModel viewModel) =>
    TypedResults.Ok(await viewModel.GetListedDate(shelterId)));

// --------------------------------------------------  Shelter Pets -------------------------------------------------- //

app.MapGet("/shelters/{shelterId}/pets", async (string shelterId, [FromServices] ShelterPetsApiViewModel viewModel) => 
    TypedResults.Ok(await viewModel.ListAllPets(shelterId)));

app.MapGet("/shelters/{shelterId}/pets/{petId}", async Task<Results<Ok<ShelteredPet>, NotFound>> (string shelterId, string petId, [FromServices] ShelterPetsApiViewModel viewModel) => 
    await viewModel.GetPetById(shelterId, petId)
        is ShelteredPet shelteredPet
        ? TypedResults.Ok(shelteredPet)
        : TypedResults.NotFound());

app.MapPost("/shelters/{shelterId}/pets", async Task<Results<Created<ShelteredPet>, BadRequest>> (string shelterId, [FromServices] ShelterPetsApiViewModel viewModel, [FromBody] ListPetModel listPetModel) => 
    await viewModel.AddPet(shelterId, listPetModel)
       is ShelteredPet petModel
         ? TypedResults.Created($"/shelters/{shelterId}/pets/{petModel.Pet.Id}", petModel)
         : TypedResults.BadRequest());

app.MapGet("/shelters/{shelterId}/pets/{petId}/history", async Task<Results<Ok<IEnumerable<ShelteredPetEvent>>, NotFound>> (string shelterId, string petId, [FromServices] ShelterPetsHistoryApiViewModel viewModel) =>
    await viewModel.GetShelteredPetHistory(shelterId, petId)
        is IEnumerable<ShelteredPetEvent> shelteredPetEvents
        && shelteredPetEvents.Count() > 0
        ? TypedResults.Ok(shelteredPetEvents)
        : TypedResults.NotFound());

app.MapPut("/shelters/{shelterId}/pets/{petId}/transfer", async Task<Results<Ok<ShelteredPetEvent>, BadRequest>> (string shelterId, string petId, [FromServices] ShelterPetsApiViewModel viewModel, [FromBody] string shelterIdTarget) => 
    await viewModel.TransferPet(shelterId, petId, shelterIdTarget)
        is ShelteredPetEvent transferEvent
        ? TypedResults.Ok(transferEvent)
        : TypedResults.BadRequest());

//app.MapGet("/shelters/{shelterId}/history", async (string shelterId, IShelterApiHandler viewModel) =>
//    TypedResults.Ok(await viewModel.GetHistoryById(shelterId))).WithName("GetShelterHistoryById");

//app.MapGet("/shelters/{shelterId}/history/{eventKind:int}", async (string shelterId, int eventKind, IShelterApiHandler viewModel) =>
//    TypedResults.Ok(await viewModel.GetHistoryEventTypeById(shelterId, eventKind))).WithName("GetShelterHistoryByIdAndEvent");

//app.MapGet("/shelters/{shelterId}/events/dateListed", async (string shelterId, IShelterApiHandler viewModel) =>
//    TypedResults.Ok(await viewModel.GetListedDate(shelterId))).WithName("GetShelterListedDate");

//app.MapGet("shelters/{shelterId}/history/{eventKind}", () => { });

//// get sheltered pets
//app.MapGet("shelters/{shelterId}/pets", () => { });
//// get sheltered pet details
//app.MapGet("shelters/{shelterId}/pets/{petId}", () => { });
//// get sheltered pet history
//app.MapGet("shelters/{shelterId}/pets/{petId}/history", () => { });
//// get sheltered pet history by event kind
//app.MapGet("shelters/{shelterId}/pets/{petId}/history/{eventKind}", () => { });

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
//app.MapGet("adopterpersons/{adopterPersonId}/pets/{petId}", () => { });
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
public partial class Program { }



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