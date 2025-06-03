using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class SoldierRepository
    {
        private readonly MilitaryDbContext _context;

        public SoldierRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public async Task SaveSoldierAsync(Soldier soldier, DateTime currentDate)
        {
            _context.Soldiers.Add(soldier);
            await _context.SaveChangesAsync();

            var service = new Service
            {
                ServiceName = "out",
                Armed = string.Empty,
                Date = currentDate,
                Soldier = soldier
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task SaveSoldiersAsync(IEnumerable<Soldier> soldiers)
        {
            foreach (var soldier in soldiers)
            {
                var service = soldier.Service;
                service.Soldier = soldier;
                service.Unit = soldier.Unit;
                _context.Services.Add(service);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Soldier>> LoadSoldAsync(Unit unit, DateTime dateOfLastCalc)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit
                            && !s.Discharged
                            && u.Date.HasValue
                            && u.Date.Value.Date == dateOfLastCalc.Date
                        orderby s.Id
                        select new SoldierServiceDto
                        {
                            Id = s.Id,
                            Company = s.Company,
                            SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                            Name = s.Name,
                            Surname = s.Surname,
                            Situation = s.Situation,
                            Active = s.Active,
                            ServiceId = u.Id,
                            Service = u.ServiceName,
                            Date = (DateTime)u.Date,
                            Armed = u.Armed,
                            Unit = s.Unit,
                            Discharged = s.Discharged,
                            Description = u.Description,
                            Shift = u.Shift
                        };

            var list = await query.ToListAsync();

            return list.Select(dto =>
            {
                var sold = new Soldier
                {
                    Id = dto.Id,
                    Company = dto.Company,
                    SoldierRegistrationNumber = dto.SoldierRegistrationNumber,
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Situation = dto.Situation,
                    Active = dto.Active,
                    Discharged = dto.Discharged
                };

                var service = new Service
                {
                    ServiceName = dto.Service,
                    Armed = dto.Armed,
                    Date = dto.Date,
                    Unit = dto.Unit,
                    Company = dto.Company,
                    Description = dto.Description,
                    Shift = dto.Shift
                };

                sold.Service = service;
                sold.Unit = service.Unit;
                return sold;
            }).ToList();
        }

        public DateTime GetDateOfCalculation(Unit unit, int calculations)
        {
            var dateOfFirstCalculation = GetDateOfFirstCalculation(unit).Result;
            return dateOfFirstCalculation.AddDays(calculations - 1);
        }

        public async Task<DateTime> GetDateOfFirstCalculation(Unit unit)
        {
            return await _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue)
                .MinAsync(s => s.Date.Value);
        }

        public async Task<DateTime> GetDateOfLastCalculation(Unit unit)
        {
            return await _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue)
                .MaxAsync(s => s.Date.Value);
        }

        public async Task<List<SoldierServiceDto>> FindCalculationByDateAsync(Unit unit, DateTime date)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit
                            && u.Date.HasValue
                            && u.Date.Value.Date == date.Date
                        orderby s.Id
                        select new SoldierServiceDto
                        {
                            Id = s.Id,
                            Company = s.Company,
                            SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                            Name = s.Name,
                            Surname = s.Surname,
                            Situation = s.Situation,
                            Active = s.Active,
                            ServiceId = u.Id,
                            Service = u.ServiceName,
                            Date = (DateTime)u.Date,
                            Armed = u.Armed,
                            Unit = s.Unit,
                            Discharged = s.Discharged
                        };

            return await query.ToListAsync();
        }

        public async Task UpdateSoldierAsync(Soldier soldier)
        {
            _context.Soldiers.Update(soldier);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HistoricalData>> GetHistoricalDataDescAsync(Unit unit, string armed)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit && !s.Discharged && u.Armed == armed
                        group s by s.Id into g
                        orderby g.Count() descending
                        select new HistoricalData(g.Key, g.Count());

            return await query.ToListAsync();
        }

        public async Task<List<HistoricalData>> GetHistoricalDataAscAsync(Unit unit, string armed)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit && !s.Discharged && u.Armed == armed
                        group s by s.Id into g
                        orderby g.Count() ascending
                        select new HistoricalData(g.Key, g.Count());

            return await query.ToListAsync();
        }

        public async Task<List<SoldierServiceStatDto>> GetSoldierServiceStatisticalDataAsync(IQueryable<Soldier> soldiersQuery)
        {
            var query = soldiersQuery
                .GroupJoin(_context.Services,
                    soldier => soldier.Id,
                    service => service.Soldier.Id,
                    (soldier, services) => new
                    {
                        Soldier = soldier,
                        ServiceCount = services.LongCount()
                    })
                .OrderByDescending(x => x.ServiceCount)
                .Select(x => new SoldierServiceStatDto(
                    x.Soldier.SoldierRegistrationNumber,
                    x.Soldier.Company,
                    x.Soldier.Name,
                    x.Soldier.Surname,
                    x.Soldier.Active,
                    x.Soldier.Situation,
                    (int)x.ServiceCount
                ));

            return await query.ToListAsync();
        }

        public async Task<Soldier> FindSoldierByIdAsync(int soldierId)
        {
            return await _context.Soldiers.FindAsync(soldierId);
        }

        public async Task<List<Soldier>> FindAllAsync(Soldier soldier)
        {
            var dateOfLastCalculation = await GetDateOfLastCalculation(soldier.Unit);
            return await LoadSoldAsync(soldier.Unit, dateOfLastCalculation);
        }

        private DateTime ConvertStringToDate(string date)
        {
            if (DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                return parsedDate;

            throw new FormatException($"Invalid date format: {date}");
        }

        public async Task<List<Soldier>> FindByUnitAndDischargedAsync(Unit unit, bool discharged)
        {
            return await _context.Soldiers
                .Where(s => s.Unit == unit && s.Discharged == discharged)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<List<Soldier>> FindBySoldierRegistrationNumberContainingIgnoreCaseAsync(string registrationFragment)
        {
            return await _context.Soldiers
                .Where(s => EF.Functions.Like(s.SoldierRegistrationNumber.ToLower(), $"%{registrationFragment.ToLower()}%"))
                .ToListAsync();
        }

        public async Task UpdateDischargedStatusByIdAsync(int id, bool discharged)
        {
            var soldier = await _context.Soldiers.FindAsync(id);
            if (soldier != null)
            {
                soldier.Discharged = discharged;
                await _context.SaveChangesAsync();
            }
        }
    }
}
