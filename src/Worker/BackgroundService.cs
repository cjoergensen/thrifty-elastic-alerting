namespace ThriftyElasticAlerting.Worker;

public class BackgroundService(ILogger<BackgroundService> logger, IAlertingStrategy alertingStrategy) : Microsoft.Extensions.Hosting.BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Service operational, alerts are monitored.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await alertingStrategy.HandleAlerts(stoppingToken);
            await Task.Delay(10000, stoppingToken);
        }
    }
}
