namespace MilitaryServices.App.Dto
{
    public class SoldierPreviousServiceDto
    {
        public string Token { get; set; }
        public string SoldierRegistrationNumber { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Situation { get; set; }
        public string Active { get; set; }
        public string Service { get; set; }
        public string Date { get; set; }
        public string Armed { get; set; }
        public string Discharged { get; set; }

        public SoldierPreviousServiceDto() { }

        public SoldierPreviousServiceDto(
            string token,
            string soldierRegistrationNumber,
            string company,
            string name,
            string surname,
            string situation,
            string active,
            string service,
            string date,
            string armed,
            string discharged)
        {
            Token = token;
            SoldierRegistrationNumber = soldierRegistrationNumber;
            Company = company;
            Name = name;
            Surname = surname;
            Situation = situation;
            Active = active;
            Service = service;
            Date = date;
            Armed = armed;
            Discharged = discharged;
        }
    }
}
