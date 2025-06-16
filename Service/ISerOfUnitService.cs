using System;
using System.Collections.Generic;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Service
{
    public interface ISerOfUnitService
    {
        List<ServiceOfUnitDto> GetAllServices(Unit unit, DateTime? prevDate);
        bool CheckIfAllowed(Unit unit, int numberOfGuards, ServiceOfUnit serviceOfUnit);
        ServiceOfUnit SaveService(ServiceOfUnit serviceOfUnit);
    }
}
