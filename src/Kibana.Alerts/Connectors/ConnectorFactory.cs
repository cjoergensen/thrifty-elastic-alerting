using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kibana.Alerts.Connectors;

public interface IConnector
{
    Task<bool> TrySend(IConfigurationSection configurationSection);
}
public class ConnectorFactory(IServiceProvider services)
{
    public IConnector Create(string connectorType) => services.GetRequiredKeyedService<IConnector>(connectorType.ToLower());
}
