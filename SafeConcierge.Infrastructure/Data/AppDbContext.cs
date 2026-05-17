using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Condominium> Condominiums => Set<Condominium>();
    public DbSet<DeliveryCompany> DeliveryCompanies => Set<DeliveryCompany>();
    public DbSet<DeliveryLog> DeliveryLogs => Set<DeliveryLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PickupCode> PickupCodes => Set<PickupCode>();
    public DbSet<Tower> Towers => Set<Tower>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Condominium
        builder.Entity<Condominium>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.Cnpj).IsUnique();
        });

        // Tower
        builder.Entity<Tower>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.Condominium)
             .WithMany()
             .HasForeignKey(t => t.CondominiumId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Apartment
        builder.Entity<Apartment>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Tower)
             .WithMany(t => t.Apartments)
             .HasForeignKey(a => a.TowerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // User
        builder.Entity<User>(e =>
        {
            e.HasOne(u => u.Condominium)
             .WithMany()
             .HasForeignKey(u => u.CondominiumId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(u => u.Apartment)
             .WithMany(a => a.Residents)
             .HasForeignKey(u => u.ApartmentId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Package
        builder.Entity<Package>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Condominium).WithMany().HasForeignKey(p => p.CondominiumId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Apartment).WithMany().HasForeignKey(p => p.ApartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Resident).WithMany().HasForeignKey(p => p.ResidentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.ReceivedBy).WithMany().HasForeignKey(p => p.ReceivedById).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.DeliveryCompany).WithMany().HasForeignKey(p => p.DeliveryCompanyId).OnDelete(DeleteBehavior.SetNull);
        });

        // PickupCode
        builder.Entity<PickupCode>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Code).IsUnique();
            e.HasOne(p => p.Package).WithMany().HasForeignKey(p => p.PackageId).OnDelete(DeleteBehavior.Cascade);
        });

        // DeliveryLog
        builder.Entity<DeliveryLog>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasOne(d => d.Package).WithMany().HasForeignKey(d => d.PackageId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.DeliveredTo).WithMany().HasForeignKey(d => d.DeliveredToId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.DeliveredBy).WithMany().HasForeignKey(d => d.DeliveredById).OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog
        builder.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        builder.Entity<Notification>(e =>
        {
            e.HasKey(n => n.Id);
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(n => n.Package).WithMany().HasForeignKey(n => n.PackageId).OnDelete(DeleteBehavior.SetNull);
        });

        // DeliveryCompany
        builder.Entity<DeliveryCompany>(e => e.HasKey(d => d.Id));
    }
}

