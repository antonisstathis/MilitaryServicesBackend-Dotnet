namespace MilitaryServices.App.Dto
{
    public class SoldierServiceStatDto
    {
        public string SoldierRegNumber { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Active { get; set; }
        public string Situation { get; set; }
        public int NumberOfServices { get; set; }

        public SoldierServiceStatDto(string soldierRegNumber, string company, string name, string surname, string active, string situation, int numberOfServices)
        {
            SoldierRegNumber = soldierRegNumber;
            Company = company;
            Name = name;
            Surname = surname;
            Active = active;
            Situation = situation;
            NumberOfServices = numberOfServices;
        }
    }
}
