using EventFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Infrastructure.Configurations
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(t => t.Id);

            // Связь User -> Registrations (один ко многим)
            builder.HasOne(t => t.User)
                   .WithMany(u => u.Registrations)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Связь Event -> Registrations (один ко многим)
            builder.HasOne(t => t.Event)
                   .WithMany(e => e.Registrations)
                   .HasForeignKey(t => t.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}