using System;
using System.Collections.Generic;
using System.Linq;
using MilitaryServices.App.Dao;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Enums;

namespace MilitaryServices.App.Service
{
    public class SerOfUnitService : ISerOfUnitService
    {
        private readonly IServiceOfUnitRepository _serOfUnitRepository;
        private readonly ISoldierRepository _soldierAccess;
        private readonly ServiceRepository _serviceRepository;

        public SerOfUnitService(
            IServiceOfUnitRepository serOfUnitRepository,
            ISoldierRepository soldierAccess,
            ServiceRepository serviceRepository)
        {
            _serOfUnitRepository = serOfUnitRepository;
            _soldierAccess = soldierAccess;
            _serviceRepository = serviceRepository;
        }

        public List<ServiceOfUnitDto> GetAllServices(Unit unit, DateTime? prevDate)
        {
            if (prevDate.HasValue)
                return GetPrevServices(unit, prevDate.Value);

            var allServices = _serOfUnitRepository.FindByUnit(unit);
            return allServices.Select(service => new ServiceOfUnitDto(
                service.Id,
                service.ServiceName,
                service.Armed,
                service.Description,
                service.Shift
            )).ToList();
        }

        private List<ServiceOfUnitDto> GetPrevServices(Unit unit, DateTime prevDate)
        {
            var services = _serviceRepository.FindByUnitAndDate(unit, prevDate);
            return [.. services.Select(service => new ServiceOfUnitDto(
                service.Id,
                service.ServiceName,
                service.Armed,
                service.Description,
                service.Shift
            ))];
        }

        public bool CheckIfAllowed(Unit unit, int numberOfGuards, ServiceOfUnit serviceOfUnit)
        {
            DateTime dateOfLastCalculation = _soldierAccess.GetDateOfLastCalculation(unit);
            List<Soldier> allSoldiers = _soldierAccess.LoadSold(unit, dateOfLastCalculation);

            var armedSoldiers = allSoldiers.Where(s => s.IsArmed).ToList();
            var unarmedSoldiers = allSoldiers.Where(s => !s.IsArmed).ToList();

            var armedServices = _serOfUnitRepository.FindByUnitAndArmed(unit, Situation.ARMED.ToString().ToLower());
            var unarmedServices = _serOfUnitRepository.FindByUnitAndArmed(unit, Situation.UNARMED.ToString().ToLower());

            int allServicesCount = armedServices.Count + unarmedServices.Count;

            bool canProceed =
                allSoldiers.Count >= (allServicesCount + numberOfGuards) &&
                (serviceOfUnit.Armed
                    ? armedSoldiers.Count >= (armedServices.Count + numberOfGuards)
                    : unarmedSoldiers.Count >= (unarmedServices.Count + numberOfGuards));

            return canProceed;
        }

        public ServiceOfUnit SaveService(ServiceOfUnit serviceOfUnit)
        {
            return _serOfUnitRepository.Save(serviceOfUnit);
        }
    }
}
