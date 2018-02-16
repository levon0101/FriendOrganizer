namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Collections.Generic;
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
                new ProgrammingLanguage { Name = "C" },
                new ProgrammingLanguage { Name = "C++" }
                );


            context.FriendPhoneNumbers.AddOrUpdate(
               pn => pn.Number,
               new FriendPhoneNumber { Number = "+374 55979969", FriendId = context.Friends.First().Id }
               );


            context.Meetings.AddOrUpdate(
                m => m.Title,
                new Meeting
                {
                    Title = "Watching Soccer",
                    DateFrom = new DateTime(2018, 5, 26),
                    DateTo = new DateTime(2018, 5, 26),
                    Friends = new List<Friend>
                    {
                        context.Friends.Single(f=>f.FirstName =="Thomas" && f.LastName == "Huber"),
                        context.Friends.Single(f=>f.FirstName =="Levon" && f.LastName == "Mardanyan")
                    }
                });

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
