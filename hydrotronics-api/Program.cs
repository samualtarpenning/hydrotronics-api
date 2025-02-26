

using GardenApi.Models;
using GardenApi.services;
using hydrotronics_api;
using hydrotronics_api.Services;
using Quartz;

using static hydrotronics_api.Services.SensorService;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
/*builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000); // Listen on any IP address
});

builder.WebHost.UseUrls("http://*:5000");*/
// add dup back if not r
// Add services to the container.
builder.Services.Configure<GardenDatabaseSettings>(
    builder.Configuration.GetSection("garden-db"));
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<SensorService>();
builder.Services.AddSingleton<SettingsService>();


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory(); 

    q.ScheduleJob<LogTemperatureJob>(trigger => trigger
       .WithIdentity("LogTemperatureTrigger")
       .StartNow()
       .WithCronSchedule("0 0 * * * ?")  
   );
    q.ScheduleJob<PollTemperatureJob>(trigger => trigger
        .WithIdentity("PollTemperatureTrigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(45).RepeatForever())
    );
});

// Add the Quartz hosted service (which runs the jobs)
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Register Quartz scheduler

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
var app = builder.Build();
app.UseCors(x => x
      .AllowAnyMethod()
      .AllowAnyHeader()
      .SetIsOriginAllowed(origin => true)
      .AllowCredentials());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
SQLitePCL.Batteries.Init();
app.UseHttpsRedirection();
app.MapHub<DeviceHub>("/deviceHub");
app.MapControllers();


app.Run();

