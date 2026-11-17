using EventFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventFlow.Infrastructure.Configurations
{
    public class OrganizerRequestConfiguration : IEntityTypeConfiguration<OrganizerRequest>
    {
        public void Configure(EntityTypeBuilder<OrganizerRequest> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Approver)
                .WithMany()
                .HasForeignKey(o => o.ReviewedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
