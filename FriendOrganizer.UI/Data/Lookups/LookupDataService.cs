﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Lookups
{
    public class LookupDataService : IFriendLookupDataService, IProgrammingLanguageLookupDataService, IMeetingLookupDataService
    {
        private Func<FriendOrganizerDbContext> _contextCreator;

        public LookupDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<Lookupitem>> GetFriendLookupAsync()
        {
            using(var ctx = _contextCreator() )
            {
                return await ctx.Friends.AsNoTracking().Select(f =>
                 new Lookupitem
                 {
                     Id = f.Id,
                     DisplayMember = f.FirstName + " " + f.LastName
                 }
                 ).ToListAsync();
            }
        }

        public async Task<IEnumerable<Lookupitem>> GetProgrammingLanguageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking().Select(f =>
                 new Lookupitem
                 {
                     Id = f.Id,
                     DisplayMember = f.Name
                 }
                 ).ToListAsync();
            }
        }

        public async Task<IEnumerable<Lookupitem>> GetMeetingLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                var items =  await ctx.Meetings.AsNoTracking().Select(m =>
                 new Lookupitem
                 {
                     Id = m.Id,
                     DisplayMember = m.Title
                 }
                 ).ToListAsync();
                return items;
            }
        }

    }
}
