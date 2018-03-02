using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IMeetingRepository : IGenereicRepository<Meeting>
    {
        //Task<Meeting> GetByIdAsync(int id);
        Task<List<Friend>> GetAllFriendsAsync();
        Task ReloadFriendAsync(int friendId);
    }
}