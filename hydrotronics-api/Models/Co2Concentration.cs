namespace hydrotronics_api.Models
{
    public class Co2Concentration
    {
        public class Co2ConcentrationSensor
        {
            public float ppm { get; set; }
        }
        public class Co2ConcentrationReading
        {
            public int Id { get; set; }
            public string DateTime { get; set; }
            public float Ppm { get; set; }
        }
        public class DailyCo2ConcentrationeReading
        {
            public DateTime Date { get; set; }
            public float MaxPpm { get; set; }
            public float MinPpm { get; set; }
            public float AvgPpm { get; set; }
        }
    }
}
