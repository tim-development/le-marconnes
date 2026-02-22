using LeMarconnes.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeMarconnes.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Zorg dat de database bestaat en up-to-date is
        context.Database.EnsureCreated();

        // 2. Rollen aanmaken
        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 3. Admin Gebruiker aanmaken
        var adminEmail = "admin@lemarconnes.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin LeMarconnes",
                Address = "Route de la Montagne, St. Julien",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "LeMarconnes2026!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 4. Bedden toevoegen
        if (!context.Beds.Any())
        {
            context.Beds.AddRange(
                new Bed { Type = "Eenpersoonsbed", numberOfPeople = 1 },
                new Bed { Type = "Tweepersoonsbed", numberOfPeople = 2 },
                new Bed { Type = "Slaapbank", numberOfPeople = 2 }
            );
            await context.SaveChangesAsync();
        }

        // 5. Accommodaties en Koppelingen
        if (!context.Accommodations.Any())
        {
            var hotelRoom = new HotelRoom
            {
                Name = "Chambre Azur",
                Description = "Luxe kamer met blauwe accenten.",
                Capacity = 2,
                RatePerNight = 110.00m,
                IsHotelRoom = true,
                IsGite = false,
                RoomNumber = 101,
                PrivateBathroom = true
            };

            var gite = new Gite
            {
                Name = "Le Petit Gite",
                Description = "Gezellig huisje in de tuin.",
                Capacity = 4,
                RatePerNight = 160.00m,
                IsHotelRoom = false,
                IsGite = true,
                EntireProperty = true,
                Garden = true,
                ParkingAvailable = true
            };

            context.Accommodations.AddRange(hotelRoom, gite);
            await context.SaveChangesAsync();

            var doubleBed = await context.Beds.FirstAsync(b => b.numberOfPeople == 2 && b.Type == "Tweepersoonsbed");
            var singleBed = await context.Beds.FirstAsync(b => b.numberOfPeople == 1);

            context.AccommodationBeds.AddRange(
                new AccommodationBed { AccommodationId = hotelRoom.Id, BedId = doubleBed.Id, Quantity = 1 },
                new AccommodationBed { AccommodationId = gite.Id, BedId = doubleBed.Id, Quantity = 1 },
                new AccommodationBed { AccommodationId = gite.Id, BedId = singleBed.Id, Quantity = 2 }
            );

            await context.SaveChangesAsync();
        }

        // 6. Reserveringen toevoegen (Indien leeg)
        if (!context.Reservations.Any())
        {
            var gite = await context.Gites.FirstAsync(g => g.Name == "Le Petit Gite");

            var reservation = new Reservation
            {
                UserId = adminUser.Id, // We koppelen het aan de admin voor de test
                AccommodationId = gite.Id,
                CheckInDate = DateTime.Now.AddDays(10),
                CheckOutDate = DateTime.Now.AddDays(17),
                NumberOfGuests = 2,
                Status = ReservationStatus.Confirmed,
                TouristTax = 12.50m,
                Discount = 0.00m
            };

            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            // 7. Betalingen toevoegen (Gekoppeld aan de reservering hierboven)
            if (!context.Payments.Any())
            {
                context.Payments.AddRange(
                    new Payment
                    {
                        ReservationId = reservation.Id,
                        Amount = 100.00m, // Aanbetaling
                        PaymentDate = DateTime.Now
                    },
                    new Payment
                    {
                        ReservationId = reservation.Id,
                        Amount = 50.00m, // Tweede betaling
                        PaymentDate = DateTime.Now.AddMinutes(5)
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}