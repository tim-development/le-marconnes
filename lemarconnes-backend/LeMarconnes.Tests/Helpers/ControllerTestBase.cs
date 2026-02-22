using LeMarconnes.Entities;
using LeMarconnes.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LeMarconnes.Tests.Helpers;

public abstract class ControllerTestBase
{
    // Maak een UserManager mock
    protected Mock<UserManager<User>> CreateUserManagerMock()
    {
        return UserManagerMock.Create();
    }

    // Maak een admin user + configureer UserManager mock
    protected (User adminUser, Mock<UserManager<User>> userManagerMock) CreateAdminUserWithMock(string id = "admin-id")
    {
        var adminUser = new User { Id = id };
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.FindByIdAsync(adminUser.Id)).ReturnsAsync(adminUser);
        userManagerMock.Setup(x => x.GetRolesAsync(adminUser)).ReturnsAsync(new[] { "Admin" });

        return (adminUser, userManagerMock);
    }

    // Maak een normale user + configureer UserManager mock
    protected (User normalUser, Mock<UserManager<User>> userManagerMock) CreateNormalUserWithMock(string id = "user-id")
    {
        var normalUser = new User { Id = id };
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.FindByIdAsync(normalUser.Id)).ReturnsAsync(normalUser);
        userManagerMock.Setup(x => x.GetRolesAsync(normalUser)).ReturnsAsync(Array.Empty<string>());

        return (normalUser, userManagerMock);
    }

    // Claims attachen aan controller
    protected void AttachAdminUserToController(ControllerBase controller, string id = "admin-id")
    {
        ControllerTestHelper.AttachAdminUser(controller);
    }

    protected void AttachNormalUserToController(ControllerBase controller, string id = "user-id")
    {
        ControllerTestHelper.AttachNormalUser(controller);
    }
}
