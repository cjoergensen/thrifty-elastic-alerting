using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ThriftyElasticAlerting.Abstractions.Connectors;
using ThriftyElasticAlerting.Model;
using ThriftyElasticAlerting.Repositories;

namespace ThriftyElasticAlerting.Worker.Tests;

public class BackgroundServiceTests
{
    [Fact]
    public async Task CheckForUpdates_ShouldHandleNullCurrentAlerts()
    {
        // Arrange
        var logger = Substitute.For<ILogger<NotifyOnStateChangeStrategy>>();
        var alertRepository = Substitute.For<IAlertRepository>();
        var connectorFactory = Substitute.For<IConnectorFactory>();
        var configuration = Substitute.For<IConfiguration>();
        var service = new NotifyOnStateChangeStrategy(logger, alertRepository, connectorFactory, configuration);

        // Act
        await service.HandleAlerts(default);

        // Assert
        await alertRepository.Received(1).GetAll();
        // Other assertions based on your specific logic
    }

    [Fact]
    public async Task CheckForUpdates_ShouldHandleNewAlert()
    {
        // Arrange
        var logger = Substitute.For<ILogger<NotifyOnStateChangeStrategy>>();
        var alertRepository = Substitute.For<IAlertRepository>();
        var connectorFactory = Substitute.For<IConnectorFactory>();
        var configuration = Substitute.For<IConfiguration>();
        var service = new NotifyOnStateChangeStrategy(logger, alertRepository, connectorFactory, configuration);

        var newAlert = new Alert { Id = "1", Name = "Test Alert", ExecutionStatus = new ExecutionStatus { Status = "pending" } };
        alertRepository.GetAll().Returns(new List<Alert> { newAlert });

        // Act
        await service.HandleAlerts(default);

        newAlert = new Alert { Id = "1", Name = "Test Alert 2", ExecutionStatus = new ExecutionStatus { Status = "OK" } };
        alertRepository.GetAll().Returns(new List<Alert> { newAlert });
        await service.HandleAlerts(default);


    }

}
