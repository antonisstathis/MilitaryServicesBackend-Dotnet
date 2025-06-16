// This is a C# .NET Core version of the Java SoldierServiceImpl
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilitaryServices.App.Dao;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Enums;
using MilitaryServices.App.Security;
using MilitaryServicesApp.Service;
using MilitaryServicesBackendDotnet.Security;
using MilitaryServices.App.Extensions;
using MilitaryServices.App.Test;

namespace MilitaryServices.App.Services.Implementations
{
    public class SoldierService : ISoldierService
    {
        private readonly ISoldierRepository _soldierRepository;
        private readonly CalculateServices _service;
        private readonly JwtUtil _jwtUtil;
        private readonly IUserRepository _userRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly IServiceOfUnitRepository _serOfUnitRepository;
        private readonly MilitaryDbContext _context;
        private readonly CheckOutput _checkOutput;

        public SoldierService(ISoldierRepository soldierRepository, CalculateServices service, JwtUtil jwtUtil,
            IUserRepository userRepository, ServiceRepository serviceRepository,
            IServiceOfUnitRepository serOfUnitRepository, MilitaryDbContext context, CheckOutput checkOutput)
        {
            _soldierRepository = soldierRepository;
            _service = service;
            _jwtUtil = jwtUtil;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _serOfUnitRepository = serOfUnitRepository;
            _context = context;
            _checkOutput = checkOutput;
        }

        public List<SoldierDto> FindAll(string username)
        {
            var user = _userRepository.FindById(username);
            var soldier = user?.Soldier;
            var soldiers = _soldierRepository.FindAll(soldier);
            
            return [.. soldiers.Select(sold =>
            {
                var dto = new SoldierDto
                {
                    Token = _jwtUtil.GenerateToken(sold.SoldierId.ToString()),
                    Company = sold.Company,
                    Name = sold.Name,
                    Surname = sold.Surname,
                    Situation = sold.Situation,
                    Active = sold.Active,
                    Service = sold.Service.ServiceName,
                    Armed = sold.Service.Armed
                };
                dto.SetDate(sold.Service.Date ?? DateTime.MinValue);
                return dto;
            })];
        }

        public List<SoldierPersonalDataDto> LoadSoldiers(string username)
        {
            var unit = _userRepository.FindById(username).Soldier.Unit;
            var allSoldiers = _soldierRepository.FindByUnitAndDischarged(unit, false);
            return allSoldiers.Select(s => new SoldierPersonalDataDto
            {
                Token = _jwtUtil.GenerateToken(s.SoldierId.ToString()),
                SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                Company = s.Company,
                Name = s.Name,
                Surname = s.Surname,
                Active = s.Active,
                Situation = s.Situation,
                Discharged = DischargedExtensions.GetDischarged(s.Discharged),
                Patronymic = s.Patronymic,
                Matronymic = s.Matronymic,
                MobilePhone = s.MobilePhone,
                City = s.City,
                Address = s.Address
            }).ToList();
        }

        public List<SoldierPersonalDataDto> FindSoldiersByRegistrationNumber(string regNumber)
        {
            var soldiers = _soldierRepository.FindBySoldierRegistrationNumberContainingIgnoreCase(regNumber);
            return soldiers.Select(s => new SoldierPersonalDataDto
            {
                Token = _jwtUtil.GenerateToken(s.SoldierId.ToString()),
                SoldierRegistrationNumber = s.SoldierRegistrationNumber,
                Company = s.Company,
                Name = s.Name,
                Surname = s.Surname,
                Active = s.Active,
                Situation = s.Situation,
                Discharged = DischargedExtensions.GetDischarged(s.Discharged),
                Patronymic = s.Patronymic,
                Matronymic = s.Matronymic,
                MobilePhone = s.MobilePhone,
                City = s.City,
                Address = s.Address
            }).ToList();
        }

        public List<SoldierPreviousServiceDto> FindPreviousCalculation(string username, DateTime date)
        {
            var unit = _userRepository.FindById(username).Soldier.Unit;
            var list = _soldierRepository.FindCalculationByDate(unit, date);
            return [.. list.Select(d => new SoldierPreviousServiceDto
            {
                Token = _jwtUtil.GenerateToken(d.Id.ToString()),
                SoldierRegistrationNumber = d.SoldierRegistrationNumber,
                Company = d.Company,
                Name = d.Name,
                Surname = d.Surname,
                Active = d.Active,
                Situation = d.Situation,
                Discharged = DischargedExtensions.GetDischarged(d.Discharged),
                Service = d.Service,
                Date = d.Date.ToString("dd-MM-yyyy"),
                Armed = d.Armed
            })];
        }

        public DateTime GetDateByCalculationNumber(string username, int calculation)
        {
            var unit = _userRepository.FindById(username).Soldier.Unit;
            return _soldierRepository.GetDateOfCalculation(unit, calculation);
        }

        public void SaveNewSoldier(SoldierPersonalDataDto dto, Unit unit)
        {
            var calcDate = _soldierRepository.GetDateOfLastCalculation(unit);
            var soldier = new Soldier
            {
                Company = dto.Company,
                SoldierRegistrationNumber = dto.SoldierRegistrationNumber,
                Name = dto.Name,
                Surname = dto.Surname,
                Active = dto.Active,
                Situation = dto.Situation,
                Address = dto.Address,
                City = dto.City,
                Unit = unit,
                Patronymic = dto.Patronymic,
                Matronymic = dto.Matronymic,
                MobilePhone = dto.MobilePhone,
                Discharged = false
            };
            var service = new MilitaryServices.App.Entity.Service("out", ActiveExtensions.GetFreeOfDuty(), calcDate, unit, soldier.Company, ActiveExtensions.GetFreeOfDuty(), "06:00-06:00")
            {
                Soldier = soldier
            };
            soldier.Service = service;
            _context.Soldiers.Add(soldier);
            _context.SaveChanges();
        }

        public void CalculateServices(string username,DateTime lastDate)
        {
            try
            {
                var user = _userRepository.FindById(username);
                Unit unit = user.Soldier?.Unit;
                DateTime dateOfLastCalculation = _soldierRepository.GetDateOfLastCalculation(unit);
                if (dateOfLastCalculation.Date == lastDate.Date)
                {
                    List<Soldier> allSoldiers = _service.CalculateNextServices(username);
                    _service.SaveNewServices(allSoldiers);
                    bool results = _checkOutput.CheckResults(username);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void UpdateSoldier(SoldDto dto)
        {
            var soldier = _soldierRepository.FindSoldierById(dto.Id);
            soldier.Active = dto.Active;
            soldier.Situation = dto.Situation;
            _soldierRepository.UpdateSoldier(soldier);
        }

        public SoldierUnitDto FindSoldier(int id)
        {
            var soldier = _soldierRepository.FindSoldierById(id);
            return new SoldierUnitDto(soldier.SoldierId, soldier.Name, soldier.Surname, soldier.Situation, soldier.Active, soldier.Unit);
        }

        public List<ServiceDto> FindServicesOfSoldier(Unit unit, int soldierId)
        {
            var result = _serviceRepository.FindBySoldier(new Soldier(soldierId));
            return result.Select(service => new ServiceDto
            {
                Id = service.Id,
                Service = service.ServiceName,
                ServiceDate = (DateTime)service.Date,
                Armed = service.Armed,
                Description = service.Description,
                Shift = service.Shift
            }).ToList();
        }

        public bool DischargeSoldier(int soldierId, Unit unit)
        {
            var soldier = _soldierRepository.FindSoldierById(soldierId);
            if (soldier.Unit.UnitId != unit.UnitId) return false;
            _soldierRepository.UpdateDischargedStatusById(soldierId, true);
            return true;
        }

        public List<SoldierServiceStatDto> GetSoldierServiceStats(Unit unit, StatisticalData caseType)
        {
            Expression<Func<Soldier, bool>> soldierPredicate = s => !s.Discharged && s.Unit.UnitId == unit.UnitId;

            Func<MilitaryServices.App.Entity.Service, bool> servicePredicate = svc => true;

            switch (caseType)
            {
                case StatisticalData.ARMED_SERVICES_ARMED_SOLDIERS:
                    servicePredicate = svc => svc.Armed.ToLower() == Situation.ARMED.ToString().ToLower();
                    break;

                case StatisticalData.UNARMED_SERVICES_ARMED_SOLDIERS:
                    servicePredicate = svc => svc.Armed.ToLower() == Situation.UNARMED.ToString().ToLower();
                    soldierPredicate = soldierPredicate.AndAlso(s => s.Situation.ToLower() == Situation.ARMED.ToString().ToLower());
                    break;

                case StatisticalData.UNARMED_SERVICES_UNARMED_SOLDIERS:
                    servicePredicate = svc => svc.Armed.ToLower() == Situation.UNARMED.ToString().ToLower();
                    soldierPredicate = soldierPredicate.AndAlso(s => s.Situation.ToLower() == Situation.UNARMED.ToString().ToLower());
                    break;

                case StatisticalData.FREE_OF_DUTY_SERVICES_ALL_SOLDIERS:
                    servicePredicate = svc => svc.Armed.ToLower() == ActiveExtensions.GetFreeOfDuty().ToLower();
                    break;
            }

            return _soldierRepository.GetSoldierServiceStatisticalData(soldierPredicate, servicePredicate);
        }

        public void DeleteServices(JsonElement servicesJson)
        {
            var ids = new List<long>();
            foreach (var node in servicesJson.EnumerateArray())
            {
                if (node.TryGetInt64(out var id))
                    ids.Add(id);
            }
            _serOfUnitRepository.DeleteAllById(ids);
        }
    }
}
