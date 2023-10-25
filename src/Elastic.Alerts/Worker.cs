using Elastic.Alerts.Model;
using Elastic.Alerts.Repositories;

namespace Elastic.Alerts;

public class Worker(ILogger<Worker> logger, IAlertRepository alertRepository) : BackgroundService
{
    private List<Alert> currentAlerts;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        currentAlerts = (await alertRepository.GetAll()).ToList();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            var alerts = await alertRepository.GetAll();
            foreach (var alert in alerts)
            {
                var currentAlert = currentAlerts.SingleOrDefault(x => x.Id == alert.Id);
                if (currentAlert == null) {
                    logger.LogInformation("Alert added with status {status}",
                        alert.ExecutionStatus.Status);
                    currentAlerts.Add(alert);
                    continue;
                }

                if (currentAlert.ExecutionStatus.Status != alert.ExecutionStatus.Status) 
                {
                    logger.LogInformation("Alert status changed from {currentStatus} to {newStatus}", 
                        currentAlert.ExecutionStatus.Status, alert.ExecutionStatus.Status);
                    currentAlerts.Remove(currentAlert);
                    currentAlerts.Add(alert);
                }
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}
