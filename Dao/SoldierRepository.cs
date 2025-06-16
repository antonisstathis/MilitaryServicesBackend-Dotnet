using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dao
{
    public class SoldierRepository : ISoldierRepository
    {
        private readonly MilitaryDbContext _context;

        public SoldierRepository(MilitaryDbContext context)
        {
            _context = context;
        }

        public void SaveSoldier(Soldier soldier, DateTime currentDate)
        {
            _context.Soldiers.Add(soldier);
            _context.SaveChanges();

            var service = new MilitaryServices.App.Entity.Service
            {
                ServiceName = "out",
                Armed = string.Empty,
                Date = currentDate,
                Soldier = soldier
            };

            _context.Services.Add(service);
            _context.SaveChanges();
        }

        public void SaveSoldiers(IEnumerable<Soldier> soldiers)
        {
            foreach (var soldier in soldiers)
            {
                var service = soldier.Service;
                service.Soldier = soldier;
                service.Unit = soldier.Unit;
                _context.Services.Add(service);
            }
            _context.SaveChanges();
        }

        public List<Soldier> LoadSold(Unit unit, DateTime dateOfLastCalc)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit
                            && !s.Discharged
                            && u.Date.HasValue
                            && u.Date.Value.Date == dateOfLastCalc.Date
                        orderby s.SoldierId
                        select new SoldierServiceDto
                        {
                            Id = s.SoldierId,
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

            var list = query.ToList();

            return [.. list.Select(dto =>
            {
                var sold = new Soldier
                {
                    SoldierId = dto.Id,
                    Company = dto.Company,
                    SoldierRegistrationNumber = dto.SoldierRegistrationNumber,
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Situation = dto.Situation,
                    Active = dto.Active,
                    Discharged = dto.Discharged
                };

                var service = new MilitaryServices.App.Entity.Service
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
            })];
        }

        public DateTime GetDateOfCalculation(Unit unit, int calculations)
        {
            var dateOfFirstCalculation = GetDateOfFirstCalculation(unit);
            return dateOfFirstCalculation.AddDays(calculations - 1);
        }

        public DateTime GetDateOfFirstCalculation(Unit unit)
        {
            return _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue)
                .Min(s => s.Date.Value);
        }

        public DateTime GetDateOfLastCalculation(Unit unit)
        {
            return _context.Services
                .Where(s => s.Unit == unit && s.Date.HasValue)
                .Max(s => s.Date.Value);
        }

        public List<SoldierServiceDto> FindCalculationByDate(Unit unit, DateTime date)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit
                            && u.Date.HasValue
                            && u.Date.Value.Date == date.Date
                        orderby s.SoldierId
                        select new SoldierServiceDto
                        {
                            Id = s.SoldierId,
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

            return [.. query];
        }

        public void UpdateSoldier(Soldier soldier)
        {
            _context.Soldiers.Update(soldier);
            _context.SaveChanges();
        }

        public List<HistoricalData> GetHistoricalDataDesc(Unit unit, string armed)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit && !s.Discharged && u.Armed == armed
                        group s by s.SoldierId into g
                        orderby g.Count() descending
                        select new HistoricalData(g.Key, g.Count());

            return [.. query];
        }

        public List<HistoricalData> GetHistoricalDataAsc(Unit unit, string armed)
        {
            var query = from s in _context.Soldiers
                        join u in _context.Services on s equals u.Soldier
                        where s.Unit == unit && !s.Discharged && u.Armed == armed
                        group s by s.SoldierId into g
                        orderby g.Count() ascending
                        select new HistoricalData(g.Key, g.Count());

            return [.. query];
        }
        
        public List<SoldierServiceStatDto> GetSoldierServiceStatisticalData(Expression<Func<Soldier, bool>> soldierPredicate, Func<MilitaryServices.App.Entity.Service, bool> servicePredicate)
        {
            var query = from soldier in _context.Soldiers.Where(soldierPredicate)
                        join service in _context.Services.Where(servicePredicate)
                            on soldier.SoldierId equals service.SoldierId
                        group soldier by soldier.SoldierRegistrationNumber into g
                        orderby g.Count() descending
                        select new SoldierServiceStatDto
                        {
                            SoldierRegNumber = g.Key,
                            Company = g.Select(s => s.Company).FirstOrDefault()!,
                            Name = g.Select(s => s.Name).FirstOrDefault()!,
                            Surname = g.Select(s => s.Surname).FirstOrDefault()!,
                            Active = g.Select(s => s.Active).FirstOrDefault()!,
                            Situation = g.Select(s => s.Situation).FirstOrDefault()!,
                            NumberOfServices = g.Count()
                        };

            return [.. query];
        }

        public Soldier FindSoldierById(int soldierId)
        {
            return _context.Soldiers.Find(soldierId);
        }

        public List<Soldier> FindAll(Soldier soldier)
        {
            var dateOfLastCalculation = GetDateOfLastCalculation(soldier.Unit);
            return LoadSold(soldier.Unit, dateOfLastCalculation);
        }

        private DateTime ConvertStringToDate(string date)
        {
            if (DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                return parsedDate;

            throw new FormatException($"Invalid date format: {date}");
        }

        public List<Soldier> FindByUnitAndDischarged(Unit unit, bool discharged)
        {
            return [.. _context.Soldiers
                .Where(s => s.Unit == unit && s.Discharged == discharged)
                .OrderBy(s => s.SoldierId)];
        }

        public List<Soldier> FindBySoldierRegistrationNumberContainingIgnoreCase(string registrationFragment)
        {
            return [.. _context.Soldiers.Where(s => EF.Functions.Like(s.SoldierRegistrationNumber.ToLower(), $"%{registrationFragment.ToLower()}%"))];
        }

        public void UpdateDischargedStatusById(int id, bool discharged)
        {
            var soldier = _context.Soldiers.Find(id);
            if (soldier != null)
            {
                soldier.Discharged = discharged;
                _context.SaveChanges();
            }
        }
    }
}
