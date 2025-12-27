using Assessment.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Assessment.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Form> Forms => Set<Form>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Form>(e =>
        {
            e.Property(p => p.Title).HasMaxLength(200).IsRequired();
            e.Property(p => p.Currency).HasMaxLength(10).HasDefaultValue("usd");
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasMany(p => p.Submissions).WithOne(s => s.Form).HasForeignKey(s => s.FormId);
        });

        builder.Entity<Submission>(e =>
        {
            e.Property(p => p.DataJson).HasColumnType("nvarchar(max)");
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(s => s.UserId);
        });

        builder.Entity<Payment>(e =>
        {
            e.Property(p => p.Currency).HasMaxLength(10).HasDefaultValue("usd");
            e.Property(p => p.Provider).HasMaxLength(50);
            e.Property(p => p.Status).HasMaxLength(50);
            e.HasOne(p => p.Submission).WithMany(s => s.Payments).HasForeignKey(p => p.SubmissionId);
        });
    }
}