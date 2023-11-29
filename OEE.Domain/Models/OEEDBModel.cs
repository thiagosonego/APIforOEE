namespace OEE.Domain.Models
{
    public class OEEDBModel
    {
        public string station { get; set; }
        public int partsOk { get; set; }
        public int partsRefused { get; set; }
        public int partsReworked { get; set; }
        public int timeAvailableTurn { get; set; }
        public int timeStopWithoutDescription { get; set; }
        public int timeStop { get; set; }
        public int timeBroken { get; set; }
        public int goalProduction { get; set; }
    }
}
