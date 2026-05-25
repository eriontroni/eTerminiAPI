using eTerminiAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eTerminiAPI.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Tenants.AnyAsync())
        {
            db.Tenants.AddRange(
                new Tenant
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Komuna e Prishtinës",
                    Slug = "prishtina",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Tenant
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Komuna e Prizrenit",
                    Slug = "prizreni",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Tenant
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Komuna e Gjilanit",
                    Slug = "gjilani",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
