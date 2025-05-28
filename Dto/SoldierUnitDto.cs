using MilitaryServices.App.Entity;

namespace MilitaryServices.App.Dto
{
    public class SoldierUnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Situation { get; set; }
        public string Active { get; set; }
        public Unit Unit { get; set; }

        public SoldierUnitDto() { }

        public SoldierUnitDto(int id, string name, string surname, string situation, string active, Unit unit)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            Unit = unit;
        }
    }
}
