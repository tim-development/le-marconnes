using System.Net.Http.Json;
using LeMarconnes.Entities; 
using Xunit;

namespace LeMarconnes.Tests.Live;

public class HotelRoomsLiveTests
{
    private readonly HttpClient _client;
    private const string ApiUrl = "https://lemarconnes-api-d7hgf2emb3cebbb3.westeurope-01.azurewebsites.net/api/hotelrooms";

    public HotelRoomsLiveTests()
    {
        _client = new HttpClient();
    }

    [Fact]
    public async Task Get_AzureHotelRooms_ReturnsSuccess()
    {
        // Act: Roep de echte Azure website aan
        var response = await _client.GetAsync(ApiUrl);

        // Assert: Controleer of de site online is (Status 200 OK)
        response.EnsureSuccessStatusCode();

        // Lees de data uit om te controleren of er kamers in de database op Azure staan
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);

        // Controleer of het een JSON lijst is (begint met [)
        Assert.StartsWith("[", content.Trim());
    }
}