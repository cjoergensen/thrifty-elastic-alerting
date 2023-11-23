namespace ThriftyElasticAlerting.Worker;

public interface IAlertingStrategy
{
    Task HandleAlerts(CancellationToken stoppingToken);
}
