using Quartz;
using Quartz.Spi;

public class CustomJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CustomJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
        if (job is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
