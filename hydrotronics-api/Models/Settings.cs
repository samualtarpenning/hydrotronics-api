using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hydrotronics_api.Models

{
    public class Settings
    {
        public string Id { get; set; }
        public long ShouldStop { get; set; }
        public long PumpOnTime { get; set; }
        public long PumpOffTime { get; set; }
        public long PumpShouldStop { get; set; }
        public long LightOnTime { get; set; }
        public long LightOffTime { get; set;}
        public long LightShouldStop { get; set; }
        public long ExhaustOnTime { get; set; }
        public long ExhaustOffTime { get;set; }
        public long ExhaustShouldStop { get; set; }
        public long FanOnTime { get; set; }
        public long FanOffTime { get;  set; }
        public long FanShouldStop { get; set; }

    }
}
