using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<MilitaryServices.App.Entity.Service> FindByUnitAndDateAndArmed(Unit unit, DateTime date, string armed)
        {
            return [.. _context.Services.Where(s => s.Unit == unit && s.Date.HasValue && s.Date.Value.Date == date.Date && s.Armed == armed)];
        }

        public List<MilitaryServices.App.Entity.Service> FindByUnitAndDate(Unit unit, DateTime date)
        {
            return [.. _context.Services.Where(s => s.Unit == unit && s.Date.HasValue && s.Date.Value.Date == date.Date)];
        }

        public List<MilitaryServices.App.Entity.Service> FindBySoldier(Soldier soldier)
        {
            return [.. _context.Services.Where(s => s.Soldier == soldier)];
        }
    }
}