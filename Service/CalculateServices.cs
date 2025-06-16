using System;
using System.Collections.Generic;
using System.Linq;
using MilitaryServices.App.Dao;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Enums;
using Microsoft.Extensions.Logging;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Services
{
    public class CalculateServices
    {
        private readonly SoldierRepository _soldierRepository;
        private readonly IServiceOfUnitRepository _serOfUnitAccess;
        private readonly IUserRepository _userRepository;
        private readonly CountServicesForEachSold _countServicesForEachSold;
        private readonly ILogger<CalculateServices> _logger;

        public CalculateServices(
            SoldierRepository soldierRepository,
            IServiceOfUnitRepository serOfUnitAccess,
            IUserRepository userRepository,
            CountServicesForEachSold countServicesForEachSold,
            ILogger<CalculateServices> logger)
        {
            _soldierRepository = soldierRepository;
            _serOfUnitAccess = serOfUnitAccess;
            _userRepository = userRepository;
            _countServicesForEachSold = countServicesForEachSold;
            _logger = logger;
        }

        public List<Soldier> CalculateNextServices(string username)
        {
            var allSoldiers = LoadSoldiersAndServices(username);
            if (allSoldiers.Count == 0) throw new Exception("No soldiers loaded for user.");

            var soldierMap = allSoldiers.ToDictionary(s => s.SoldierId, s => s);
            var armedSoldiers = new HashSet<Soldier>();
            var unarmedSoldiers = new HashSet<Soldier>();

            var unit = allSoldiers.First().Unit;
            var nextDate = FindNextCalculationDate(allSoldiers.First().Service.Date);

            var servicesOfUnit = _serOfUnitAccess.FindByUnit(unit);
            var armedServices = new List<Service>();
            var unarmedServices = new List<Service>();
            bool specialCase = true;

            AddServicesAndSoldiers(allSoldiers, armedSoldiers, unarmedSoldiers, soldierMap, servicesOfUnit, armedServices, unarmedServices);
            SetFreeAndOutgoingSoldiers(allSoldiers, armedSoldiers, unarmedSoldiers);
            int numberOfOutgoing = CalculateNumberOfOutgoing(allSoldiers);
            if ((armedSoldiers.Count - armedServices.Count) >= numberOfOutgoing)
            {
                var proportionList = _countServicesForEachSold.GetProportions(armedSoldiers, unarmedSoldiers, allSoldiers, soldierMap, true, "");
                CalculateOutgoingSoldiers(armedSoldiers, unarmedSoldiers, soldierMap, proportionList, numberOfOutgoing);
                specialCase = false;
            }

            if (specialCase)
            {
                int armedQuota = armedSoldiers.Count - armedServices.Count;
                var armedProps = _countServicesForEachSold.GetProportions(armedSoldiers, unarmedSoldiers, allSoldiers, soldierMap, false, Situation.ARMED.ToString().ToLower());
                CalculateOutgoingInRareCase(armedSoldiers, soldierMap, armedProps, armedQuota);

                var unarmedProps = _countServicesForEachSold.GetProportions(armedSoldiers, unarmedSoldiers, allSoldiers, soldierMap, false, Situation.UNARMED.ToString().ToLower());
                CalculateOutgoingInRareCase(unarmedSoldiers, soldierMap, unarmedProps, numberOfOutgoing - armedQuota);
            }

            CalculateServicesForUnarmedSoldiers(unarmedSoldiers, unarmedServices);

            if (unarmedServices.Count > 0)
            {
                SetUnarmedServicesToArmedSoldiers(allSoldiers, armedSoldiers, soldierMap, unarmedServices);
            }

            CalculateServicesForArmedSoldiers(armedSoldiers, armedServices);
            SetCalculationDateAndUnit(nextDate, allSoldiers);

            return allSoldiers;
        }

        public void SaveNewServices(List<Soldier> soldiers)
        {
            try
            {
                _soldierRepository.SaveSoldiers(soldiers);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save soldiers.", ex);
            }
        }

        private List<Soldier> LoadSoldiersAndServices(string username)
        {
            var user = _userRepository.FindById(username) ?? throw new Exception("User not found");
            var unit = user.Soldier.Unit;
            var lastDate = _soldierRepository.GetDateOfLastCalculation(unit);
            return _soldierRepository.LoadSold(unit, lastDate);
        }

        private DateTime FindNextCalculationDate(DateTime lastDate) =>
            lastDate.Date.AddDays(1);

        private void AddServicesAndSoldiers(List<Soldier> allSoldiers,
                                            HashSet<Soldier> armedSoldiers,
                                            HashSet<Soldier> unarmedSoldiers,
                                            Dictionary<int, Soldier> soldierMap,
                                            List<ServiceOfUnit> servicesOfUnit,
                                            List<Service> armedServices,
                                            List<Service> unarmedServices)
        {
            var unit = allSoldiers.First().Unit;
            var today = DateTime.Now;

            foreach (var svc in servicesOfUnit)
            {
                var newService = new Service(svc.ServiceName, svc.Armed, today, unit, svc.Company, svc.Description, svc.Shift);
                if (svc.Armed)
                    armedServices.Add(newService);
                else
                    unarmedServices.Add(newService);
            }

            armedSoldiers.Clear();
            unarmedSoldiers.Clear();

            foreach (var s in allSoldiers)
            {
                if (s.IsArmed)
                    armedSoldiers.Add(s);
                else
                    unarmedSoldiers.Add(s);

                soldierMap[s.SoldierId] = s;
            }
        }

        private void SetFreeAndOutgoingSoldiers(List<Soldier> allSoldiers,
                                                HashSet<Soldier> armedSoldiers,
                                                HashSet<Soldier> unarmedSoldiers)
        {
            foreach (var s in allSoldiers)
            {
                if (!s.IsActive)
                {
                    s.Service = new Service(ActiveExtensions.GetFreeOfDuty(), ActiveExtensions.GetFreeOfDuty(), DateTime.Now, s.Unit);
                    if (s.IsArmed)
                        armedSoldiers.Remove(s);
                    else
                        unarmedSoldiers.Remove(s);
                }
            }
        }

        private void CalculateOutgoingSoldiers(HashSet<Soldier> armed,
                                               HashSet<Soldier> unarmed,
                                               Dictionary<int, Soldier> soldierMap,
                                               List<SoldierProportion> props,
                                               int count)
        {
            AssignAsOutgoingBasedOnProportion(armed, unarmed, soldierMap, props, count);
        }

        private void AssignAsOutgoingBasedOnProportion(HashSet<Soldier> armed,
                                                       HashSet<Soldier> unarmed,
                                                       Dictionary<int, Soldier> soldierMap,
                                                       List<SoldierProportion> proportions,
                                                       int number)
        {
            if (number == 0) return;

            foreach (var prop in proportions.OrderByDescending(p => p.Proportion))
            {
                if (number == 0) break;
                if (soldierMap.TryGetValue(prop.SoldId, out var soldier))
                {
                    soldier.Service = new Service("out", ActiveExtensions.GetFreeOfDuty(), DateTime.Now, soldier.Unit, soldier.Company, ActiveExtensions.GetFreeOfDuty(), "06:00-06:00");
                    if (soldier.IsArmed)
                        armed.Remove(soldier);
                    else
                        unarmed.Remove(soldier);
                    number--;
                }
            }
        }

        private void CalculateOutgoingInRareCase(HashSet<Soldier> pool,
                                                 Dictionary<int, Soldier> soldierMap,
                                                 List<SoldierProportion> props,
                                                 int number)
        {
            foreach (var prop in props.OrderByDescending(p => p.Proportion))
            {
                if (number == 0) break;
                if (soldierMap.TryGetValue(prop.SoldId, out var soldier))
                {
                    soldier.Service = new Service("out", ActiveExtensions.GetFreeOfDuty(), DateTime.Now, soldier.Unit, soldier.Company, ActiveExtensions.GetFreeOfDuty(), "06:00-06:00");
                    pool.Remove(soldier);
                    number--;
                }
            }
        }

        private int CalculateNumberOfOutgoing(List<Soldier> allSoldiers)
        {
            var count = _serOfUnitAccess.CountServicesOfUnit(allSoldiers.First().Unit)?.FirstOrDefault() ?? 0;
            return TotalSoldiersForCalculation(allSoldiers) - (int)count;
        }

        private int TotalSoldiersForCalculation(List<Soldier> allSoldiers) =>
            allSoldiers.Count(s => s.Active.Equals(Active.ACTIVE.ToString().ToLower()));

        private void CalculateServicesForUnarmedSoldiers(HashSet<Soldier> unarmed, List<Service> services)
        {
            var rnd = new Random();
            foreach (var s in unarmed)
            {
                if (services.Count == 0) break;
                var idx = rnd.Next(services.Count);
                s.Service = services[idx];
                services.RemoveAt(idx);
            }
        }

        private void SetUnarmedServicesToArmedSoldiers(List<Soldier> allSoldiers,
                                                       HashSet<Soldier> armedSoldiers,
                                                       Dictionary<int, Soldier> soldierMap,
                                                       List<Service> services)
        {
            var history = _soldierRepository.GetHistoricalDataDesc(allSoldiers.First().Unit, Situation.ARMED.ToString().ToLower());

            if (history.Count < armedSoldiers.Count)
                _countServicesForEachSold.AddTheRestOnes(history, soldierMap);

            foreach (var hd in history)
            {
                if (services.Count == 0) break;

                if (!soldierMap.TryGetValue(hd.SoldierId, out var s)) continue;
                if (s.Service?.ServiceName == "out") continue;

                s.Service = services[0];
                services.RemoveAt(0);
                armedSoldiers.Remove(s);
            }

            services.Clear();
        }

        private void CalculateServicesForArmedSoldiers(HashSet<Soldier> armed, List<Service> services)
        {
            var rnd = new Random();
            foreach (var s in armed)
            {
                if (services.Count == 0) break;
                var idx = rnd.Next(services.Count);
                s.Service = services[idx];
                services.RemoveAt(idx);
            }
        }

        private void SetCalculationDateAndUnit(DateTime date, List<Soldier> allSoldiers)
        {
            foreach (var s in allSoldiers)
            {
                s.Service.Date = date;
                s.Service.Unit = s.Unit;
            }
        }
    }
}
