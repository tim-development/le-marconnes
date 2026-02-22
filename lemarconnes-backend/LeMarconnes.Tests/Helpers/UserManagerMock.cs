using LeMarconnes.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LeMarconnes.Tests.Helpers;

public static class UserManagerMock
{
    public static Mock<UserManager<User>> Create()
    {
        var store = new Mock<IUserStore<User>>();

        return new Mock<UserManager<User>>(
            store.Object,
            null, null, null, null, null, null, null, null
        );
    }
}
