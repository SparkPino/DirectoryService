using System.Net;
using System.Net.Http.Json;
using DirectoryService.Contracts.Department;

namespace DirectoryService.IntegrationTests.DepartmentsTest;

public class BulkDepartmentsValidationTest : DirectoryBaseTests
{
    private readonly HttpClient _client;

    public BulkDepartmentsValidationTest(DirectoryServiceWebFactory factory)
        : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task Empty_List_Should_Return_400(string action)
    {
        var response = await PostAsync(action, []);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task Body_Without_DepartmentIds_Should_Return_400(string action)
    {
        var response = await _client.PostAsJsonAsync($"/api/departments/bulk/{action}", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task More_Than_100_Ids_Should_Return_400(string action)
    {
        var response = await PostAsync(action, CreateIds(101));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task Exactly_100_Ids_Should_Pass_Validation(string action)
    {
        // Записей нет, поэтому ждём 409 от хендлера, а не 400 от валидатора
        var response = await PostAsync(action, CreateIds(100));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task Empty_Guid_Should_Return_400(string action)
    {
        var response = await PostAsync(action, [Guid.NewGuid(), Guid.Empty]);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("soft-delete")]
    [InlineData("activate")]
    [InlineData("deactivate")]
    public async Task Duplicate_Ids_Should_Return_400(string action)
    {
        var id = Guid.NewGuid();

        var response = await PostAsync(action, [id, id]);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static List<Guid> CreateIds(int count) =>
        Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();

    private Task<HttpResponseMessage> PostAsync(string action, List<Guid> departmentIds) =>
        _client.PostAsJsonAsync($"/api/departments/bulk/{action}", new BulkDepartmentsDto(departmentIds));
}
