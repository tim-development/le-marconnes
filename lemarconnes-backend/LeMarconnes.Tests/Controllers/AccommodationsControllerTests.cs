using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LeMarconnes.Tests.Controllers;

public class AccommodationsControllerTests : ControllerTestBase
{
    [Fact]
    public async Task GetAccommodations_Returns_List()
    {
        // Arrange
        var context = DbContextHelper.Create(nameof(GetAccommodations_Returns_List));

        context.Accommodations.Add(new Accommodation
        {
            Name = "Test accommodatie",
            Description = "Test.",
            Capacity = 2,
            RatePerNight = 100
        });

        await context.SaveChangesAsync();

        // Gebruik één regel voor normale user setup
        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new AccommodationsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        // Act
        var result = await controller.GetAccommodations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task DeleteAccommodation_Removes_WhenAdmin()
    {
        // Arrange
        var context = DbContextHelper.Create(nameof(DeleteAccommodation_Removes_WhenAdmin));

        var acc = new Accommodation
        {
            Name = "Delete me",
            Description = "To be deleted.",
            Capacity = 1,
            RatePerNight = 50
        };

        context.Accommodations.Add(acc);
        await context.SaveChangesAsync();

        // Gebruik één regel voor admin user setup
        var (_, userManagerMock) = CreateAdminUserWithMock();
        var controller = new AccommodationsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        // Act
        var result = await controller.DeleteAccommodation(acc.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Accommodations);
    }
}
