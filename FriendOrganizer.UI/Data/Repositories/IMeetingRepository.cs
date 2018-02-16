using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IMeetingRepository:IGenereicRepository<Meeting>
    {
        //Task<Meeting> GetByIdAsync(int id);
    }
}