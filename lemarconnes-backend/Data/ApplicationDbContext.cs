using LeMarconnes.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeMarconnes.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Accommodation> Accommodations => Set<Accommodation>();
        public DbSet<HotelRoom> HotelRooms => Set<HotelRoom>();
        public DbSet<Gite> Gites => Set<Gite>();
        public DbSet<Bed> Beds => Set<Bed>();
        public DbSet<AccommodationBed> AccommodationBeds => Set<AccommodationBed>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Decimal precisie
            builder.Entity<Accommodation>().Property(a => a.RatePerNight).HasPrecision(18, 2);
            builder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            builder.Entity<Reservation>().Property(r => r.Discount).HasPrecision(18, 2);
            builder.Entity<Reservation>().Property(r => r.TouristTax).HasPrecision(18, 2);

            // Accommodation inheritance (TPH)
            builder.Entity<Accommodation>()
                .ToTable("Accommodations")
                .HasDiscriminator<string>("AccommodationType")
                .HasValue<HotelRoom>("HotelRoom")
                .HasValue<Gite>("Gite");

            // Many-to-Many: Accommodation <-> Bed via AccommodationBed
            builder.Entity<AccommodationBed>()
                .HasKey(ab => ab.Id);

            builder.Entity<AccommodationBed>()
                .HasOne(ab => ab.Accommodation)
                .WithMany(a => a.AccommodationBeds)
                .HasForeignKey(ab => ab.AccommodationId);

            builder.Entity<AccommodationBed>()
                .HasOne(ab => ab.Bed)
                .WithMany(b => b.AccommodationBeds)
                .HasForeignKey(ab => ab.BedId);

            // User -> Reservations
            builder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation -> Payments
            builder.Entity<Reservation>()
                .HasMany(r => r.Payments)
                .WithOne(p => p.Reservation)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation -> Accommodation
            builder.Entity<Reservation>()
                .HasOne(r => r.Accommodation)
                .WithMany(a => a.Reservations)
                .HasForeignKey(r => r.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}