namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(
                f => f.FirstName,
            new Friend { FirstName = "Karen", LastName = "Barseghyan" },
            new Friend { FirstName = "Raf", LastName = "Grigorian" },
            new Friend { FirstName = "Henrik", LastName = "Mghitaryan" },
            new Friend { FirstName = "Suren", LastName = "Hakobyan" }
            );

            context.ProgrammingLanguages.AddOrUpdate(
                f => f.Name,

                new ProgrammingLanguage { Name = "C#" },
                new ProgrammingLanguage { Name = "TypeScript" },
                new ProgrammingLanguage { Name = "F#" },
                new ProgrammingLanguage { Name = "Swift" },
                new ProgrammingLanguage { Name = "Java" },
                new ProgrammingLanguage { Name = "C"},
                new ProgrammingLanguage { Name = "C++" });
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
