using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Dao;

namespace MilitaryServices.App.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public User? FindUser(string username)
        {
            return _userRepository.FindById(username);
        }
    }
}
