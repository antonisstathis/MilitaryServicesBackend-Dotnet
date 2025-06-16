using System.Collections.Generic;
using System.Linq;
using MilitaryServices.App.Entity;
using Microsoft.EntityFrameworkCore;

namespace MilitaryServices.App.Dao
{
    public class AuthorityRepository : IAuthorityRepository
    {
        private readonly MilitaryDbContext _context;

        public AuthorityRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public List<Authority> FindByUser(User user)
        {
            return [.. _context.Authorities.Where(a => a.Username== user.Username)];
        }
    }
}
