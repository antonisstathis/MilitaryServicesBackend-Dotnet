using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Services
{
    public interface IUserService
    {
        User? FindUser(string username);
    }
}
