using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class ServiceOfUnitRepository : IServiceOfUnitRepository
    {
        private readonly MilitaryDbContext _context;

        public ServiceOfUnitRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public List<ServiceOfUnit> FindByUnit(Unit unit)
        {
            return [.. _context.ServiceOfUnits.Where(s => s.Unit == unit)];
        }

        public int CountServicesOfUnit(Unit unit)
        {
            return _context.ServiceOfUnits
                .Where(s => s.Unit == unit)
                .Count();
        }

        public List<ServiceOfUnit> FindByUnitAndArmed(Unit unit, string armed)
        {
            return [.. _context.ServiceOfUnits.Where(s => s.Unit == unit && s.Armed == armed)];
        }

        public void DeleteAllById(List<long> ids)
        {
            var servicesToDelete = _context.Services
            .Where(s => ids.Contains(s.Id))
            .ToList();

            if (servicesToDelete.Count > 0)
            {
                _context.Services.RemoveRange(servicesToDelete);
                _context.SaveChanges();
            }
        }

         public ServiceOfUnit Save(ServiceOfUnit service)
        {
            if (service.Id == 0)
                _context.ServiceOfUnits.Add(service);
            else
                _context.ServiceOfUnits.Update(service);

            _context.SaveChanges();
            return service;
        }
    }
}
