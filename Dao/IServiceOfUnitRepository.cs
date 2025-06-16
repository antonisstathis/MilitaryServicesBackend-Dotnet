using System.Collections.Generic;
using System.Threading.Tasks;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public interface IServiceOfUnitRepository
    {
        List<ServiceOfUnit> FindByUnit(Unit unit);
        int CountServicesOfUnit(Unit unit);
        List<ServiceOfUnit> FindByUnitAndArmed(Unit unit, string armed);
        void DeleteAllById(List<long> ids);
        ServiceOfUnit Save(ServiceOfUnit service);
    }
}
