using LeMarconnes.Data;
using LeMarconnes.DTOs;
using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LeMarconnes.Tests.Controllers;

public class AccommodationBedsControllerTests : ControllerTestBase
{
    private async Task<(ApplicationDbContext, Bed, Accommodation)> SeedAccommodationAndBed(string dbName)
    {
        var context = DbContextHelper.Create(dbName);

        // Maak een geldig Bed
        var bed = new Bed
        {
            Type = "Single",
            numberOfPeople = 1
        };
        context.Beds.Add(bed);

        // Maak een geldig Gite met alle vereiste velden
        var accommodation = new Gite
        {
            Name = "Test Gite",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100,
            IsGite = true,
            IsHotelRoom = false
        };
        context.Gites.Add(accommodation);

        await context.SaveChangesAsync();

        return (context, bed, accommodation);
    }

    [Fact]
    public async Task GetAccommodationBeds_Returns_List()
    {
        var (context, bed, accommodation) = await SeedAccommodationAndBed(nameof(GetAccommodationBeds_Returns_List));

        var accBed = new AccommodationBed
        {
            AccommodationId = accommodation.Id,
            BedId = bed.Id,
            Quantity = 2
        };
        context.AccommodationBeds.Add(accBed);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new AccommodationBedsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetAccommodationBeds();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task AddAccommodationBed_Allows_Admin()
    {
        var (context, bed, accommodation) = await SeedAccommodationAndBed(nameof(AddAccommodationBed_Allows_Admin));

        var (_, userManagerMock) = CreateAdminUserWithMock();
        var controller = new AccommodationBedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var newAccBed = new AccommodationBed
        {
            AccommodationId = accommodation.Id,
            BedId = bed.Id,
            Quantity = 3
        };

        var result = await controller.AddAccommodationBed(newAccBed);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<AccommodationBedDto>(okResult.Value);

        Assert.Equal(3, dto.Quantity);
        Assert.Single(context.AccommodationBeds);
    }

    [Fact]
    public async Task UpdateAccommodationBed_Allows_Admin()
    {
        var (context, bed1, accommodation) = await SeedAccommodationAndBed(nameof(UpdateAccommodationBed_Allows_Admin));

        var bed2 = new Bed { Type = "Double", numberOfPeople = 2 };
        context.Beds.Add(bed2);
        await context.SaveChangesAsync();

        var accBed = new AccommodationBed
        {
            AccommodationId = accommodation.Id,
            BedId = bed1.Id,
            Quantity = 1
        };
        context.AccommodationBeds.Add(accBed);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateAdminUserWithMock();
        var controller = new AccommodationBedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var updatedAccBed = new AccommodationBed
        {
            AccommodationId = accommodation.Id,
            BedId = bed2.Id,
            Quantity = 4
        };

        var result = await controller.UpdateAccommodationBed(accBed.Id, updatedAccBed);

        Assert.IsType<NoContentResult>(result);

        var bedFromDb = context.AccommodationBeds.First();
        Assert.Equal(4, bedFromDb.Quantity);
        Assert.Equal(bed2.Id, bedFromDb.BedId);
    }

    [Fact]
    public async Task DeleteAccommodationBed_Removes_When_Admin()
    {
        var (context, bed, accommodation) = await SeedAccommodationAndBed(nameof(DeleteAccommodationBed_Removes_When_Admin));

        var accBed = new AccommodationBed
        {
            AccommodationId = accommodation.Id,
            BedId = bed.Id,
            Quantity = 1
        };
        context.AccommodationBeds.Add(accBed);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateAdminUserWithMock();
        var controller = new AccommodationBedsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var result = await controller.DeleteAccommodationBed(accBed.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.AccommodationBeds);
    }
}
