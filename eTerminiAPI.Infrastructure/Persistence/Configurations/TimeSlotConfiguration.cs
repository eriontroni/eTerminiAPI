using eTerminiAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTerminiAPI.Infrastructure.Persistence.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasOne(t => t.StaffMember)
               .WithMany(s => s.TimeSlots)
               .HasForeignKey(t => t.StaffMemberId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Service)
               .WithMany(s => s.TimeSlots)
               .HasForeignKey(t => t.ServiceId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
