
using GardenApi.Models;
using hydrotronics_api;
using hydrotronics_api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Device.Gpio;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Runtime;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;
namespace GardenApi.services;

public class SettingsService

{

    private static readonly string ConnectionString = "Data Source=hydrotronics-garden.db";
    private static SqliteConnection _connection;
    private readonly DeviceService _deviceService;
    private readonly IHubContext<DeviceHub> _hubContext;
    private static SqliteConnection Connection
{
  get
  {
    if (_connection == null)
    {
      _connection = new SqliteConnection(ConnectionString);
    }
    return _connection;
  }
}

    public SettingsService(
        IOptions<GardenDatabaseSettings> gardenDatabaseSettings, IHubContext<DeviceHub> hubContext, DeviceService deviceService)
    {
        
        _deviceService = deviceService;

        _hubContext = hubContext;
    }

    
    public async Task<bool> CreateSettingsTable ()
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics0garden.db"))
        {
            connection.Open();

            // 1. Check if table exists (optional)
            bool tableExists = CheckTableExists(connection, "settings");

            if (!tableExists)
            {
                // 1.a Create table if it doesn't exist
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"
            CREATE TABLE settings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ShouldStop INTEGER DEFAULT 0,
                PumpOnTime INTEGER DEFAULT 0,
                PumpOffTime INTEGER DEFAULT 0,
                PumpShouldStop INTEGER DEFAULT 0,
                LightOnTime INTEGER DEFAULT 0,
                LightOffTime INTEGER DEFAULT 0,
                LightShouldStop INTEGER DEFAULT 0,
                ExhaustOnTime INTEGER DEFAULT 0,
                ExhaustOffTime INTEGER DEFAULT 0,
                ExhaustShouldStop INTEGER DEFAULT 0,
                FanOnTime INTEGER DEFAULT 0,
                FanOffTime INTEGER DEFAULT 0,
                FanShouldStop INTEGER DEFAULT 0
            );
        ";

                createTableCommand.ExecuteNonQuery();
                Console.WriteLine("Settings table created successfully!");
            }

            // 2. Update or Insert settings
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
        INSERT OR REPLACE INTO settings (
            ShouldStop,
            PumpOnTime,
            PumpOffTime,
            PumpShouldStop,
            LightOnTime,
            LightOffTime,
            LightShouldStop,
            ExhaustOnTime,
            ExhaustOffTime,
            ExhaustShouldStop,
            FanOnTime,
            FanOffTime,
            FanShouldStop
        )
        VALUES (
            @shouldStop,
            @pumpOnTime,
            @pumpOffTime,
            @pumpShouldStop,
            @lightOnTime,
            @lightOffTime,
            @lightShouldStop,
            @exhaustOnTime,
            @exhaustOffTime,
            @exhaustShouldStop,
            @fanOnTime,
            @fanOffTime,
            @fanShouldStop
        )";

            // Set parameter values (replace with your actual values)
            updateCommand.Parameters.AddWithValue("@shouldStop", 1);
            updateCommand.Parameters.AddWithValue("@pumpShouldStop", 1);
            updateCommand.Parameters.AddWithValue("@pumpOnTime", 5000);
            updateCommand.Parameters.AddWithValue("@pumpOffTime", 2000);
            updateCommand.Parameters.AddWithValue("@lightOnTime", 3000);
            updateCommand.Parameters.AddWithValue("@lightOffTime", 3000);
            updateCommand.Parameters.AddWithValue("@lightShouldStop", 1);
            updateCommand.Parameters.AddWithValue("@exhaustOnTime", 4000);
            updateCommand.Parameters.AddWithValue("@exhaustOffTime", 4000);
            updateCommand.Parameters.AddWithValue("@exhaustShouldStop", 1); // Adjust as needed
            updateCommand.Parameters.AddWithValue("@fanOnTime", 5000);
            updateCommand.Parameters.AddWithValue("@fanOffTime", 5000);
            updateCommand.Parameters.AddWithValue("@fanShouldStop", 1); // Adjust as needed

            updateCommand.ExecuteNonQuery();

            Console.WriteLine("Settings updated/inserted successfully!"); // Or handle success/failure differently

            connection.Close(); 
            return tableExists;
        }

        // Helper function to check if table exists (optional)
        bool CheckTableExists(SqliteConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            var reader = command.ExecuteReader();
            return reader.HasRows;
        }

      
    }
    public async Task<bool> CreateDeviceTable()
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics0garden.db"))
        {
            connection.Open();

            // 1. Check if table exists (optional)
            bool tableExists = CheckTableExists(connection, "settings");

            if (!tableExists)
            {
                // 1.a Create table if it doesn't exist
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"
            CREATE TABLE settings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Relay1 INTEGER DEFAULT 0,
                Relay2 INTEGER DEFAULT 0,
                Relay3 INTEGER DEFAULT 0,
                Relay4 INTEGER DEFAULT 0,
            );
        ";

                createTableCommand.ExecuteNonQuery();
                Console.WriteLine("Settings table created successfully!");
            }

            // 2. Update or Insert settings
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
        INSERT OR REPLACE INTO settings (
            Relay1,
            Relay2,
            Relay3,
            Relay4
        )
        VALUES (
            @relay1,
            @relay2,
            @relay3,
            @relay4,
        )";

            // Set parameter values (replace with your actual values)
            updateCommand.Parameters.AddWithValue("@relay1", 0);
            updateCommand.Parameters.AddWithValue("@relay2", 0);
            updateCommand.Parameters.AddWithValue("@relay3", 0);
            updateCommand.Parameters.AddWithValue("@relay4", 0);

            updateCommand.ExecuteNonQuery();

            Console.WriteLine("Settings updated/inserted successfully!"); // Or handle success/failure differently

            connection.Close();
            return tableExists;
        }
        bool CheckTableExists(SqliteConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            var reader = command.ExecuteReader();
            return reader.HasRows;
        }
    }
        public async Task<Settings?> GetAsync()
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
        {
            connection.Open();

            var _settings = new Settings();
            var command = connection.CreateCommand();

            // Use parameterized query and avoid hardcoded ID
            command.CommandText = @"
        SELECT *
        FROM settings
        WHERE Id = @id
    ";
            command.Parameters.AddWithValue("@id", 1);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    var columnIndexes = new Dictionary<string, int>();
                    columnIndexes.Add("ShouldStop", reader.GetOrdinal("ShouldStop"));
                    columnIndexes.Add("PumpOnTime", reader.GetOrdinal("PumpOnTime"));
                    columnIndexes.Add("PumpOffTime", reader.GetOrdinal("PumpOffTime"));
                    columnIndexes.Add("PumpShouldStop", reader.GetOrdinal("PumpShouldStop"));
                    columnIndexes.Add("LightOnTime", reader.GetOrdinal("LightOnTime"));
                    columnIndexes.Add("LightOffTime", reader.GetOrdinal("LightOffTime"));
                    columnIndexes.Add("LightShouldStop", reader.GetOrdinal("LightShouldStop"));
                    columnIndexes.Add("ExhaustOnTime", reader.GetOrdinal("ExhaustOnTime"));
                    columnIndexes.Add("ExhaustOffTime", reader.GetOrdinal("ExhaustOffTime"));
                    columnIndexes.Add("ExhaustShouldStop", reader.GetOrdinal("ExhaustShouldStop"));
                    columnIndexes.Add("FanOnTime", reader.GetOrdinal("FanOnTime"));
                    columnIndexes.Add("FanOffTime", reader.GetOrdinal("FanOffTime"));
                    columnIndexes.Add("FanShouldStop", reader.GetOrdinal("FanShouldStop"));

                    foreach (var keyValuePair in columnIndexes)
                    {
                        string propertyName = keyValuePair.Key;
                        int columnIndex = keyValuePair.Value;
                        var property = _settings.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            property.SetValue(_settings, reader.GetValue(columnIndex));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No settings found!"); // Or handle missing settings differently
                    return null; // Indicate no settings found
                }
            }

            connection.Close();
            return _settings;
        }
    }


    public async Task UpdateAsync(string id, Settings updateDeviceState)
    {
        using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();

            // Update specific settings
            command.CommandText = @"
        UPDATE settings
        SET
          ShouldStop = @shouldStop,
          PumpOnTime = @pumpOnTime,
          PumpOffTime = @pumpOffTime,
          PumpShouldStop = @pumpShouldStop,
          LightOnTime = @lightOnTime,
          LightOffTime = @lightOffTime,
          LightShouldStop = @lightShouldStop,
          ExhaustOnTime = @exhaustOnTime,
          ExhaustOffTime = @exhaustOffTime,
          ExhaustShouldStop = @exhaustShouldStop,
          FanOnTime = @fanOnTime,
          FanOffTime = @fanOffTime,
          FanShouldStop = @fanShouldStop
        WHERE Id = @id
    ";

            command.Parameters.AddWithValue("@id", 1);
            command.Parameters.AddWithValue("@shouldStop", updateDeviceState.ShouldStop);
            command.Parameters.AddWithValue("@pumpOnTime", updateDeviceState.PumpOnTime);
            command.Parameters.AddWithValue("@pumpOffTime", updateDeviceState.PumpOffTime);
            command.Parameters.AddWithValue("@pumpShouldStop", updateDeviceState.PumpShouldStop);
            command.Parameters.AddWithValue("@lightOnTime", updateDeviceState.LightOnTime);
            command.Parameters.AddWithValue("@lightOffTime", updateDeviceState.LightOffTime);
            command.Parameters.AddWithValue("@lightShouldStop", updateDeviceState.LightShouldStop);
            command.Parameters.AddWithValue("@exhaustOnTime", updateDeviceState.ExhaustOnTime);
            command.Parameters.AddWithValue("@exhaustOffTime", updateDeviceState.ExhaustOffTime);
            command.Parameters.AddWithValue("@exhaustShouldStop", updateDeviceState.ExhaustShouldStop);
            command.Parameters.AddWithValue("@fanOnTime", updateDeviceState.FanOnTime);
            command.Parameters.AddWithValue("@fanOffTime", updateDeviceState.FanOffTime);
            command.Parameters.AddWithValue("@fanShouldStop", updateDeviceState.FanShouldStop);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine("No rows affected. Update might have failed (e.g., ID not found).");
                }
                else
                {
                    Console.WriteLine("Settings updated successfully!");
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("Error updating settings: " + ex.Message);
                // Handle the exception (e.g., log the error)
            }

            connection.Close();
        }
    }


    public async Task<bool> StartPump() // Now an async Task<bool> method
    {
       using (var connection = SettingsService.Connection) // Use the static Connection property
  {
    connection.Open();;

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT PumpShouldStop, PumpOnTime, PumpOffTime
                FROM settings
                WHERE id = $id
                     ";
            command.Parameters.AddWithValue("$id", 1);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var pumpShouldStop = reader.GetInt64(0);
                    var pumpOnTime = reader.GetInt64(1);
                    var pumpOffTime = reader.GetInt64(2);

                    var manager = new TimerManager(_hubContext, _deviceService);                   
                    await manager.AddPumpOnTask("Pump Task", () => Console.WriteLine("Running pump..."), pumpOnTime);
                    await _deviceService.Relay1On();
                    return true; 
                }
            }

            connection.Close();
        }

        return false; 
    }

    public async Task<bool> StartLight() 
    {
        using (var connection = SettingsService.Connection)
        {
            connection.Open(); ;

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT LightShouldStop, LightOnTime, LightOffTime
        FROM settings
        WHERE id = $id
    ";
            command.Parameters.AddWithValue("$id", 1);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var lightShouldStop = reader.GetInt64(0);
                    var lightOnTime = reader.GetInt64(1);
                    var lightOffTime = reader.GetInt64(2);
                    var manager = new TimerManager(_hubContext, _deviceService);
                    manager.AddLightOnTask("Light Task", () => Console.WriteLine("Running light..."), lightOnTime);

                    await _deviceService.Relay3On();
                    return true; 
                }
            }

            connection.Close();
        }

        return false; // Indicate failure if no settings found
    }


    public async Task<bool> StartFan() // Now an async Task<bool> method
    {
        using (var connection = SettingsService.Connection) // Use the static Connection property
        {
            connection.Open(); ;
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                                        SELECT FanShouldStop, FanOnTime, FanOffTime
                                        FROM settings
                                        WHERE id = $id
                                    ";
                command.Parameters.AddWithValue("$id", 1);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var fanShouldStop = reader.GetInt64(0);
                        var fanOnTime = reader.GetInt64(1);
                        var fanOffTime = reader.GetInt64(2);

                        var manager = new TimerManager(_hubContext, _deviceService);


                        await manager.AddFanOnTask("Fan Task", () => Console.WriteLine("Running fan"), fanOnTime);
                         await  _deviceService.Relay2On();

                        return true;
                    }
                }
            } 
            catch (Exception ex) { 
             Console.WriteLine(ex.ToString());
            }
           

            connection.Close();
        }

        return false; // Indicate failure if no settings found
    }

    public async Task<bool> StartExhaust() 
    {
        using (var connection = SettingsService.Connection) 
        {
            connection.Open(); ;

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT ExhaustShouldStop, ExhaustOnTime, ExhaustOffTime
        FROM settings
        WHERE id = $id
    ";
            command.Parameters.AddWithValue("$id", 1);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var exhaustShouldStop = reader.GetInt64(0);
                    var exhaustOnTime = reader.GetInt64(1);
                    var exhaustOffTime = reader.GetInt64(2);

                    var manager = new TimerManager(_hubContext, _deviceService);


                    await manager.AddExhaustOnTask("Exhaust Task", () => Console.WriteLine("Running exhaust..."), exhaustOnTime);
                   _deviceService.Relay4On();
                    return true;
                }
            }

            connection.Close();
        }

        return false; // Indicate failure if no settings found
    }


    public void StopAllTimers()
    {
       var manager = new TimerManager(_hubContext, _deviceService);
        manager.StopAll();
    }

    private class TimerManager
    {
        private readonly List<ScheduledTask> _tasks;
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly DeviceService _deviceService;

        public TimerManager(IHubContext<DeviceHub> hubContext, DeviceService deviceService)
        {

            _tasks = new List<ScheduledTask>();
        
            _hubContext = hubContext;

            _deviceService = deviceService;
        }

        public async void AddPumpOffTask(string name, Action task, long delayMs)
    {
            var _settings = new Settings();
            using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT PumpShouldStop, PumpOnTime, PumpOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
                var scheduledTask = new ScheduledTask
                {
                    Name = name,
                    Task = task,
                    DelayMs = _settings.PumpOnTime
                };
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        _settings.PumpShouldStop = reader.GetInt64(0);
                        _settings.PumpOnTime = reader.GetInt64(1);
                        _settings.PumpOffTime = reader.GetInt64(2);
                        task?.Invoke(); // Execute the provided task
                        connection.Close();
                        if (_settings.PumpShouldStop == 0)
                        {
                           
                            scheduledTask.Timer = new Timer(_ => StartPumpOnTimer(scheduledTask), null, _settings.PumpOffTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                             await _deviceService.Relay1Off();
                        } else
                        {
                            scheduledTask?.Timer?.Dispose();
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No settings found!");
                    } 
                   
                } 


            }

        }

        public async Task<bool> AddPumpOnTask(string name, Action task, long delayMs)
        {
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT PumpShouldStop, PumpOnTime, PumpOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _settings.PumpShouldStop = reader.GetInt64(0);
                        _settings.PumpOnTime = reader.GetInt64(1);
                        _settings.PumpOffTime = reader.GetInt64(2);
                        task?.Invoke(); 
                        connection.Close();

                        if (_settings.PumpShouldStop == 0)
                        {
                        var scheduledTask = new ScheduledTask
                        {
                            Name = name,
                            Task = task,
                            DelayMs = _settings.PumpOffTime
                        };

                        scheduledTask.Timer = new Timer(_ => StartPumpOffTimer(scheduledTask), null, _settings.PumpOnTime, Timeout.Infinite);
                        _tasks.Add(scheduledTask);
                        await  _deviceService.Relay1On();
                        }
                        else
                        {
                            var scheduledTask = new ScheduledTask
                            {
                                Name = name,
                                
                                Task = task,
                                DelayMs = _settings.PumpOffTime
                            };
                            scheduledTask?.Timer?.Dispose();
                        };
                      

                        return true;
                    }
                }
               
            }
            return true; 
        }






        public async Task<bool> AddLightOnTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding light on Task");
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT LightShouldStop, LightOnTime, LightOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
                var scheduledTask = new ScheduledTask
                {
                    Name = name,
                    Task = task,
                    DelayMs = _settings.LightOffTime
                };
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _settings.LightShouldStop = reader.GetInt64(0);
                        _settings.LightOnTime = reader.GetInt64(1);
                        _settings.LightOffTime = reader.GetInt64(2);
                        task?.Invoke();
                        connection.Close();

                        if (_settings.LightShouldStop == 0)
                        {
                            Console.WriteLine($"Executing task: {name}");

                            scheduledTask.Timer = new Timer(_ => StartLightOffTimer(scheduledTask), null, _settings.LightOnTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                             await _deviceService.Relay3On();
                        }
                        else
                        {
                            Console.WriteLine("Light Stopped");
                            scheduledTask?.Timer?.Dispose();
                        };


                        return true;
                    }
                }

            }
            return true;
        }




        public async void AddLightOffTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding light off Task");
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT LightShouldStop, LightOnTime, LightOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
                var scheduledTask = new ScheduledTask
                {
                    Name = name,
                    Task = task,
                    DelayMs = _settings.LightOnTime
                };
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        _settings.LightShouldStop = reader.GetInt64(0);
                        _settings.LightOnTime = reader.GetInt64(1);
                        _settings.LightOffTime = reader.GetInt64(2);
                        task?.Invoke(); // Execute the provided task
                        connection.Close();
                        if (_settings.LightShouldStop == 0)
                        {
                            
                            Console.WriteLine($"Executing task: {name}");
                            scheduledTask.Timer = new Timer(_ => StartLightOnTimer(scheduledTask), null, _settings.LightOffTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                            await  _deviceService.Relay3Off();
                        } else
                        {
                            scheduledTask?.Timer?.Dispose();
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No settings found!"); // Or handle missing settings differently
                    }

                }

            }

        }

        public async Task<bool> AddExhaustOnTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding exhaust on Task");
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT ExhaustShouldStop, ExhaustOnTime, ExhaustOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
                var scheduledTask = new ScheduledTask
                {
                    Name = name,
                    Task = task,
                    DelayMs = _settings.ExhaustOffTime
                };
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _settings.ExhaustShouldStop = reader.GetInt64(0);
                        _settings.ExhaustOnTime = reader.GetInt64(1);
                        _settings.ExhaustOffTime = reader.GetInt64(2);
                        task?.Invoke();
                        connection.Close();

                        if (_settings.ExhaustShouldStop == 0)
                        {
                          
                            Console.WriteLine($"Executing task: {name}");

                            scheduledTask.Timer = new Timer(_ => StartExhaustOffTimer(scheduledTask), null, _settings.ExhaustOnTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                             await  _deviceService.Relay4On();
                        }
                        else
                        {
                            Console.WriteLine("Exhaust Stopped");
                            scheduledTask?.Timer?.Dispose();
                        };


                        return true;
                    }
                }

            }
            return true;
        }


        public async void AddExhaustOffTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding exhaust off Task");
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT ExhaustShouldStop, ExhaustOnTime, ExhaustOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
                var scheduledTask = new ScheduledTask
                {
                    Name = name,
                    Task = task,
                    DelayMs = _settings.ExhaustOnTime
                };
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        _settings.ExhaustShouldStop = reader.GetInt64(0);
                        _settings.ExhaustOnTime = reader.GetInt64(1);
                        _settings.ExhaustOffTime = reader.GetInt64(2);
                        task?.Invoke(); // Execute the provided task
                        connection.Close();
                        if (_settings.ExhaustShouldStop == 0)
                        {
                           
                            Console.WriteLine($"Executing task: {name}");
                            scheduledTask.Timer = new Timer(_ => StartExhaustOnTimer(scheduledTask), null, _settings.ExhaustOffTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                            await  _deviceService.Relay4Off();
                        }
                        else
                        {
                            scheduledTask?.Timer?.Dispose();
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No settings found!"); // Or handle missing settings differently
                    }

                }

            }

        }

        public async Task<bool> AddFanOnTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding fan on Task");
            var _settings = new Settings();
            using (var connection = SettingsService.Connection)
            {
                connection.Open();
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                SELECT FanShouldStop, FanOnTime, FanOffTime
                FROM settings
                WHERE id = $id
    ";
                    command.Parameters.AddWithValue("$id", 1);
                    var scheduledTask = new ScheduledTask
                    {
                        Name = name,
                        Task = task,
                        DelayMs = _settings.FanOffTime
                    };
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _settings.FanShouldStop = reader.GetInt64(0);
                            _settings.FanOnTime = reader.GetInt64(1);
                            _settings.FanOffTime = reader.GetInt64(2);
                            task?.Invoke();
                            connection.Close();

                            if (_settings.FanShouldStop == 0)
                            {
                              
                                Console.WriteLine($"Executing task: {name}");

                                scheduledTask.Timer = new Timer(_ => StartFanOffTimer(scheduledTask), null, _settings.FanOnTime, Timeout.Infinite);
                                _tasks.Add(scheduledTask);
                             await  _deviceService.Relay2On();
                            }
                            else
                            {
                                Console.WriteLine("Fan Stopped");
                                scheduledTask?.Timer?.Dispose();
                            };


                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return true;
        }


        public async void AddFanOffTask(string name, Action task, long delayMs)
        {
            Console.WriteLine("Adding fan off Task");
            var _settings = new Settings();
            var scheduledTask = new ScheduledTask
            {
                Name = name,
                Task = task,
                DelayMs = _settings.FanOnTime
            };
            using (var connection = SettingsService.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT FanShouldStop, FanOnTime, FanOffTime
                FROM settings
                WHERE id = $id
    ";
                command.Parameters.AddWithValue("$id", 1);
               
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        _settings.FanShouldStop = reader.GetInt64(0);
                        _settings.FanOnTime = reader.GetInt64(1);
                        _settings.FanOffTime = reader.GetInt64(2);
                        task?.Invoke(); // Execute the provided task
                        connection.Close();
                        if (_settings.FanShouldStop == 0)
                        {
                           
                            Console.WriteLine($"Executing task: {name}");
                            scheduledTask.Timer = new Timer(_ => StartFanOnTimer(scheduledTask), null, _settings.FanOffTime, Timeout.Infinite);
                            _tasks.Add(scheduledTask);
                           await  _deviceService.Relay2Off();
                        } else
                        {
                            scheduledTask?.Timer?.Dispose();
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No settings found!"); // Or handle missing settings differently
                    }

                }

            }


        }


        public async void StartPumpOffTimer(ScheduledTask task)
    {
            try
            {
                var manager = new TimerManager(_hubContext, _deviceService);
                manager.AddPumpOffTask("Pump Off Task", () => Console.WriteLine("Turning Pump Off..."), task.DelayMs);
                task.Timer.Dispose();
                task.Task.Invoke(); // Execute the task action
                task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
           
        
        
    
    }
        public async void StartLightOffTimer(ScheduledTask task)
        {
            var deviceId = "661aafd5de6034907f9019cd";
            try
            {
                var manager = new TimerManager(_hubContext, _deviceService);
                Console.WriteLine($"Executing task: {task.Name}");
                manager.AddLightOffTask("Light Off Task", () => Console.WriteLine("Turning Light Off..."), task.DelayMs);
                task.Timer.Dispose();
                task.Task.Invoke(); // Execute the task action
                task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }

        public void StartLightOnTimer(ScheduledTask task)
        {
            var manager = new TimerManager(_hubContext, _deviceService);
            Console.WriteLine($"Executing task: {task.Name}");
            manager.AddLightOnTask("Light On Task", () => Console.WriteLine("Running Light..."), task.DelayMs);
            task.Timer.Dispose();
            task.Task.Invoke(); // Execute the task action
            task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
        }

        public async void StartExhaustOffTimer(ScheduledTask task)
        {
            var deviceId = "661aafd5de6034907f9019cd";
            try
            {
                var manager = new TimerManager(_hubContext, _deviceService);
                Console.WriteLine($"Executing task: {task.Name}");
                manager.AddExhaustOffTask("Exhaust Off Task", () => Console.WriteLine("Turning Exhaust Off..."), task.DelayMs);
                task.Timer.Dispose();
                task.Task.Invoke(); // Execute the task action
                task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }

        private void StartExhaustOnTimer(ScheduledTask task)
        {
            var manager = new TimerManager(_hubContext, _deviceService);
            Console.WriteLine($"Executing task: {task.Name}");
            manager.AddExhaustOnTask("Exhaust On Task", () => Console.WriteLine("Running Exhaust..."), task.DelayMs);
            task.Timer.Dispose();
            task.Task.Invoke(); // Execute the task action
            task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
        }

        private async void StartFanOffTimer(ScheduledTask task)
        {
            var deviceId = "661aafd5de6034907f9019cd";
            try
            {
                var manager = new TimerManager(_hubContext, _deviceService);
                Console.WriteLine($"Executing task: {task.Name}");
                manager.AddFanOffTask("Fan Off Task", () => Console.WriteLine("Turning Fan Off..."), task.DelayMs);
                task.Timer.Dispose();
                task.Task.Invoke(); // Execute the task action
                task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }

        private void StartFanOnTimer(ScheduledTask task)
        {
            var manager = new TimerManager(_hubContext, _deviceService);
            Console.WriteLine($"Executing task: {task.Name}");
            manager.AddFanOnTask("Fan On Task", () => Console.WriteLine("Running Fan..."), task.DelayMs);
            task.Timer.Dispose();
            task.Task.Invoke(); // Execute the task action
            task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
        }


        private void StartPumpOnTimer(ScheduledTask task)
    {
        var manager = new TimerManager(_hubContext, _deviceService);
        manager.AddPumpOnTask("Pump On Task", () => Console.WriteLine("Running pump..."), task.DelayMs);
         task.Timer.Dispose();
        task.Task.Invoke(); // Execute the task action
        task.Timer.Change(task.DelayMs, Timeout.Infinite); // Reset timer for next execution
    }



    public void StartAll()
    {
        foreach (var task in _tasks)
        {
            Console.WriteLine($"{task.Name}");
        }
    }

        public void StopAll()
        {
            var manager = new TimerManager(_hubContext, _deviceService);
            Console.WriteLine("Stopping all timers...");
            foreach(var task in _tasks)
            {
                Console.WriteLine(task.DelayMs);
            }
        }
    }
}