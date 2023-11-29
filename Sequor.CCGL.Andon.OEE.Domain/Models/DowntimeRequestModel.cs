using System.Collections.Generic;

namespace OEE.Domain.Models
{
    public class DowntimeRequestModel
    {
        public List<StationRequestModel> StationRequest { get; set; }
        public string Period { get; set; }
        public int Quantity { get; set; }
    }
}
