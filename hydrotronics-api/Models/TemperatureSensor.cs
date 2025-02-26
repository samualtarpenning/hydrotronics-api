namespace hydrotronics_api.Models
{
    public class TemperatureSensor
    {
        public float humidity { get; set; }
        public float temperature { get; set; }
    }
    public class TemperatureReading
    {
        public int Id { get; set; }
        public string DateTime { get; set; }     
        public float Temperature { get; set; }  
        public float Humidity { get; set; }         
    }

    public class DailyTemperatureReading
    {
        public DateTime Date { get; set; }
        public float MaxTemperature { get; set; }
        public float MinTemperature { get; set; }
        public float AvgTemperature { get; set; }
        public float MaxHumidity { get; set; }
        public float MinHumidity { get; set; }
        public float AvgHumidity { get; set; }
    }
    public class PageRequest
    {
        public int PageNumber { get; set; }
    }
}
