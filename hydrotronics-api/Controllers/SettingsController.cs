using GardenApi.services;
using hydrotronics_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace hydrotronics_api.Controllers
{
    public class SettingsController : Controller
    {
        private readonly SettingsService _settingsService;

        public SettingsController(SettingsService settingsService) =>
            _settingsService = settingsService;


        [HttpPost("updateSettings")] 
        public async Task<ActionResult<bool>> UpdateSettings([FromBody] Settings newSettings)
        {
            await _settingsService.UpdateAsync("661c668df067539a582ee9be", newSettings);
            return true;
        }
        [HttpGet("settings")]
        public async Task<Settings> GetSettings()
        {
            var settings =  await _settingsService.GetAsync();
            return settings;
        }
        [HttpGet("createSettingsTable")]
        public async Task<bool> GetSettingsTable()
        {
            var settings = await _settingsService.CreateSettingsTable();
            return settings;
        }
        [HttpGet("createDeviceTable")]
        public async Task<bool> GetDeviceTable()
        {
            var settings = await _settingsService.CreateSettingsTable();
            return settings;
        }
        [HttpGet("startTimers")]
        public async Task<ActionResult<bool>> StartTimers()
        {
            _settingsService.StartPump();
            _settingsService.StartLight();
            _settingsService.StartExhaust();
            _settingsService.StartFan();
            return true;
        }
        [HttpGet("startPumpTimers")]
        public async Task<ActionResult<bool>> StartPumpTimer()
        {
             var res = await _settingsService.StartPump();
            return res;
           
            
        }

        [HttpGet("startLightTimers")]
        public async Task<ActionResult<bool>> StartLightTimer()
        {
          
            await _settingsService.StartLight();
            return true;
        }

        [HttpGet("startFanTimers")]
        public async Task<ActionResult<bool>> StartFanTimers()
        {

            _settingsService.StartFan();
            return true;
        }


        [HttpGet("startExhaustTimers")]
        public async Task<ActionResult<bool>> StartExaustTimer()
        {

            _settingsService.StartExhaust();
            return true;
        }


        [HttpGet("stopTimers")]
        public async Task<ActionResult<bool>> StopTimers()
        {
            _settingsService.StopAllTimers();
            return true;
        }
        [HttpGet("createTables")]
        public async Task<ActionResult<bool>> CreateTables()
        {
            await _settingsService.CreateSettingsTable();
            await _settingsService.CreateDeviceTable();
            return true;
        }

    }

}
