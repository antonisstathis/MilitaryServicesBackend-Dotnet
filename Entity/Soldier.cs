using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MilitaryServices.App.Enums;

namespace MilitaryServices.App.Entity
{
    [Table("soldiers", Schema = "ms")]
    public class Soldier
    {
        [Key]
        [Column("sold_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("soldierRegistrationNumber")]
        public string SoldierRegistrationNumber { get; set; }

        [Required(ErrorMessage = "is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "enter at least 3 characters")]
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "enter at least 3 characters")]
        [Column("surname")]
        public string Surname { get; set; }

        [Column("situation")]
        public string Situation { get; set; }

        [Column("active")]
        public string Active { get; set; }

        [Column("discharged")]
        public bool Discharged { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("patronymic")]
        public string Patronymic { get; set; }

        [Column("matronymic")]
        public string Matronymic { get; set; }

        [Column("mobile_phone")]
        public string MobilePhone { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [ForeignKey("UnitId")]
        [Column("unit_id")]
        public int? UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual List<Service> Services { get; set; } = new();

        public Soldier() { }

        public Soldier(int id)
        {
            Id = id;
        }

        public Soldier(int id, string name, string surname, string situation, string active, bool discharged)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            Discharged = discharged;
        }

        public Soldier(int id, string company, string soldierRegistrationNumber, string name, string surname, string situation, string active, bool discharged)
        {
            Id = id;
            Company = company;
            SoldierRegistrationNumber = soldierRegistrationNumber;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            Discharged = discharged;
        }

        public Soldier(int id, string soldierRegistrationNumber, string name, string surname, string situation, string active, List<Service> services, bool discharged, string company, string patronymic, string matronymic, string mobilePhone, string city, string address, Unit unit)
        {
            Id = id;
            SoldierRegistrationNumber = soldierRegistrationNumber;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            Services = services ?? new List<Service>();
            Discharged = discharged;
            Company = company;
            Patronymic = patronymic;
            Matronymic = matronymic;
            MobilePhone = mobilePhone;
            City = city;
            Address = address;
            Unit = unit;
            UnitId = (int?)unit?.Id;
        }

        [NotMapped]
        public bool IsActive => string.Equals(Active, Enums.Active.ACTIVE.ToString(), StringComparison.OrdinalIgnoreCase);

        [NotMapped]
        public bool IsArmed => string.Equals(Situation, Enums.Situation.ARMED.ToString(), StringComparison.OrdinalIgnoreCase);

        [NotMapped]
        public bool IsOut => Services.Count > 0 && string.Equals(Services[0].ServiceName, "out", StringComparison.OrdinalIgnoreCase);

        [NotMapped]
        public bool IsAvailable => Services.Count > 0 && string.Equals(Services[0].ServiceName, "available", StringComparison.OrdinalIgnoreCase);

        public void SetService(Service service)
        {
            Services.Clear();
            Services.Add(service);
        }

        public Service GetService()
        {
            return Services.Count > 0 ? Services[0] : null;
        }

        public void Print()
        {
            string s = $"{Name} {Surname} {Situation} {Active}";
            Console.WriteLine(s);
        }
    }
}
