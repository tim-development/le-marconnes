using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeMarconnes.Tests.Controllers;

public class BedsControllerTests : ControllerTestBase
{
    [Fact]
    public async Task GetBeds_Returns_List()
    {
        // Arrange
        var context = DbContextHelper.Create(nameof(GetBeds_Returns_List));

        context.Beds.Add(new Bed { Type = "Single", numberOfPeople = 1 });
        context.Beds.Add(new Bed { Type = "Double", numberOfPeople = 2 });
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new BedsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        // Act
        var result = await controller.GetBeds();

        // Assert
        var list = Assert.IsAssignableFrom<IEnumerable<Bed>>(result.Value);
        Assert.Equal(2, list.Count());
    }

    [Fact]
    public async Task GetBedById_Returns_Bed()
    {
        var context = DbContextHelper.Create(nameof(GetBedById_Returns_Bed));
        var bed = new Bed { Type = "Single", numberOfPeople = 1 };
        context.Beds.Add(bed);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new BedsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetBed(bed.Id);

        var returnedBed = Assert.IsType<Bed>(result.Value);
        Assert.Equal(bed.Id, returnedBed.Id);
        Assert.Equal("Single", returnedBed.Type);
    }

    [Fact]
    public async Task PostBed_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(PostBed_Allows_Admin));
        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new BedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var newBed = new Bed { Type = "Queen", numberOfPeople = 2 };

        var result = await controller.PostBed(newBed);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        var bed = Assert.IsType<Bed>(createdAt.Value);
        Assert.Equal("Queen", bed.Type);
        Assert.Single(context.Beds);
    }

    [Fact]
    public async Task PutBed_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(PutBed_Allows_Admin));
        var bed = new Bed { Type = "Single", numberOfPeople = 1 };
        context.Beds.Add(bed);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new BedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var updatedBed = new Bed { Id = bed.Id, Type = "Double", numberOfPeople = 2 };
        var result = await controller.PutBed(bed.Id, updatedBed);

        Assert.IsType<NoContentResult>(result);
        var bedFromDb = context.Beds.First();
        Assert.Equal("Double", bedFromDb.Type);
        Assert.Equal(2, bedFromDb.numberOfPeople);
    }

    [Fact]
    public async Task DeleteBed_Removes_When_Admin()
    {
        var context = DbContextHelper.Create(nameof(DeleteBed_Removes_When_Admin));
        var bed = new Bed { Type = "Single", numberOfPeople = 1 };
        context.Beds.Add(bed);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new BedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var result = await controller.DeleteBed(bed.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Beds);
    }
}
