using System.Collections.Generic;

namespace OEE.Domain.Models
{
    public class DowntimeModel
    {
        public DowntimeModel()
        {
            Stops = new List<DowntimeStopsModel>();
        }
        public string StationName { get; set; }
        public List<DowntimeStopsModel> Stops { get; set; }
    }
}
