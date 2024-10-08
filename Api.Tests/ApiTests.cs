using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using Core.Interface.Models;
using Core.Interface.Events;
using Core.Interface;

namespace Api.Tests;

public class ApiTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> webFactory;
    private readonly HttpClient httpClient;
    private const string AuthorizationHeader = "Bearer";
    private const string DeveloperBearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0uVmluY2VudCIsInN1YiI6IkFkYW0uVmluY2VudCIsImp0aSI6ImVkOTMzMDNiIiwicm9sZSI6InNoZWx0ZXIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDI1NCIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzczIiwiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI2NyJdLCJuYmYiOjE2OTE4MTI5MjAsImV4cCI6MTY5OTc2MTcyMCwiaWF0IjoxNjkxODEyOTIwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.ETEUXYOuMN1yY0GeqAw4aRcT0EfeXEBJ5lTHiJkZ_Gk";

    public ApiTests(TestWebApplicationFactory<Program> factory)
    {
        this.webFactory = factory;
        this.httpClient = factory.CreateClient();
        //this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationHeader, DeveloperBearerToken);
    }

    /// <summary>
    /// Thoughts:
    /// For the API tests, we are testing for 
    /// - Data Format
    /// - Proper exception handling by checking
    ///   - Status Codes
    ///   - Response messages & format
    /// - API identity and access  
    /// Every thing else is unit tested!
    /// </summary>
    /// <returns></returns>
    /// 

    [Fact]
    public async Task ListShelters_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterListResponse = await httpClient.GetAsync("/shelters");
        shelterListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateShelter_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        shelterCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetShelter_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var shelterGetResponse = await httpClient.GetAsync($"/shelters/{shelterId}");

        shelterGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetShelter_ShouldRetunNotFound()
    {
        using var scope = webFactory.Services.CreateScope();
        ShelterIdentity shelterId = new(Ulid.Empty);

        var shelterGetResponse = await httpClient.GetAsync($"/shelters/{shelterId}");

        shelterGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteShelter_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var shelterDeleteResponse = await httpClient.DeleteAsync($"/shelters/{shelterId}");

        shelterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ShelterHistoryDateListed_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        ShelterEvent? shelterDateListedResponse = await httpClient.GetFromJsonAsync<ShelterEvent>($"/shelters/{shelterId}/history/date-listed");
        
        shelterDateListedResponse!.ShelterEventKind.Should().Be(ShelterEventKind.ShelterListed);
    }

    [Fact]
    public async Task ShelterHistoryDateListed_ShouldReturnNotFound()
    {
        using var scope = webFactory.Services.CreateScope();
        ShelterIdentity shelterId = new(Ulid.Empty);

        var shelterDateListedResponse = await httpClient.GetAsync($"/shelters/{shelterId}/history/date-listed");
        shelterDateListedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNotEmptyShelter_ShouldFail()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;
        var listPetResponse = await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Sandy"));

        var shelterDeleteResponse = await httpClient.DeleteAsync($"/shelters/{shelterId}");

        shelterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetShelterdPets_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var listPetResponse = await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Sandy"));
        var petResponseContent = await listPetResponse.Content.ReadFromJsonAsync<ShelteredPet>();
        var petId = petResponseContent?.Pet.Id.Id;

        ShelteredPet? sut = await httpClient.GetFromJsonAsync<ShelteredPet>($"/shelters/{shelterId}/pets/{petId}");
        //sut.ShelterIdentity.Should().Be(shelterId);
        sut!.Pet.PetDetails.Name.Should().Be("Sandy");
    }

    [Fact]
    public async Task GetShelter_ShouldReturn404()
    {
        using var scope = webFactory.Services.CreateScope();
        var shelterId = Ulid.Empty.ToString();

        var sut = async () => await httpClient.GetFromJsonAsync<Shelter>($"/shelters/{shelterId}");

        await sut.Should().ThrowAsync<HttpRequestException>().Where(ex => ex.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetShelterdPets_ShouldReturn404()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var petId = Ulid.Empty.ToString();

        var sut = async () => await httpClient.GetFromJsonAsync<ShelteredPet>($"/shelters/{shelterId}/pets/{petId}");
        await sut.Should().ThrowAsync<HttpRequestException>().Where(ex => ex.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetShelterdPets2_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Sandy"));
        await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Molly"));

        IEnumerable<ShelteredPet>? sut = await httpClient.GetFromJsonAsync<IEnumerable<ShelteredPet>>($"/shelters/{shelterId}/pets");
        //sut.ShelterIdentity.Should().Be(shelterId);
        sut!.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetShelterdPetHistory_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var listPetResponse = await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Sandy"));
        var petResponseContent = await listPetResponse.Content.ReadFromJsonAsync<ShelteredPet>();
        var petId = petResponseContent?.Pet.Id.Id;

        IEnumerable<ShelteredPetEvent>? sut = await httpClient.GetFromJsonAsync<IEnumerable<ShelteredPetEvent>>($"/shelters/{shelterId}/pets/{petId}/history");
        sut!.Count().Should().Be(1);
        sut!.First().PetEventKind.Should().Be(PetEventKind.ListedAtShelter);
    }


    [Fact]
    public async Task GetShelterdPetHistory_ShouldReturn404()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var petId = Ulid.Empty.ToString();

        var sut = async () => await httpClient.GetFromJsonAsync<ShelteredPetEvent>($"/shelters/{shelterId}/pets/{petId}/history");
        await sut.Should().ThrowAsync<HttpRequestException>().Where(ex => ex.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PetTransfer_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse1 = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var shelterCreateResponse2 = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterB"));
        
        var content1 = await shelterCreateResponse1.Content.ReadFromJsonAsync<Shelter>();
        var content2 = await shelterCreateResponse2.Content.ReadFromJsonAsync<Shelter>();

        var shelterIdOrigin = content1?.Id.Id;
        var shelterIdTarget = content2?.Id.Id;

        var listPetResponse = await httpClient.PostAsJsonAsync($"/shelters/{shelterIdOrigin}/pets", new ListPetModel("Sandy"));
        var petResponseContent = await listPetResponse.Content.ReadFromJsonAsync<ShelteredPet>();
        Ulid petId = petResponseContent!.Pet.Id.Id;

        var sut = await httpClient.PutAsJsonAsync<string>($"/shelters/{shelterIdOrigin}/pets/{petId}/transfer", shelterIdTarget.ToString());
        sut.StatusCode.Should().Be(HttpStatusCode.OK);
        var sutContent = await sut.Content.ReadFromJsonAsync<ShelteredPetEvent>();
        sutContent!.PetIdentity.Id.Should().Be(petId);
        sutContent.PetEventKind.Should().Be(PetEventKind.TransferredToAnotherShelter);
    }

    //// Shelter create only takes in a name
    //// A shelter cannot be duplicated using the create API
    //[Fact]
    //public async Task CreateShelter_ShouldThrow_WhenCreatingDuplicaShelter()
    //{
    //    using var scope = webFactory.Services.CreateScope();
    //    var response1 = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterB"));
    //    var response2 = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterB"));

    //    response1.StatusCode.Should().Be(HttpStatusCode.OpenToAdopt);
    //    response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //}b6rt 'l

    [Fact]
    public async Task ShelterListPet_ShouldSucceed()
    {
        using var scope = webFactory.Services.CreateScope();

        var shelterCreateResponse = await httpClient.PostAsJsonAsync("/shelters", new ShelterModel("ShelterA"));
        var content = await shelterCreateResponse.Content.ReadFromJsonAsync<Shelter>();
        var shelterId = content?.Id.Id;

        var listPetResponse = await httpClient.PostAsJsonAsync($"/shelters/{shelterId}/pets", new ListPetModel("Sandy"));
        listPetResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    //[Fact]
    //public async Task ShelterListPet_ShouldReturnPetObject()
    //{
    //    using var scope = webFactory.Services.CreateScope();

    //    _ = await httpClient.PostAsJsonAsync("/shelters/create", new ShelterModel("ShelterD"));
    //    var listPetResponse = await httpClient.PostAsJsonAsync("/shelters/listpet", new AddPet("Sandy", "ShelterD"));
    //    var listPet = await listPetResponse.Content.ReadFromJsonAsync<ShelteredPet>();

    //    listPetResponse.StatusCode.Should().Be(HttpStatusCode.OpenToAdopt);
    //    listPet.Pet.Name.Name.Should().Be("Sandy");
    //    //listPet.Shelter.Name.Should().Be("ShelterD");
    //}

    //[Fact]
    //public async Task ShelterListPet_ShouldThrow_WhenShelterDoesNotExist()
    //{
    //    using var scope = webFactory.Services.CreateScope();

    //    var listPetResponse = await httpClient.PostAsJsonAsync("/shelters/listpet", new AddPet("Sandy", "NON-EXISTANT"));

    //    listPetResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //}

    //[Fact]
    //public async Task ShelterTransferPet_ShouldSucceed()
    //{
    //    using var scope = webFactory.Services.CreateScope();

    //    _ = await httpClient.PostAsJsonAsync("/shelters/create", new ShelterModel("ShelterE"));
    //    _ = await httpClient.PostAsJsonAsync("/shelters/create", new ShelterModel("ShelterF"));
    //    var listPetResponse = await httpClient.PostAsJsonAsync("/shelters/listpet", new AddPet("Sandy", "ShelterE"));
    //    var listPet = await listPetResponse.Content.ReadFromJsonAsync<ShelteredPet>();
    //    var transferRequest = new TransferPetByIdToShelterName(listPet.Pet.Id.Id, "ShelterF");

    //    var response1 = await httpClient.PostAsJsonAsync("/shelters/transfer", transferRequest);

    //    response1.StatusCode.Should().Be(HttpStatusCode.OK);
    //}
}