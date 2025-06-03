using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class ServiceOfUnitRepository
    {
        private readonly MilitaryDbContext _context;

        public ServiceOfUnitRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceOfUnit>> FindByUnitAsync(Unit unit)
        {
            return await _context.ServiceOfUnits
                .Where(s => s.Unit == unit)
                .ToListAsync();
        }

        public async Task<int> CountServicesOfUnitAsync(Unit unit)
        {
            return await _context.ServiceOfUnits
                .Where(s => s.Unit == unit)
                .CountAsync();
        }

        public async Task<List<ServiceOfUnit>> FindByUnitAndArmedAsync(Unit unit, string armed)
        {
            return await _context.ServiceOfUnits
                .Where(s => s.Unit == unit && s.Armed == armed)
                .ToListAsync();
        }
    }
}
