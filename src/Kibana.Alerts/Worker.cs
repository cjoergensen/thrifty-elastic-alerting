using Kibana.Alerts.Connectors;
using Kibana.Alerts.Model;
using Kibana.Alerts.Repositories;

namespace Kibana.Alerts;

public class Worker(ILogger<Worker> logger, IAlertRepository alertRepository, ConnectorFactory connectorFactory, IConfiguration configuration) : BackgroundService
{
    private List<Alert> currentAlerts;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        currentAlerts = (await alertRepository.GetAll()).ToList();
        while (!stoppingToken.IsCancellationRequested)
        {
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
                    await SendAlerts(alert, stoppingToken);
                    currentAlerts.Remove(currentAlert);
                    currentAlerts.Add(alert);
                }
            }
            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task SendAlerts(Alert alert, CancellationToken cancellationToken = default)
    {
        foreach (var tag in alert.Tags)
        {
            var connectors = configuration.GetSection($"Groups:{tag}:Connectors").GetChildren().ToDictionary(x => x.Key, null).Keys.ToList();
            if (!connectors.Any())
            {
                logger.LogWarning("Tag {tag} on alert {alertName} not mapped to any groups in groups.json", tag, alert.Name);
                continue;
            }
            foreach (var connector in connectors)
            {
                try
                {
                    var connectorImpl = connectorFactory.Create(connector);
                    await connectorImpl.Send(alert, configuration.GetSection($"Groups:{tag}:Connectors:{connector}"), cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unable to send alert via connector {ConnectorName}. Exception was: {ExceptionMsg}", connector, e.Message);
                    throw;
                }
            }
        }
    }
}
