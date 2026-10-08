using EventFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Infrastructure.Configurations
{
    public class TicketsConfiguration : IEntityTypeConfiguration<Tickets>
    {
        public void Configure(EntityTypeBuilder<Tickets> builder)
        {
            builder.HasKey(t => t.Id);


            // Связь User -> Tickets (один ко многим)
            builder.HasOne(t => t.User)
                       .WithMany(u => u.Tickets)
                       .HasForeignKey(t => t.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

            // Связь Event -> Tickets (один ко многим)
            builder.HasOne(t => t.Event)
                       .WithMany(e => e.Tickets)
                       .HasForeignKey(t => t.EventId)
                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}