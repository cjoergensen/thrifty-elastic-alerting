using Kibana.Alerts.Connectors;
using Kibana.Alerts.Model;
using Kibana.Alerts.Repositories;

namespace Kibana.Alerts;

public class Worker(ILogger<Worker> logger, IAlertRepository alertRepository, ConnectorFactory connectorFactory, IConfiguration configuration) : BackgroundService
{
    private Dictionary<string, Alert> currentAlerts = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Service operational, alerts are monitored.");

        currentAlerts = (await alertRepository.GetAll()).ToDictionary(alert => alert.Id, alert => alert);
        while (!stoppingToken.IsCancellationRequested)
        {
            var alerts = await alertRepository.GetAll();
            foreach (var alert in alerts)
            {
                var currentAlert = currentAlerts.GetValueOrDefault(alert.Id);
                if (currentAlert == null)
                {
                    logger.LogInformation("Alert '{Alert} ({AlertId})' added with status '{Status}'", alert.Name, alert.Id, alert.ExecutionStatus.Status);
                    currentAlerts.Add(alert.Id, alert);
                    continue;
                }

                if (currentAlert.Enabled != alert.Enabled)
                {
                    logger.LogInformation("Alert '{Alert} ({AlertId})' enabled state changed from {CurrentState} to {NewState}", alert.Name, alert.Id, currentAlert.Enabled, alert.Enabled);
                    currentAlerts[alert.Id] = alert;
                }

                if (currentAlert.ExecutionStatus.Status != alert.ExecutionStatus.Status)
                {
                    logger.LogInformation("Alert '{Alert} ({AlertId})' changed status from '{CurrentStatus}' to '{NewStatus}'", alert.Name, alert.Id, currentAlert.ExecutionStatus.Status, alert.ExecutionStatus.Status);

                    if (alert.Enabled && alert.ExecutionStatus.Status != "pending")
                    {
                        await SendAlerts(alert, stoppingToken);
                    }

                    currentAlerts[alert.Id] = alert;
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
            if (connectors.Count == 0)
            {
                logger.LogWarning("Tag {Tag} on alert {AlertName} not mapped to any groups in groups.json", tag, alert.Name);
                continue;
            }

            foreach (var connector in connectors)
            {
                try
                {
                    logger.LogInformation("Sending alert: '{Alert} ({AlertId})' to group '{Group}' using '{Connector}'.", alert.Name, alert.Id, tag, connector);

                    var connectorImpl = connectorFactory.Create(connector);
                    await connectorImpl.Send(alert, configuration.GetSection($"Groups:{tag}:Connectors:{connector}"), cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError("Error when sending alert '{Alert} ({AlertId})' to group '{Group}' using '{Connector}'. Exception was: {ExceptionMsg}", alert.Name, alert.Id, tag, connector, e.Message);
                }
            }
        }
    }
}
