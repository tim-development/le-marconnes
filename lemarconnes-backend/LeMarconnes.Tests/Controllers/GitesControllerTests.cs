using LeMarconnes.DTOs;
using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeMarconnes.Tests.Controllers;

public class GitesControllerTests : ControllerTestBase
{
    [Fact]
    public async Task GetGites_Returns_List()
    {
        var context = DbContextHelper.Create(nameof(GetGites_Returns_List));

        context.Gites.Add(new Gite
        {
            Name = "Test Gite",
            Description = "Test beschrijving",
            Capacity = 4,
            RatePerNight = 150
        });
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new GitesController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetGites();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetGiteById_Returns_Gite()
    {
        var context = DbContextHelper.Create(nameof(GetGiteById_Returns_Gite));
        var gite = new Gite
        {
            Name = "Gite 1",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100
        };
        context.Gites.Add(gite);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new GitesController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetGiteById(gite.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<GiteDto>(okResult.Value);
        Assert.Equal(gite.Name, dto.Name);
    }

    [Fact]
    public async Task AddGite_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(AddGite_Allows_Admin));
        var (adminUser, userManagerMock) = CreateAdminUserWithMock();

        var controller = new GitesController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var newGite = new Gite
        {
            Name = "Nieuwe Gite",
            Description = "Beschrijving",
            Capacity = 3,
            RatePerNight = 120
        };

        var result = await controller.AddGite(newGite);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<GiteDto>(okResult.Value);
        Assert.Equal(newGite.Name, dto.Name);
        Assert.Single(context.Gites);
    }

    [Fact]
    public async Task UpdateGite_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(UpdateGite_Allows_Admin));
        var gite = new Gite
        {
            Name = "Oude Gite",
            Description = "Oude beschrijving",
            Capacity = 2,
            RatePerNight = 100
        };
        context.Gites.Add(gite);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new GitesController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var updatedGite = new Gite
        {
            Name = "Nieuwe Gite",
            Description = "Nieuwe beschrijving",
            Capacity = 4,
            RatePerNight = 150,
            EntireProperty = true,
            Garden = true,
            ParkingAvailable = true
        };

        var result = await controller.UpdateGite(gite.Id, updatedGite);

        Assert.IsType<NoContentResult>(result);

        var giteFromDb = context.Gites.First();
        Assert.Equal("Nieuwe Gite", giteFromDb.Name);
        Assert.True(giteFromDb.EntireProperty);
        Assert.True(giteFromDb.Garden);
        Assert.True(giteFromDb.ParkingAvailable);
    }

    [Fact]
    public async Task DeleteGite_Removes_When_Admin()
    {
        var context = DbContextHelper.Create(nameof(DeleteGite_Removes_When_Admin));
        var gite = new Gite
        {
            Name = "Te verwijderen Gite",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100
        };
        context.Gites.Add(gite);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new GitesController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var result = await controller.DeleteGite(gite.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Gites);
    }
}
