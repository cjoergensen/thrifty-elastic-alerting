using ThriftyElasticAlerting.Model;
using ThriftyElasticAlerting.Repositories;

namespace ThriftyElasticAlerting.Worker;

public class BackgroundService(ILogger<BackgroundService> logger, IAlertingStrategy alertingStrategy, IUserRepository userRepository) : Microsoft.Extensions.Hosting.BackgroundService
{
    private Dictionary<string, Alert>? currentAlerts = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        userRepository.UpdateUser();
        logger.LogInformation("Service operational, alerts are monitored.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await alertingStrategy.HandleAlerts(stoppingToken);
            await Task.Delay(10000, stoppingToken);
        }
    }
}
