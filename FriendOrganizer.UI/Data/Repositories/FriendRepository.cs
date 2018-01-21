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
    public class FriendRepository : IFriendRepository
    { 
        private FriendOrganizerDbContext _context;

        public FriendRepository(FriendOrganizerDbContext context )
        {
            _context = context;
        }

        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
        }

        public async Task<Friend> GetByIdAsync(int friendid)
        {
            
                var friends = await _context.Friends.SingleAsync(f => f.Id == friendid);
                //await Task.Delay(5000); Test to workin asyncronus programming
                return friends;

        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public async Task SaveAsync()
        { 
                await   _context.SaveChangesAsync();  
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
