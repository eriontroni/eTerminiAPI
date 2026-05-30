using System.Text.Json;
using eTerminiAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTerminiAPI.Infrastructure.Persistence.Configurations;

public class AdminRoleConfiguration : IEntityTypeConfiguration<AdminRole>
{
    public void Configure(EntityTypeBuilder<AdminRole> builder)
    {
        builder.Property(r => r.Name).IsRequired().HasMaxLength(120);
        builder.Property(r => r.Description).HasMaxLength(500);

        // Ruaj listën e lejeve si kolonë JSON.
        var comparer = new ValueComparer<List<string>>(
            (a, b) => a!.SequenceEqual(b!),
            v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
            v => v.ToList());

        builder.Property(r => r.Permissions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
               .Metadata.SetValueComparer(comparer);

        builder.HasMany(r => r.Users)
               .WithOne(u => u.AdminRole)
               .HasForeignKey(u => u.AdminRoleId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
