
using GardenApi.Models;
using hydrotronics_api;
using hydrotronics_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Device.Gpio;
using System.Text.Json;


namespace GardenApi.services;

public class DeviceService
{
    private readonly IHubContext<DeviceHub> _hubContext;

    public DeviceService(
        IOptions<GardenDatabaseSettings> gardenDatabaseSettings, IHubContext<DeviceHub> hubContext)
    {
      

        _hubContext = hubContext;

    }
    public async Task<bool> Relay1Off()
    {
        var currentState = await GetAsync();
        /*   int pin = 24;
           using var controller = new GpioController();
           controller.OpenPin(pin, PinMode.Output);
           controller.Write(pin, PinValue.High);*/
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay1/off";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var updatedDeviceState = new Device
                    {
                        Relay1 = 0,
                        Relay2 = currentState.Relay2,
                        Relay3 = currentState.Relay3,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Pump Turned off");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

      
        return true;
    }
  
    public async Task<bool> Relay1On()
    {

        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay1/on";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = 1,
                        Relay2 = currentState.Relay2,
                        Relay3 = currentState.Relay3,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Pump Turned on");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    
      /*  int pin = 24;
        using var controller = new GpioController();
        controller.OpenPin(pin, PinMode.Output);
        controller.Write(pin, PinValue.Low);*/
        return true;
    }
    public async Task<bool> Relay2Off()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay2/off";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {


                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = 0,
                        Relay3 = currentState.Relay3,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Relat 2 Turned off");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
       
     /*   int pin = 17;
        using var controller = new GpioController();
        controller.OpenPin(pin, PinMode.Output);
        controller.Write(pin, PinValue.High);*/
     
        return true;
    }

    public async Task<bool> Relay2On()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay2/on";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = 1,
                        Relay3 = currentState.Relay3,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Light Turned on");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return true;
    }
    public async Task<bool> Relay3On()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay3/on";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = currentState.Relay2,
                        Relay3 = 1,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Exhaust turned on");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return true;
    }
    public async Task<bool> Relay3Off()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay3/off";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = currentState.Relay2,
                        Relay3 = 0,
                        Relay4 = currentState.Relay4
                    };
                    await UpdateAsync(updatedDeviceState, "Exhaust turned off");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return true;
    }

    public async Task<bool> Relay4On()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay4/on";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = currentState.Relay2,
                        Relay3 = currentState.Relay3,
                        Relay4 = 1
                    };
                    await UpdateAsync(updatedDeviceState, "Fan turned on");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
     
      /*  int pin = 27;
        using var controller = new GpioController();
        controller.OpenPin(pin, PinMode.Output);
        controller.Write(pin, PinValue.Low);*/

        return true;
    }
    public async Task<bool> Relay4Off()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            UseProxy = false
        };

        try
        {
            string url = "http://192.168.12.246:80/relay4/off";
            using (HttpClient client = new HttpClient(handler))
            {
                Console.WriteLine("Using URL: " + url);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    var currentState = await GetAsync();
                    var updatedDeviceState = new Device
                    {
                        Relay1 = currentState.Relay1,
                        Relay2 = currentState.Relay2,
                        Relay3 = currentState.Relay3,
                        Relay4 = 0
                    };
                    await UpdateAsync(updatedDeviceState, "Fan turned off");
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        /*  int pin = 27;
          using var controller = new GpioController();
          controller.OpenPin(pin, PinMode.Output);
          controller.Write(pin, PinValue.High);*/
     
        return true;
    }
    public async Task<Device?> GetAsync()
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
        {
            connection.Open();

            var _device = new Device();
            var command = connection.CreateCommand();

            command.CommandText = @"
        SELECT *
        FROM device
        WHERE Id = @id
    ";
            command.Parameters.AddWithValue("@id", 1); 

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    var columnIndexes = new Dictionary<string, int>();
                    columnIndexes.Add("Relay1", reader.GetOrdinal("Relay1"));
                    columnIndexes.Add("Relay2", reader.GetOrdinal("Relay2")); 
                    columnIndexes.Add("Relay3", reader.GetOrdinal("Relay3"));
                    columnIndexes.Add("Relay4", reader.GetOrdinal("Relay4"));

                    foreach (var keyValuePair in columnIndexes)
                    {
                        string propertyName = keyValuePair.Key;
                        int columnIndex = keyValuePair.Value;

                        var property = _device.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            property.SetValue(_device, reader.GetValue(columnIndex));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No device found!"); 
                    return null;
                }
            }

            connection.Close();
            return _device;
        }
    }

    private static string lastMessage = null; 
    private static DateTime lastMessageTime = DateTime.MinValue; 
    private static readonly TimeSpan messageCooldown = TimeSpan.FromSeconds(2);  

    public async Task UpdateAsync(Device updateDeviceState, string message)
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"UPDATE device
                    SET
                      Relay1 = @relay1,
                      Relay2 = @relay2,
                      Relay3 = @relay3,
                      Relay4 = @relay4
                    WHERE Id = @id;";

                    command.Prepare();

                    command.Parameters.AddWithValue("@id", 1);
                    command.Parameters.AddWithValue("@relay1", updateDeviceState.Relay1);
                    command.Parameters.AddWithValue("@relay2", updateDeviceState.Relay2);
                    command.Parameters.AddWithValue("@relay3", updateDeviceState.Relay3);
                    command.Parameters.AddWithValue("@relay4", updateDeviceState.Relay4);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                    connection.Close();
                    await _hubContext.Clients.All.SendAsync("DeviceUpdated", updateDeviceState);


                    Console.WriteLine("Device updated successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating device: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }
    }



}
   