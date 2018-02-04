using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IGenereicRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task SaveAsync();
        bool HasChanges();
        void Add(T model);
        void Remove(T model);
    }
}