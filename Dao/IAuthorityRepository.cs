using System.Collections.Generic;
using System.Threading.Tasks;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public interface IAuthorityRepository
    {
        Task<List<Authority>> FindByUserAsync(User user);
    }
}
