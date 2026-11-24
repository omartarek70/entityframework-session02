using EventHub.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHub.Configurations;

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.ToTable("Badges");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BadgeNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(b => b.BadgeNumber).IsUnique();

        builder.Property(b => b.IssuedDate)
               .HasColumnType("date")
               .HasDefaultValueSql("CAST(GETDATE() AS date)");

        builder.Property(b => b.Tier)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasOne(b => b.Attendee)
               .WithOne(a => a.Badge)
               .HasForeignKey<Badge>(b => b.AttendeeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.AttendeeId).IsUnique();
    }
}
