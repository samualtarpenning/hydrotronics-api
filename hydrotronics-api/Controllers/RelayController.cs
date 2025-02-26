
using Microsoft.AspNetCore.Mvc;
using System.Device.Gpio;
using GardenApi.services;
using hydrotronics_api.Models;

namespace hydrotronics_api.Controllers
{

    public class RelayController : Controller
    {
        private readonly DeviceService _deviceService;


        public RelayController(DeviceService deviceService) =>
            _deviceService = deviceService;

        [HttpGet("relay1On")]
        public async Task<ActionResult<bool>> Relay1On()
        {
            await _deviceService.Relay1On();
            return Ok(true);
        }
        [HttpGet("getDeviceState")]
        public async Task<ActionResult<Device>> GetDeviceState()
        {
            var currentState = await _deviceService.GetAsync();
            
            return Ok(currentState);
        }

        [HttpGet("relay1Off")]
        public async Task<ActionResult<bool>> Relay1Off()
        {
            await _deviceService.Relay1Off();
            return Ok(true);
        }
    

        [HttpGet("relay2On")]
        public async Task<ActionResult<bool>> Relay2On()
        {
            await _deviceService.Relay2On();
            return Ok(true);
        }


        [HttpGet("relay2Off")]
        public async Task<ActionResult<bool>> Relay2Off()
        {
            await _deviceService.Relay2Off();
            return Ok(false);
        }

        [HttpGet("relay3On")]
        public async Task<ActionResult<bool>> Relay3On()
        {
            await _deviceService.Relay3On();
        return Ok(true);
        }


        [HttpGet("relay3Off")]
        public async Task<ActionResult<bool>> Relay3Off()
        {
            await _deviceService.Relay3Off();
            return true;
        }
        [HttpGet("relay4On")]
        public async Task<ActionResult<bool>> Relay4On()
        {
            await _deviceService.Relay4On();
            return Ok(true);    
        }


        [HttpGet("relay4Off")]
        public async Task<ActionResult<bool>> Relay4Off()
        {
           await _deviceService.Relay4Off();
        return true;
        }
    }
}
