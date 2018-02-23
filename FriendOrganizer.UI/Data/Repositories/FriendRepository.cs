using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
//Test Github // test v2
namespace FriendOrganizer.UI.Data.Repositories
{


    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>, IFriendRepository
    {

        public FriendRepository(FriendOrganizerDbContext context)
            : base(context)
        {
        }



        public override async Task<Friend> GetByIdAsync(int friendid)
        {

            var friends = await Context.Friends
                .Include(f => f.PhoneNumbers)
                .SingleAsync(f => f.Id == friendid);
            //await Task.Delay(5000); Test to workin asyncronus programming
            return friends;

        }

        public async Task<bool> HasMeetingsAsync(int friendId)
        {
            return await Context.Meetings.AsNoTracking()
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }

        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }


        /*
        public async Task<List<Friend>> GetAllAsync()
        {

           using(var ctx = _contextCreator())
           {
               var friends =  await ctx.Friends.AsNoTracking().ToListAsync();
               //await Task.Delay(5000); Test to workin asyncronus programming
               return friends;
           }
           //ToDo  load data from real DB: 
           //yield return new Friend { FirstName = "Karen", LastName = "Barseghyan" };
           //yield return new Friend { FirstName = "Raf", LastName = "Grigorian" };
           //yield return new Friend { FirstName = "Henrik", LastName = "Mghitaryan" };
           //yield return new Friend { FirstName = "Suren", LastName = "Hakobyan" };

        } */
    }
}
