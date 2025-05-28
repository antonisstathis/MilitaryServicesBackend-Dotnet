namespace MilitaryServices.App.Dto
{
    public class HistoricalData
    {
        public int SoldierId { get; set; }
        public long NumberOfServices { get; set; }

        public HistoricalData()
        {
        }

        public HistoricalData(int soldierId, long numberOfServices)
        {
            SoldierId = soldierId;
            NumberOfServices = numberOfServices;
        }
    }
}
