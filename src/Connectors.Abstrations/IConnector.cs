using Microsoft.Extensions.Configuration;
using ThriftyElasticAlerting.Model;

namespace ThriftyElasticAlerting.Abstractions.Connectors;

public interface IConnector
{
    Task Send(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default);
}
