using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
//Test Github // test v2
namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private Func<FriendOrganizerDbContext> _contextCreator;

        public FriendDataService(Func<FriendOrganizerDbContext> contextCreator )
        {
            _contextCreator = contextCreator;
        }
        public async Task<Friend> GetByIdAsync(int friendid)
        {

            using (var ctx = _contextCreator())
            {
                var friends = await ctx.Friends.AsNoTracking().SingleAsync(f => f.Id == friendid);
                //await Task.Delay(5000); Test to workin asyncronus programming
                return friends;
            }
            

        }

        public async Task SaveAsync(Friend friend)
        {
            using(var ctx = _contextCreator())
            {
                ctx.Friends.Attach(friend);
                ctx.Entry(friend).State = EntityState.Modified;
                await   ctx.SaveChangesAsync();
            }
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
