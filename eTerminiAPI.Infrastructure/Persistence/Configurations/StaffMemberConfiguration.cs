using eTerminiAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTerminiAPI.Infrastructure.Persistence.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.HasOne(s => s.User)
               .WithMany()
               .HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Department)
               .WithMany(d => d.StaffMembers)
               .HasForeignKey(s => s.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
