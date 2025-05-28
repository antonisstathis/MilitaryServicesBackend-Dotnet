using System.Collections.Generic;

namespace MilitaryServices.App.Dto
{
    public class CountServices
    {
        public Dictionary<int, HistoricalData> HdForArmed { get; set; }
        public Dictionary<int, HistoricalData> HdForUnarmed { get; set; }
        public Dictionary<int, HistoricalData> HdForOut { get; set; }

        public CountServices()
        {
            HdForArmed = [];
            HdForUnarmed = [];
            HdForOut = [];
        }

        public CountServices(
            Dictionary<int, HistoricalData> hdForArmed,
            Dictionary<int, HistoricalData> hdForUnarmed,
            Dictionary<int, HistoricalData> hdForOut)
        {
            HdForArmed = hdForArmed;
            HdForUnarmed = hdForUnarmed;
            HdForOut = hdForOut;
        }
    }
}
