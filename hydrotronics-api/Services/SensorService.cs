using GardenApi.Models;
using hydrotronics_api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Quartz;
using Quartz.Impl;
using System.Text.Json;
using static hydrotronics_api.Models.Co2Concentration;

namespace hydrotronics_api.Services
{
    public class SensorService
    {
        private readonly IHubContext<DeviceHub> _hubContext;
        private double _lastTemperature;
        private double _lastHumidity;
        private double _lastPpm;

        public SensorService(IHubContext<DeviceHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task<List<DailyTemperatureReading>> GetReadings(int pageNumber)
        {
            var result = new List<DailyTemperatureReading>();
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            var nowCST = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, centralTimeZone);
            const int itemsPerPage = 5;
            var startDate = nowCST.AddDays(-(pageNumber * itemsPerPage));
            var endDate = startDate.AddDays(itemsPerPage);

            for (int i = 0; i < itemsPerPage; i++)
            {
                var fromDateTimeCST = startDate.AddDays(i).Date; 
                var toDateTimeCST = fromDateTimeCST.AddDays(1); 

                if (fromDateTimeCST.Date == nowCST.Date)
                {
                    fromDateTimeCST = nowCST.Date; 
                    toDateTimeCST = fromDateTimeCST.AddDays(1);
                }

                float maxTemperature = float.MinValue;
                float minTemperature = float.MaxValue;
                float sumTemperature = 0.0f;
                float maxHumidity = float.MinValue;
                float minHumidity = float.MaxValue;
                float sumHumidity = 0.0f;
                int count = 0;

                using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
                {
                    try
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = @"
                    SELECT DateTime, Temperature, Humidity
                    FROM TemperatureLogs
                    WHERE DateTime >= @FromDateTime AND DateTime < @ToDateTime;";

                        command.Parameters.AddWithValue("@FromDateTime", fromDateTimeCST.ToString("MM/dd/yyyy HH:mm:ss"));
                        command.Parameters.AddWithValue("@ToDateTime", toDateTimeCST.ToString("MM/dd/yyyy HH:mm:ss"));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var reading = new TemperatureReading
                                {
                                    DateTime = reader.GetString(0),
                                    Temperature = reader.GetFloat(1),
                                    Humidity = reader.GetFloat(2)
                                };

                                // Calculate high, low, and sum for temperature and humidity
                                maxTemperature = Math.Max(maxTemperature, reading.Temperature);
                                minTemperature = Math.Min(minTemperature, reading.Temperature);
                                sumTemperature += reading.Temperature;

                                maxHumidity = Math.Max(maxHumidity, reading.Humidity);
                                minHumidity = Math.Min(minHumidity, reading.Humidity);
                                sumHumidity += reading.Humidity;

                                count++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving data for {fromDateTimeCST.ToShortDateString()}: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                // If no readings were found for the current day, skip adding this day's data
                if (count > 0)
                {
                    // Calculate averages if there are readings
                    float avgTemperature = count > 0 ? sumTemperature / count : 0;
                    float avgHumidity = count > 0 ? sumHumidity / count : 0;

                    result.Add(new DailyTemperatureReading
                    {
                        Date = fromDateTimeCST,
                        MaxTemperature = maxTemperature,
                        MinTemperature = minTemperature,
                        AvgTemperature = avgTemperature,
                        MaxHumidity = maxHumidity,
                        MinHumidity = minHumidity,
                        AvgHumidity = avgHumidity
                    });
                }
            }

            return result;
        }
        public async Task<List<DailyCo2ConcentrationeReading>> GetCo2Readings(int pageNumber)
        {
            var result = new List<DailyCo2ConcentrationeReading>();
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            var nowCST = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, centralTimeZone);
            const int itemsPerPage = 5;
            var startDate = nowCST.AddDays(-(pageNumber * itemsPerPage));
            var endDate = startDate.AddDays(itemsPerPage);

            for (int i = 0; i < itemsPerPage; i++)
            {
                var fromDateTimeCST = startDate.AddDays(i).Date;
                var toDateTimeCST = fromDateTimeCST.AddDays(1);

                if (fromDateTimeCST.Date == nowCST.Date)
                {
                    fromDateTimeCST = nowCST.Date;
                    toDateTimeCST = fromDateTimeCST.AddDays(1);
                }

                float maxPpm = float.MinValue;
                float minPpm = float.MaxValue;
                float sumPpm = 0.0f;
                int count = 0;

                using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
                {
                    try
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = @"
                    SELECT DateTime, Ppm
                    FROM Co2ConcentrationLogs
                    WHERE DateTime >= @FromDateTime AND DateTime < @ToDateTime;";

                        command.Parameters.AddWithValue("@FromDateTime", fromDateTimeCST.ToString("MM/dd/yyyy HH:mm:ss"));
                        command.Parameters.AddWithValue("@ToDateTime", toDateTimeCST.ToString("MM/dd/yyyy HH:mm:ss"));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var reading = new Co2ConcentrationReading
                                {
                                    DateTime = reader.GetString(0),
                                    Ppm = reader.GetFloat(1)
                                };

                                // Calculate high, low, and sum for ppm
                                maxPpm = Math.Max(maxPpm, reading.Ppm);
                                minPpm = Math.Min(minPpm, reading.Ppm);
                                sumPpm += reading.Ppm;

                                count++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving data for {fromDateTimeCST.ToShortDateString()}: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                // If no readings were found for the current day, skip adding this day's data
                if (count > 0)
                {
                    // Calculate averages if there are readings
                    float avgPpm = count > 0 ? sumPpm / count : 0;

                    result.Add(new DailyCo2ConcentrationeReading
                    {
                        Date = fromDateTimeCST,
                        MaxPpm = maxPpm,
                        MinPpm = minPpm,
                        AvgPpm = avgPpm
                    });
                }
            }

            return result;
        }

        public async Task<List<TemperatureReading>> GetReadingsForLast24HoursOfDay(DateTime date)
        {
            var readings = new List<TemperatureReading>();
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");

            var startOfDayCST = TimeZoneInfo.ConvertTime(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0), TimeZoneInfo.Local, centralTimeZone); // Start of the day (Midnight)
            var endOfDayCST = startOfDayCST.AddHours(24);
            var currentDateTime = DateTime.Now;

          
            using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
         SELECT DateTime, Temperature, Humidity
         FROM TemperatureLogs
         WHERE DateTime >= @StartOfDay AND DateTime < @EndOfDay;";

                    command.Parameters.AddWithValue("@StartOfDay", startOfDayCST.ToString("MM/dd/yyyy HH:mm:ss"));
                    command.Parameters.AddWithValue("@EndOfDay", endOfDayCST.ToString("MM/dd/yyyy HH:mm:ss"));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reading = new TemperatureReading
                            {
                                DateTime = reader.GetString(0),
                                Temperature = reader.GetFloat(1), // Use GetDouble for REAL in SQLite if required
                                Humidity = reader.GetFloat(2)
                            };

                            readings.Add(reading);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving data: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return readings;
        }
        public async Task<List<Co2ConcentrationReading>> GetCo2ReadingsForLast24HoursOfDay(DateTime date)
        {
            var readings = new List<Co2ConcentrationReading>();
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");

            var startOfDayCST = TimeZoneInfo.ConvertTime(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0), TimeZoneInfo.Local, centralTimeZone); // Start of the day (Midnight)
            var endOfDayCST = startOfDayCST.AddHours(24);
            var currentDateTime = DateTime.Now;


            using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                             SELECT DateTime, Ppm
                             FROM Co2ConcentrationLogs
                             WHERE DateTime >= @StartOfDay AND DateTime < @EndOfDay;";

                    command.Parameters.AddWithValue("@StartOfDay", startOfDayCST.ToString("MM/dd/yyyy HH:mm:ss"));
                    command.Parameters.AddWithValue("@EndOfDay", endOfDayCST.ToString("MM/dd/yyyy HH:mm:ss"));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var reading = new Co2ConcentrationReading
                            {
                                DateTime = reader.GetString(0),
                                Ppm = reader.GetFloat(1),
                            };

                            readings.Add(reading);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving data: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return readings;
        }
        public async Task<int> GetTotalPages()
        {
            const int itemsPerPage = 5; // Number of items per page (adjust if needed)

            int totalRecords = 0;

            using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                SELECT COUNT(*)
                FROM TemperatureLogs;"; // Count all rows in the table

                    totalRecords = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error counting records: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            // Calculate total pages based on the total number of records
            int totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);

            return totalPages;
        }





        public class LogTemperatureJob : IJob
        {
             SensorService _sensorService;

            public LogTemperatureJob(SensorService sensorService)
            {
                _sensorService = sensorService;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                var tempData = await _sensorService.GetTemperatureData();
                var co2Data = await _sensorService.GetCo2Concentration();
                var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
                var centralTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, centralTimeZone);

                var tempReading = new TemperatureReading
                {
                    Temperature = tempData.temperature,
                    Humidity = tempData.humidity,
                    DateTime = centralTime.ToString("MM/dd/yyyy HH:mm:ss")
                };

                var co2Reading = new Co2ConcentrationReading
                {
                    Ppm = co2Data.ppm,
                    DateTime = centralTime.ToString("MM/dd/yyyy HH:mm:ss")
                };
                    
                using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
                {
                    try
                    {
                        connection.Open();

                        // First command for inserting into TemperatureLogs
                        var command1 = connection.CreateCommand();
                        command1.CommandText = @"INSERT INTO TemperatureLogs (
                                                        DateTime, 
                                                        Temperature, 
                                                        Humidity
                                                    )
                                                    VALUES (
                                                        @DateTime, 
                                                        @Temperature, 
                                                        @Humidity
                                                    );";

                        command1.Parameters.AddWithValue("@DateTime", tempReading.DateTime);
                        command1.Parameters.AddWithValue("@Temperature", tempReading.Temperature);
                        command1.Parameters.AddWithValue("@Humidity", tempReading.Humidity);

                        // Second command for inserting into Co2ConcentrationLogs
                        var command2 = connection.CreateCommand();
                        command2.CommandText = @"INSERT INTO Co2ConcentrationLogs (
                                                            DateTime, 
                                                            Ppm
                                                        )
                                                        VALUES (
                                                            @DateTime, 
                                                            @Ppm
                                                        );";

                        command2.Parameters.AddWithValue("@DateTime", co2Reading.DateTime);
                        command2.Parameters.AddWithValue("@Ppm", co2Reading.Ppm);

                        // Execute both commands
                        await command1.ExecuteNonQueryAsync();
                        await command2.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception (log or rethrow as needed)
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
        }

        public class PollTemperatureJob : IJob
        {
            private readonly SensorService _sensorService;

            public PollTemperatureJob(SensorService sensorService)
            {
                _sensorService = sensorService;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                try
                {
                    var tempData = await _sensorService.GetTemperatureData();
                    var co2Concentration = await _sensorService.GetCo2Concentration();
                    bool hasChanged = false;
                    if (_sensorService._lastTemperature != tempData.temperature)
                    {
                        hasChanged = true;
                        _sensorService._lastTemperature = tempData.temperature;
                    }
                    if (_sensorService._lastHumidity != tempData.humidity)
                    {
                        hasChanged = true;
                        _sensorService._lastHumidity = tempData.humidity;
                    }
                    if (Math.Abs(_sensorService._lastPpm - co2Concentration.ppm) > 10)
                    {
                        hasChanged = true;
                        _sensorService._lastPpm = co2Concentration.ppm;
                    }
                    if (hasChanged)
                    {
                        var tempReading = new TemperatureReading
                        {
                            Temperature = tempData.temperature,
                            Humidity = tempData.humidity
                        };

                        var co2ConcentrationReading = new Co2ConcentrationReading
                        {
                            Ppm = co2Concentration.ppm
                        };

                        await _sensorService._hubContext.Clients.All.SendAsync("Temperature/Humidity", tempReading);
                        await _sensorService._hubContext.Clients.All.SendAsync("CO2Concentration", co2ConcentrationReading);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in {nameof(PollTemperatureJob)}: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }


        // Methods for getting and inserting temperature readings
        public async Task<TemperatureSensor> GetTemperatureData()
        {
            var temperatureSensor = new TemperatureSensor();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseProxy = false 
            };

            try
            {
                string url = "http://192.168.12.179:80/getdata";
                using (HttpClient client = new HttpClient(handler))
                {
                    Console.WriteLine("Using URL: " + url);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        temperatureSensor = JsonSerializer.Deserialize<TemperatureSensor>(jsonResponse);

                        Console.WriteLine("Temperature: " + temperatureSensor.temperature);
                        Console.WriteLine("Humidity: " + temperatureSensor.humidity);
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
                await _hubContext.Clients.All.SendAsync("MessageLog", "Data Logging Failed - Temperature Sensor Failure");
            }

            return temperatureSensor;
        }

        public async Task<Co2ConcentrationSensor> GetCo2Concentration()
        {
            var co2ConcetrationSensor = new Co2ConcentrationSensor();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseProxy = false
            };

            try
            {
                string url = "http://192.168.12.179:80/getdata";
                using (HttpClient client = new HttpClient(handler))
                {
                    Console.WriteLine("Using URL: " + url);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        co2ConcetrationSensor = JsonSerializer.Deserialize<Co2ConcentrationSensor>(jsonResponse);

                        Console.WriteLine("ppm: " + co2ConcetrationSensor.ppm);
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
                await _hubContext.Clients.All.SendAsync("MessageLog", "Data Logging Failed - Temperature Sensor Failure");
            }

            return co2ConcetrationSensor;
        }

        public async Task<bool> InsertTemperatureReadingAsync(TemperatureReading reading)
        {
            using (var connection = new SqliteConnection("Data Source=hydrotronics-garden.db"))
            {
                try
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                INSERT INTO TemperatureLogs (
                    DateTime, 
                    Temperature, 
                    Humidity
                )
                VALUES (
                    @DateTime, 
                    @Temperature, 
                    @Humidity
                );";

                    command.Parameters.AddWithValue("@DateTime", reading.DateTime);
                    command.Parameters.AddWithValue("@Temperature", reading.Temperature);
                    command.Parameters.AddWithValue("@Humidity", reading.Humidity);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting reading: {ex.Message}");
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
