
using Microsoft.AspNetCore.Mvc;
using System.Device.Gpio;
using GardenApi.services;
using hydrotronics_api.Models;
using hydrotronics_api.Services;
using static hydrotronics_api.Models.Co2Concentration;

namespace hydrotronics_api.Controllers
{

    public class SensorController : Controller
    {
        private readonly SensorService _sensorService;

        public SensorController(SensorService sensorService) =>
            _sensorService = sensorService;

        [HttpGet("getTemperatureData")]
        public async Task<ActionResult<TemperatureSensor>> GetTemperatureData()
        {
            var sensorData = await _sensorService.GetTemperatureData();
            return sensorData;
        }


        [HttpGet("getTemperatureDataLast24Hours")]
        public async Task<ActionResult<List<TemperatureReading>>> GetTemperatureDataLast24Hours([FromQuery] DateTime date)
        {
            return await _sensorService.GetReadingsForLast24HoursOfDay(date);
        }


        [HttpGet("getCo2DataLast24Hours")]
        public async Task<ActionResult<List<Co2ConcentrationReading>>> GetCo2DataLast24Hours([FromQuery] DateTime date)
        {
            return await _sensorService.GetCo2ReadingsForLast24HoursOfDay(date);
        }


        [HttpPost("getTemperatureReadings")]
        public async Task<ActionResult<List<DailyTemperatureReading>>> GetReadings([FromBody] PageRequest pageNumber)
        {
            return await _sensorService.GetReadings(pageNumber.PageNumber);
        }

        [HttpPost("getCo2eReadings")]
        public async Task<ActionResult<List<DailyCo2ConcentrationeReading>>> GetCo2eReadings([FromBody] PageRequest pageNumber)
        {
            return await _sensorService.GetCo2Readings(pageNumber.PageNumber);
        }

        [HttpGet("getTotalPages")]
        public async Task<ActionResult<int>> GetTotalPages()
        {
            return await _sensorService.GetTotalPages();
        }

        [HttpGet("getCo2Concentration")]
        public async Task<ActionResult<Co2ConcentrationSensor>> GetCo2Concentration()
        {
            return await _sensorService.GetCo2Concentration();

        }

    }
}