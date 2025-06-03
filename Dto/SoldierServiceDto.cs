using System;
using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dto
{
    public class SoldierServiceDto
    {
        public int Id { get; set; }
        public string SoldierRegistrationNumber { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Situation { get; set; }
        public string Active { get; set; }
        public long ServiceId { get; set; }
        public string Service { get; set; }
        public DateTime Date { get; set; }
        public Unit Unit { get; set; }  
        public string Armed { get; set; }
        public bool Discharged { get; set; }
        public string Description { get; set; }
        public string Shift { get; set; }

        public SoldierServiceDto()
        {
            
        }

        public SoldierServiceDto(
            int id,
            string company,
            string soldierRegistrationNumber,
            string name,
            string surname,
            string situation,
            string active,
            long serviceId,
            string service,
            DateTime date,
            string armed,
            Unit unit,
            bool discharged)
        {
            Id = id;
            Company = company;
            SoldierRegistrationNumber = soldierRegistrationNumber;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            ServiceId = serviceId;
            Service = service;
            Date = date;
            Armed = armed;
            Unit = unit;
            Discharged = discharged;
        }

        public SoldierServiceDto(
            int id,
            string company,
            string soldierRegistrationNumber,
            string name,
            string surname,
            string situation,
            string active,
            long serviceId,
            string service,
            DateTime date,
            string armed,
            Unit unit,
            bool discharged,
            string description,
            string shift)
            : this(id, company, soldierRegistrationNumber, name, surname, situation, active, serviceId, service, date, armed, unit, discharged)
        {
            Description = description;
            Shift = shift;
        }

        public string GetFormattedDate()
        {
            return Date.ToString("dd-MM-yyyy");
        }
    }
}
