using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class UserRepository
    {
        private readonly MilitaryDbContext _context;

        public UserRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public async Task<User?> FindByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
