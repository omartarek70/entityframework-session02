using EventHub.Data;
using EventHub.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore_2_ConsoleApp7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new EventHubContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully.\n");

    
            var organizer = new Organizer
            {
                Name = "Omar Tarek",
                CompanyName = "TechTalks Co.",
                IsVerified = true,
                Profile = new OrganizerProfile
                {
                    Biography = "We organize tech meetups across Egypt.",
                    WebsiteUrl = "https://techtalks.example.com",
                    LogoUrl = "https://techtalks.example.com/logo.png"
                }
            };
            organizer.Events.Add(new Event
            {
                Title = ".NET Meetup",
                Description = "A full day about .NET and EF Core.",
                StartDate = new DateTime(2026, 12, 20, 10, 0, 0),
                EndDate = new DateTime(2026, 12, 20, 18, 0, 0),
                MaxAttendees = 150
            });

            var attendee = new Attendee
            {
                FullName = "Sara Ali",
                Email = "sara@example.com",
                Street = "12 Nile St",
                City = "Beni Suef",
                Country = "Egypt",
                PostalCode = "62511",
                Badge = new Badge { BadgeNumber = "BDG-0001", Tier = BadgeTier.VIP }
            };

            context.Organizers.Add(organizer);
            context.Attendees.Add(attendee);
            context.SaveChanges();

            var a = context.Attendees.Include(x => x.Badge).First();
            Console.WriteLine($"Attendee: {a.FullName} | {a.FullAddress}");
            Console.WriteLine($"Badge: {a.Badge!.BadgeNumber} | {a.Badge.Tier} | issued {a.Badge.IssuedDate:yyyy-MM-dd}\n");

            var o = context.Organizers.Include(x => x.Profile).Include(x => x.Events).First();
            Console.WriteLine($"Organizer: {o.Name} ({o.CompanyName}) | verified: {o.IsVerified} | events: {o.Events.Count} | profile: {o.Profile!.WebsiteUrl}\n");

            var conn = context.Database.GetDbConnection();
            conn.Open();

            Console.WriteLine("===== Columns =====");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
        SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE,
               ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS varchar(10)), '-'), IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        ORDER BY TABLE_NAME, ORDINAL_POSITION";
                using var r = cmd.ExecuteReader();
                Console.WriteLine($"{"Table",-18}{"Column",-16}{"Type",-10}{"Length",-8}Nullable");
                Console.WriteLine(new string('-', 66));
                while (r.Read())
                    Console.WriteLine($"{r.GetString(0),-18}{r.GetString(1),-16}{r.GetString(2),-10}{r.GetString(3),-8}{r.GetString(4)}");
            }

            Console.WriteLine("\n===== Foreign Keys =====");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
        SELECT OBJECT_NAME(fk.parent_object_id), OBJECT_NAME(fk.referenced_object_id)
        FROM sys.foreign_keys fk
        ORDER BY 1";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    Console.WriteLine($"{r.GetString(0)} -> {r.GetString(1)}");
            }

            Console.WriteLine("\nNote: FullAddress is [NotMapped], so it has no column.");
        }
    }
}
