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
    public class CountServicesForEachSold
    {
        private readonly SoldierRepository _soldierRepository;

        public CountServicesForEachSold(SoldierRepository soldierRepository)
        {
            _soldierRepository = soldierRepository;
        }

        public List<SoldierProportion> GetProportions(
            HashSet<Soldier> armedSoldiers,
            HashSet<Soldier> unarmedSoldiers,
            List<Soldier> allSoldiers,
            Dictionary<int, Soldier> soldierMap,
            bool mode,
            string armed)
        {
            var datas = GetHistoricalData(armedSoldiers, unarmedSoldiers, allSoldiers, soldierMap);
            var proportions = new List<SoldierProportion>();

            foreach (var soldier in allSoldiers)
            {
                if (mode || soldier.Situation.Equals(armed, StringComparison.OrdinalIgnoreCase))
                {
                    int countServices = CountAllServices(soldier, datas);
                    int countOut = AddServices(datas.HdForOut.TryGetValue(soldier.SoldierId, out HistoricalData? outData) ? outData : null, 0);

                    float proportion = 0;
                    if (countServices != 0 && countOut != 0)
                        proportion = (float)countServices / countOut;
                    else if (countServices == 0 && countOut != 0)
                        proportion = 0;
                    else if (countServices != 0 && countOut == 0)
                        proportion = float.MaxValue;

                    proportions.Add(new SoldierProportion(soldier.SoldierId, proportion));
                }
            }

            return proportions;
        }

        private CountServices GetHistoricalData(
            HashSet<Soldier> armedSoldiers,
            HashSet<Soldier> unarmedSoldiers,
            List<Soldier> allSoldiers,
            Dictionary<int, Soldier> soldierMap)
        {
            var unit = allSoldiers.First().Unit;
            var armedServices = _soldierRepository.GetHistoricalDataDesc(unit, Situation.ARMED.ToString().ToLower());
            var unarmedServices = _soldierRepository.GetHistoricalDataDesc(unit, Situation.UNARMED.ToString().ToLower());
            var outServices = _soldierRepository.GetHistoricalDataDesc(unit, ActiveExtensions.GetFreeOfDuty());

            if (outServices.Count < allSoldiers.Count)
                AddTheRestOnes(outServices, soldierMap);
            if (unarmedServices.Count < unarmedSoldiers.Count)
                AddTheRestOnes(armedServices, soldierMap);
            if (armedServices.Count < armedSoldiers.Count)
                AddTheRestOnes(unarmedServices, soldierMap);

            var armedMap = CreateMap(armedServices);
            var unarmedMap = CreateMap(unarmedServices);
            var outMap = CreateMap(outServices);

            return new CountServices(armedMap, unarmedMap, outMap);
        }

        private int CountAllServices(Soldier soldier, CountServices datas)
        {
            int count = 0;
            datas.HdForArmed.TryGetValue(soldier.SoldierId, out var armedData);
            datas.HdForUnarmed.TryGetValue(soldier.SoldierId, out var unarmedData);

            count = AddServices(armedData, count);
            count = AddServices(unarmedData, count);

            return count;
        }

        private int AddServices(HistoricalData historicalData, int count)
        {
            return historicalData != null ? (int)historicalData.NumberOfServices + count : count;
        }

        public void AddTheRestOnes(List<HistoricalData> historicalData, Dictionary<int, Soldier> soldierMap)
        {
            var existing = historicalData
                .Select(hd => hd.SoldierId)
                .ToHashSet();

            foreach (var kvp in soldierMap)
            {
                if (!existing.Contains(kvp.Key))
                {
                    historicalData.Add(new HistoricalData(kvp.Key, 0));
                }
            }
        }

        private Dictionary<int, HistoricalData> CreateMap(List<HistoricalData> list)
        {
            return list.ToDictionary(hd => hd.SoldierId, hd => hd);
        }
    }
}
