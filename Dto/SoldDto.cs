namespace MilitaryServices.App.Dto
{
    public class SoldDto
    {
        private int _id;
        private string _name;
        private string _surname;
        private string _situation;
        private string _active;

        public SoldDto(int id, string name, string surname, string situation, string active)
        {
            _id = id;
            _name = name;
            _surname = surname;
            _situation = situation;
            _active = active;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Surname
        {
            get => _surname;
            set => _surname = value;
        }

        public string Situation
        {
            get => _situation;
            set => _situation = value;
        }

        public string Active
        {
            get => _active;
            set => _active = value;
        }
    }
}
