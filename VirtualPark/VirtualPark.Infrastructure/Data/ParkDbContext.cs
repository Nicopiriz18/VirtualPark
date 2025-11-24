// <copyright file="ParkDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Infrastructure.Data;

public class ParkDbContext(DbContextOptions<ParkDbContext> options) : DbContext(options)
{
    public DbSet<Attraction> Attractions { get; set; }

    public DbSet<Incidence> Incidences { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<AttractionAccess> AttractionAccesses { get; set; }

    public DbSet<Visitor> Visitors { get; set; }

    public DbSet<SpecialEvent> SpecialEvents { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<ScoreLog> ScoreLogs { get; set; }

    public DbSet<Session> Sessions { get; set; }

    public DbSet<ActiveScoringStrategy> ActiveScoringStrategies { get; set; }

    public DbSet<ClockConfiguration> ClockConfigurations { get; set; }

    public DbSet<Reward> Rewards { get; set; }

    public DbSet<PreventiveMaintenance> PreventiveMaintenances { get; set; }

    public DbSet<RewardRedemption> RewardRedemptions { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1506:Avoid excessive class coupling", Justification = "EF Core model configuration naturally requires coupling with all entity types")]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attraction>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Name).IsRequired().HasMaxLength(120);
            b.Property(a => a.Description).HasMaxLength(1000);
            b.Property(a => a.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
            b.Property(a => a.MinAge).IsRequired();
            b.Property(a => a.Capacity).IsRequired();

            b.HasIndex(a => a.Name).IsUnique();

            b.Navigation(a => a.Incidences)
                .HasField("incidences")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(a => a.Incidences)
                .WithOne(i => i.Attraction)
                .HasForeignKey(i => i.AttractionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Incidence>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.HasIndex(e => e.AttractionId);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitDate).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.QrCode).IsRequired();
            entity.HasIndex(t => t.QrCode).IsUnique();
            entity.Property<Guid?>("SpecialEventId");
            entity.HasOne<SpecialEvent>()
                .WithMany()
                .HasForeignKey("SpecialEventId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AttractionAccess>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntryTime).IsRequired();
            entity.Property(x => x.EntryMethod).HasConversion<string>().HasMaxLength(16).IsRequired();
            entity.HasOne<Attraction>().WithMany().HasForeignKey(a => a.AttractionId);
            entity.Ignore(x => x.IsClosed);

            entity.HasOne<Attraction>()
                .WithMany()
                .HasForeignKey(x => x.AttractionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Visitor>()
                .WithMany()
                .HasForeignKey(x => x.VisitorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => new { x.AttractionId, x.EntryTime });
        });
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Name).IsRequired().HasMaxLength(100);
            b.Property(u => u.Surname).IsRequired().HasMaxLength(100);
            b.Property(u => u.Email).IsRequired().HasMaxLength(320);
            b.Property(u => u.Password).IsRequired().HasMaxLength(256);

            b.HasIndex(u => u.Email).IsUnique();

            b.Property<List<RoleEnum>>("roles")
                .HasColumnName("Roles")
                .HasMaxLength(500)
                .HasConversion(
                    roles => JsonSerializer.Serialize(roles, (JsonSerializerOptions?)null),
#pragma warning disable IDE0028 // Collection initialization can be simplified
                    json => JsonSerializer.Deserialize<List<RoleEnum>>(json, (JsonSerializerOptions?)null) ?? new List<RoleEnum>());
#pragma warning restore IDE0028 // Collection initialization can be simplified

            b.HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Visitor>("Visitor");
        });
        modelBuilder.Entity<Visitor>(b =>
        {
            b.Property(v => v.BirthDate).IsRequired();
            b.Property(v => v.MembershipLevel)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();
            b.Property(v => v.NfcId).IsRequired();

            b.HasIndex(v => v.NfcId).IsUnique();

            b.HasMany(v => v.Tickets)
                .WithOne(t => t.Visitor)
                .HasForeignKey(t => t.VisitorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<SpecialEvent>(b =>
        {
            b.HasKey(se => se.Id);
            b.Property(se => se.Name).IsRequired().HasMaxLength(200);
            b.Property(e => e.Date).IsRequired();
            b.Property(e => e.MaxCapacity).IsRequired();
            b.Property(e => e.AdditionalCost).IsRequired();
            b.HasIndex(se => se.Name).IsUnique();
            b.Navigation(e => e.Attractions).HasField("attractions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(e => e.Attractions)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "SpecialEventAttractions",
                    right => right.HasOne<Attraction>()
                        .WithMany()
                        .HasForeignKey("AttractionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<SpecialEvent>()
                        .WithMany()
                        .HasForeignKey("SpecialEventId")
                        .OnDelete(DeleteBehavior.Cascade),
                    je =>
                    {
                        je.HasKey("SpecialEventId", "AttractionId");
                    });
        });

        modelBuilder.Entity<ScoreLog>(b =>
        {
            b.HasKey(sl => sl.Id);
            b.Property(sl => sl.VisitorId).IsRequired();
            b.Property(sl => sl.AttractionAccessId);
            b.Property(sl => sl.AttractionId);
            b.Property(sl => sl.PointsAwarded).IsRequired();
            b.Property(sl => sl.StrategyUsed).IsRequired().HasMaxLength(50);
            b.Property(sl => sl.AwardedAt).IsRequired();

            b.HasOne<Visitor>()
                .WithMany()
                .HasForeignKey(sl => sl.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<AttractionAccess>()
                .WithMany()
                .HasForeignKey(sl => sl.AttractionAccessId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Attraction>()
                .WithMany()
                .HasForeignKey(sl => sl.AttractionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(sl => new { sl.AwardedAt, sl.PointsAwarded });
            b.HasIndex(sl => new { sl.VisitorId, sl.AwardedAt });
        });

        modelBuilder.Entity<Session>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Token).IsRequired().HasMaxLength(500);
            b.HasIndex(s => s.Token).IsUnique();
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActiveScoringStrategy>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.StrategyName).IsRequired().HasMaxLength(50);
            b.HasData(new ActiveScoringStrategy { Id = 1, StrategyName = "ScoreByAttractionType" });
        });

        modelBuilder.Entity<ClockConfiguration>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.CustomDateTime);
            b.HasData(new ClockConfiguration { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), CustomDateTime = null });
        });
        modelBuilder.Entity<PreventiveMaintenance>(b =>
        {
            b.HasKey(pm => pm.Id);
            b.Property(pm => pm.AttractionId).IsRequired();
            b.Property(pm => pm.ScheduledDate).IsRequired();
            b.Property(pm => pm.StartTime).IsRequired();
            b.Property(pm => pm.EstimatedDuration).IsRequired();
            b.Property(pm => pm.Description).IsRequired().HasMaxLength(1000);
            b.Property(pm => pm.AssociatedIncidenceId);

            b.HasOne(pm => pm.Attraction)
                .WithMany()
                .HasForeignKey(pm => pm.AttractionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(pm => pm.AttractionId);
        });
        modelBuilder.Entity<Reward>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).IsRequired().HasMaxLength(200);
            b.Property(r => r.Description).HasMaxLength(500);
            b.Property(r => r.CostInPoints).IsRequired();
            b.Property(r => r.AvailableQuantity).IsRequired();
            b.HasIndex(r => r.Name);
        });
        modelBuilder.Entity<RewardRedemption>(entity =>
        {
            entity.HasKey(rr => rr.Id);
            entity.Property(rr => rr.VisitorId).IsRequired();
            entity.Property(rr => rr.RewardId).IsRequired();
            entity.Property(rr => rr.Date).IsRequired();
            entity.Property(rr => rr.PointsSpent).IsRequired();
        });
        this.SeedAdminUser(modelBuilder);
    }

    private void SeedAdminUser(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var sessionId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        // Usa reflexión para crear el usuario sin pasar por el constructor público
        var adminUser = (User)Activator.CreateInstance(typeof(User), true)!;

        typeof(User).GetProperty("Id")!.SetValue(adminUser, adminId);
        typeof(User).GetProperty("Name")!.SetValue(adminUser, "Admin");
        typeof(User).GetProperty("Surname")!.SetValue(adminUser, "User");
        typeof(User).GetProperty("Email")!.SetValue(adminUser, "admin@virtualpark.com");
        typeof(User).GetProperty("Password")!.SetValue(adminUser, "PBKDF2$V1$PRF=HMACSHA256$iter=120000$i4W2rUDsE+ttw0sDVH4EnA==$FtUAt4gfajbTyqy4iYwXqvc8uP8PchZlv5qPEADDJBM=");

        adminUser.AssignRole(RoleEnum.Administrator);
        adminUser.AssignRole(RoleEnum.Operator);

        modelBuilder.Entity<User>().HasData(adminUser);
        var adminSession = (Session)Activator.CreateInstance(typeof(Session), true)!;
        typeof(Session).GetProperty("Id")!.SetValue(adminSession, sessionId);
        typeof(Session).GetProperty("UserId")!.SetValue(adminSession, adminId);
        typeof(Session).GetProperty("Token")!.SetValue(adminSession, "ADMIN-DEV-TOKEN-123456789");
        modelBuilder.Entity<Session>().HasData(adminSession);
    }
}
