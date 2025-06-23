using System.Collections.Generic;
using System.Linq;
using MilitaryServices.App.Dao;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Services
{
    public class AuthorityService : IAuthorityService
    {
        private readonly IAuthorityRepository _authorityRepository;

        public AuthorityService(IAuthorityRepository authorityRepository)
        {
            _authorityRepository = authorityRepository;
        }

        public List<string> FindRolesByUsernameAsync(User user)
        {
            var authorities = _authorityRepository.FindByUser(user);
            return [.. authorities.Select(auth => $"ROLE_{auth.AuthorityName.ToUpper()}")];
        }
    }
}
