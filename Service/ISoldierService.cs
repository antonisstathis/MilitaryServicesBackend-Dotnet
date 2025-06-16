using System;
using System.Collections.Generic;
using System.Text.Json;
using MilitaryServices.App.Dto;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Enums;

namespace MilitaryServicesApp.Service
{
    public interface ISoldierService
    {
        List<SoldierDto> FindAll(string username);
        List<SoldierPersonalDataDto> LoadSoldiers(string username);
        List<SoldierPersonalDataDto> FindSoldiersByRegistrationNumber(string regNumber);
        List<SoldierPreviousServiceDto> FindPreviousCalculation(string username, DateTime date);
        DateTime GetDateByCalculationNumber(string username, int calculation);
        void SaveNewSoldier(SoldierPersonalDataDto dto, Unit unit);
        void CalculateServices(string username,DateTime lastDate);
        void UpdateSoldier(SoldDto dto);
        SoldierUnitDto FindSoldier(int id);
        List<ServiceDto> FindServicesOfSoldier(Unit unit, int soldierId);
        bool DischargeSoldier(int soldierId, Unit unit);
        List<SoldierServiceStatDto> GetSoldierServiceStats(Unit unit, StatisticalData caseType);
        void DeleteServices(JsonElement servicesJson);
    }
}
