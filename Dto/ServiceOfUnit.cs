namespace MilitaryServices.App.Dto
{
    public class ServiceOfUnitDto
    {
        private long? _id;
        private string _service;
        private string _armed;
        private string _description;
        private string _shift;

        public ServiceOfUnitDto() { }

        public ServiceOfUnitDto(long? id, string service, string armed, string description, string shift)
        {
            _id = id;
            _service = service;
            _armed = armed;
            _description = description;
            _shift = shift;
        }

        public long? Id
        {
            get => _id;
            set => _id = value;
        }

        public string Service
        {
            get => _service;
            set => _service = value;
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
