using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{

    public interface IFriendRepository:IGenereicRepository<Friend>
    {
       
        void RemovePhoneNumber(FriendPhoneNumber model);
    }
}