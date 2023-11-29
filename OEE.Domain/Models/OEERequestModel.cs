using System.Collections.Generic;

namespace OEE.Domain.Models
{
    public class OEERequestModel
    {
        public List<StationRequestModel> StationRequest { get; set; }
        public string Period { get; set; }
    }
}
