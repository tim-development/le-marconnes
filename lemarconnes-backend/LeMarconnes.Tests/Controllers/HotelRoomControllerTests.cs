using LeMarconnes.DTOs;
using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using LeMarconnesAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeMarconnes.Tests.Controllers;

public class HotelRoomsControllerTests : ControllerTestBase
{
    [Fact]
    public async Task GetHotelRooms_Returns_List()
    {
        var context = DbContextHelper.Create(nameof(GetHotelRooms_Returns_List));

        context.HotelRooms.Add(new HotelRoom
        {
            Name = "Test Room",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100,
            RoomNumber = 101,
            PrivateBathroom = true
        });
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new HotelRoomsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetHotelRooms();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task GetHotelRoomById_Returns_Room()
    {
        var context = DbContextHelper.Create(nameof(GetHotelRoomById_Returns_Room));
        var room = new HotelRoom
        {
            Name = "Room 1",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100,
            RoomNumber = 101,
            PrivateBathroom = false
        };
        context.HotelRooms.Add(room);
        await context.SaveChangesAsync();

        var (_, userManagerMock) = CreateNormalUserWithMock();
        var controller = new HotelRoomsController(context, userManagerMock.Object);
        AttachNormalUserToController(controller);

        var result = await controller.GetHotelRoomById(room.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<HotelRoomDto>(okResult.Value);
        Assert.Equal(room.Name, dto.Name);
    }

    [Fact]
    public async Task AddHotelRoom_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(AddHotelRoom_Allows_Admin));
        var (adminUser, userManagerMock) = CreateAdminUserWithMock();

        var controller = new HotelRoomsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var newRoom = new HotelRoom
        {
            Name = "Nieuwe Room",
            Description = "Beschrijving",
            Capacity = 3,
            RatePerNight = 150,
            RoomNumber = 102,
            PrivateBathroom = true
        };

        var result = await controller.AddHotelRoom(newRoom);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<HotelRoomDto>(okResult.Value);
        Assert.Equal(newRoom.Name, dto.Name);
        Assert.Single(context.HotelRooms);
    }

    [Fact]
    public async Task UpdateHotelRoom_Allows_Admin()
    {
        var context = DbContextHelper.Create(nameof(UpdateHotelRoom_Allows_Admin));
        var room = new HotelRoom
        {
            Name = "Oude Room",
            Description = "Oude beschrijving",
            Capacity = 2,
            RatePerNight = 100,
            RoomNumber = 101,
            PrivateBathroom = false
        };
        context.HotelRooms.Add(room);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new HotelRoomsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var updatedRoom = new HotelRoom
        {
            Name = "Nieuwe Room",
            Description = "Nieuwe beschrijving",
            Capacity = 4,
            RatePerNight = 200,
            RoomNumber = 202,
            PrivateBathroom = true
        };

        var result = await controller.UpdateHotelRoom(room.Id, updatedRoom);

        Assert.IsType<NoContentResult>(result);

        var roomFromDb = context.HotelRooms.First();
        Assert.Equal("Nieuwe Room", roomFromDb.Name);
        Assert.Equal(202, roomFromDb.RoomNumber);
        Assert.True(roomFromDb.PrivateBathroom);
    }

    [Fact]
    public async Task DeleteHotelRoom_Removes_When_Admin()
    {
        var context = DbContextHelper.Create(nameof(DeleteHotelRoom_Removes_When_Admin));
        var room = new HotelRoom
        {
            Name = "Te verwijderen Room",
            Description = "Beschrijving",
            Capacity = 2,
            RatePerNight = 100,
            RoomNumber = 101,
            PrivateBathroom = false
        };
        context.HotelRooms.Add(room);
        await context.SaveChangesAsync();

        var (adminUser, userManagerMock) = CreateAdminUserWithMock();
        var controller = new HotelRoomsController(context, userManagerMock.Object);
        AttachAdminUserToController(controller);

        var result = await controller.DeleteHotelRoom(room.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.HotelRooms);
    }
}
