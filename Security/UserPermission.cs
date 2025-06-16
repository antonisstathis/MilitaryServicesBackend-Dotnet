using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Service;
using MilitaryServices.App.Services;
using MilitaryServices.App.Services.Implementations;
using MilitaryServicesBackendDotnet.Security;

namespace MilitaryServices.App.Security
{
    public class UserPermission
    {
        private readonly SoldierService _soldierService;
        private readonly UserService _userService;
        private readonly JwtUtil _jwtUtil;

        public UserPermission(SoldierService soldierService, UserService userService, JwtUtil jwtUtil)
        {
            _soldierService = soldierService;
            _userService = userService;
            _jwtUtil = jwtUtil;
        }

        public bool CheckIfUserHasAccess(string token, HttpRequest request, string situation, string active)
        {
            if (!_jwtUtil.IsTokenValid(token))
                return false;

            if (!int.TryParse(_jwtUtil.ExtractUsername(token), out var soldierId))
                return false;
            var username = ExtractUsernameFromRequest(request);
            bool isPermitted = CheckIfSoldierBelongsToUser(soldierId, username);
            if (!isPermitted)
                return false;

            bool areOptionsValid = CheckOptions.AreValidOptions(situation, active);
            if (!areOptionsValid)
                return false;

            return true;
        }

        public bool CheckIfSoldierBelongsToUser(int soldierId, string username)
        {
            var soldierToUpdate = _soldierService.FindSoldier(soldierId);
            var optionalUser = _userService.FindUser(username);

            if (optionalUser == null || optionalUser.Soldier == null)
                return false;

            var userSoldier = _soldierService.FindSoldier(optionalUser.Soldier.SoldierId);

            return soldierToUpdate.Unit.UnitId == userSoldier.Unit.UnitId;
        }

        private string? ExtractUsernameFromRequest(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length).Trim();
            return _jwtUtil.ExtractUsername(token);
        }
    }
}
