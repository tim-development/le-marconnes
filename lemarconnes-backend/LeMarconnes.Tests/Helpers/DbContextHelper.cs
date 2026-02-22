using LeMarconnes.Data;
using Microsoft.EntityFrameworkCore;

namespace LeMarconnes.Tests.Helpers;

public static class DbContextHelper
{
    public static ApplicationDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
