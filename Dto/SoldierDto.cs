using System;
using System.Globalization;

namespace MilitaryServices.App.Dto
{
    public class SoldierDto
    {
        private string _token;
        private string _company;
        private string _name;
        private string _surname;
        private string _situation;
        private string _active;
        private string _service;
        private DateTime _date;
        private string _armed;

        public SoldierDto(string name, string surname, string situation, string active)
        {
            _name = name;
            _surname = surname;
            _situation = situation;
            _active = active;
        }

        public SoldierDto(string token, string company, string name, string surname, string situation, string active, string service, DateTime date, string armed)
        {
            _token = token;
            _company = company;
            _name = name;
            _surname = surname;
            _situation = situation;
            _active = active;
            _service = service;
            _date = date;
            _armed = armed;
        }

        public string Token
        {
            get => _token;
            set => _token = value;
        }

        public string Company
        {
            get => _company;
            set => _company = value;
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

        public string Service
        {
            get => _service;
            set => _service = value;
        }

        // Returns formatted date as string
        public string Date
        {
            get => _date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        // Returns raw DateTime
        public DateTime ExtractDate()
        {
            return _date;
        }

        public string Armed
        {
            get => _armed;
            set => _armed = value;
        }

        public void SetDate(DateTime date)
        {
            _date = date;
        }
    }
}
