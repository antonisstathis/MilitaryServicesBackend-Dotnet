using System;

namespace MilitaryServices.App.Dto
{
    public class ServiceDto
    {
        private long _id;
        private string _service;
        private DateTime _serviceDate;
        private string _armed;
        private string _description;
        private string _shift;

        public ServiceDto(long id, string service, DateTime serviceDate, string armed, string description, string shift)
        {
            _id = id;
            _service = service;
            _serviceDate = serviceDate;
            _armed = armed;
            _description = description;
            _shift = shift;
        }

        public long Id
        {
            get => _id;
            set => _id = value;
        }

        public string Service
        {
            get => _service;
            set => _service = value;
        }

        public DateTime ServiceDate
        {
            get => _serviceDate;
            set => _serviceDate = value;
        }

        public string Armed
        {
            get => _armed;
            set => _armed = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public string Shift
        {
            get => _shift;
            set => _shift = value;
        }
    }
}
