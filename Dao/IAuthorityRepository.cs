using System.Collections.Generic;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public interface IAuthorityRepository
    {
        List<Authority> FindByUser(User user);
    }
}
