using System.Collections.Generic;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Services
{
    public interface IAuthorityService
    {
        List<string> FindRolesByUsername(User user);
    }
}
