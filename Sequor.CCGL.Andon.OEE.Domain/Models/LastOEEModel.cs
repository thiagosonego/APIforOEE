using System.Collections.Generic;

namespace OEE.Domain.Models
{
    public class LastOEEModel
    {
        public LastOEEModel()
        {
            StationName = "";
            LastPeriods = new List<LastPeriodsModel>();
        }
        public string StationName { get; set; }
        public List<LastPeriodsModel> LastPeriods { get; set; }
    }
}
