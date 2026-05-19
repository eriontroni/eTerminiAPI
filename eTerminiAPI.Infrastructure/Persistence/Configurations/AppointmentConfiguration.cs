using eTerminiAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTerminiAPI.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasOne(a => a.User)
               .WithMany(u => u.Appointments)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TimeSlot)
               .WithOne(t => t.Appointment)
               .HasForeignKey<Appointment>(a => a.TimeSlotId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
