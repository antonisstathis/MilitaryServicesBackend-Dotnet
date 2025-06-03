using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class ServiceRepository
    {
        private readonly MilitaryDbContext _context;

        public ServiceRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> FindByUnitAndDateAndArmedAsync(Unit unit, DateTime date, string armed)
        {
            return await _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue && s.Date.Value.Date == date.Date && s.Armed == armed)
                .ToListAsync();
        }

        public async Task<List<Service>> FindByUnitAndDateAsync(Unit unit, DateTime date)
        {
            return await _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue && s.Date.Value.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Service>> FindBySoldierAsync(Soldier soldier)
        {
            return await _context.Services
                .Where(s => s.Soldier == soldier)
                .ToListAsync();
        }
    }
}