using System;
using System.Collections.Generic;
using System.Linq;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Dao;

namespace MilitaryServices.App.Test
{
    public class CheckOutput
    {
        private readonly IUserRepository _userRepository;
        private readonly ISoldierRepository _soldierRepository;
        private readonly IServiceOfUnitRepository _serOfUnitRepository;

        // Constructor injection for dependencies
        public CheckOutput(
            IUserRepository userRepository,
            ISoldierRepository soldierRepository,
            IServiceOfUnitRepository serOfUnitRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _soldierRepository = soldierRepository ?? throw new ArgumentNullException(nameof(soldierRepository));
            _serOfUnitRepository = serOfUnitRepository ?? throw new ArgumentNullException(nameof(serOfUnitRepository));
        }

        public bool CheckResults(string username)
        {
            var user = _userRepository.FindById(username);
            if (user == null || user.Soldier == null)
                return false;

            Unit unit = user.Soldier.Unit;
            DateTime dateOfLastCalculation = _soldierRepository.GetDateOfLastCalculation(unit);
            List<Soldier> allSoldiers = _soldierRepository.LoadSold(unit, dateOfLastCalculation);
            List<ServiceOfUnit> servicesOfUnit = _serOfUnitRepository.FindByUnit(unit);

            var servicesMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var serviceOfUnit in servicesOfUnit)
            {
                if (servicesMap.TryGetValue(serviceOfUnit.ServiceName, out int freq))
                    servicesMap[serviceOfUnit.ServiceName] = freq + 1;
                else
                    servicesMap[serviceOfUnit.ServiceName] = 1;
            }

            foreach (var soldier in allSoldiers)
            {
                string serviceName = soldier.Service?.ServiceName ?? string.Empty;

                if (servicesMap.ContainsKey(serviceName))
                {
                    servicesMap[serviceName]--;
                }
                else if (!string.Equals(serviceName, "out", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return servicesMap.Values.All(count => count == 0);
        }
    }
}
