using System.ComponentModel.DataAnnotations;

namespace MilitaryServices.App.Dto
{
    public class SoldierPersonalDataDto
    {
        private string _token;
        private string _soldierRegistrationNumber;
        private string _company;
        private string _name;
        private string _surname;
        private string _situation;
        private string _active;
        private string _discharged;
        private string _patronymic;
        private string _matronymic;
        private string _mobilePhone;
        private string _city;
        private string _address;

        public SoldierPersonalDataDto() { }

        public SoldierPersonalDataDto(string token, string soldierRegistrationNumber, string company, string name,
            string surname, string situation, string active, string discharged,
            string patronymic, string matronymic, string mobilePhone, string city, string address)
        {
            _token = token;
            _soldierRegistrationNumber = soldierRegistrationNumber;
            _company = company;
            _name = name;
            _surname = surname;
            _situation = situation;
            _active = active;
            _discharged = discharged;
            _patronymic = patronymic;
            _matronymic = matronymic;
            _mobilePhone = mobilePhone;
            _city = city;
            _address = address;
        }

        public string Token
        {
            get => _token;
            set => _token = value;
        }

        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Registration number must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF0-9]+$", ErrorMessage = "Name must only contain alphabetic characters (a-z, A-Z).")]
        public string SoldierRegistrationNumber
        {
            get => _soldierRegistrationNumber;
            set => _soldierRegistrationNumber = value;
        }

        [Required(ErrorMessage = "Company is required")]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "Company must be a number.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "The company must only contain numbers.")]
        public string Company
        {
            get => _company;
            set => _company = value;
        }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF]+$", ErrorMessage = "Name must only contain alphabetic characters (a-z, A-Z).")]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [Required(ErrorMessage = "Surname is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Surname must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF]+$", ErrorMessage = "Surname must only contain alphabetic characters (a-z, A-Z).")]
        public string Surname
        {
            get => _surname;
            set => _surname = value;
        }

        [RegularExpression("armed|unarmed", ErrorMessage = "Situation must be 'armed' or 'unarmed'")]
        public string Situation
        {
            get => _situation;
            set => _situation = value;
        }

        [RegularExpression("active|inactive", ErrorMessage = "Status must be 'active' or 'inactive'")]
        public string Active
        {
            get => _active;
            set => _active = value;
        }

        public string Discharged
        {
            get => _discharged;
            set => _discharged = value;
        }

        [Required(ErrorMessage = "Patronymic is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Patronymic must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF]+$", ErrorMessage = "Patronymic must only contain alphabetic characters (a-z, A-Z).")]
        public string Patronymic
        {
            get => _patronymic;
            set => _patronymic = value;
        }

        [Required(ErrorMessage = "Matronymic is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Matronymic must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF]+$", ErrorMessage = "Matronymic must only contain alphabetic characters (a-z, A-Z).")]
        public string Matronymic
        {
            get => _matronymic;
            set => _matronymic = value;
        }

        [Required(ErrorMessage = "Mobile phone is required")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Mobile phone must be between 5 and 15 characters.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "The mobile phone must only contain numbers.")]
        public string MobilePhone
        {
            get => _mobilePhone;
            set => _mobilePhone = value;
        }

        [Required(ErrorMessage = "City is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "City must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF]+$", ErrorMessage = "City must only contain alphabetic characters (a-z, A-Z).")]
        public string City
        {
            get => _city;
            set => _city = value;
        }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 15 characters.")]
        [RegularExpression("^[a-zA-Z\u0370-\u03FF0-9]+$", ErrorMessage = "The address must only contain letters (a-z, A-Z) and numbers (0-9).")]
        public string Address
        {
            get => _address;
            set => _address = value;
        }
    }
}
