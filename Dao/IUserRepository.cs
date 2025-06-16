using MilitaryServices.App.Entity;
using System.Collections.Generic;

namespace MilitaryServices.App.Dao
{
    public interface IUserRepository
    {
        User? FindById(string id);

        void AddUser(User user);

        void DeleteUser(string id);

        List<User> GetAll();
    }
}
