using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public interface ISoldierRepository
    {
        void SaveSoldier(Soldier soldier, DateTime currentDate);

        void SaveSoldiers(IEnumerable<Soldier> soldiers);

        List<Soldier> LoadSold(Unit unit, DateTime dateOfLastCalc);

        DateTime GetDateOfCalculation(Unit unit, int calculations);

        DateTime GetDateOfFirstCalculation(Unit unit);

        DateTime GetDateOfLastCalculation(Unit unit);

        List<SoldierServiceDto> FindCalculationByDate(Unit unit, DateTime date);

        void UpdateSoldier(Soldier soldier);

        List<HistoricalData> GetHistoricalDataDesc(Unit unit, string armed);

        List<HistoricalData> GetHistoricalDataAsc(Unit unit, string armed);

        List<SoldierServiceStatDto> GetSoldierServiceStatisticalData(Expression<Func<Soldier, bool>> soldierPredicate, Func<MilitaryServices.App.Entity.Service, bool> servicePredicate);

        Soldier FindSoldierById(int soldierId);

        List<Soldier> FindAll(Soldier soldier);

        List<Soldier> FindByUnitAndDischarged(Unit unit, bool discharged);

        List<Soldier> FindBySoldierRegistrationNumberContainingIgnoreCase(string registrationFragment);

        void UpdateDischargedStatusById(int id, bool discharged);
    }
}
