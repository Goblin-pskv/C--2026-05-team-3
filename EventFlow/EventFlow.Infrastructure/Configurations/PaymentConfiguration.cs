using EventFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(t => t.Id);

            // Связь User -> Payments (один ко многим)
            builder.HasOne(t => t.User)
                   .WithMany(u => u.Payments)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Связь ticket -> Payments (один к одному)
            builder.HasOne(t => t.Ticket)
                   .WithMany(e => e.Payments)
                   .HasForeignKey(t => t.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
