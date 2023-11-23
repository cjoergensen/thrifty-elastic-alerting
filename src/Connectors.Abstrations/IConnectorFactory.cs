namespace ThriftyElasticAlerting.Abstractions.Connectors
{
    public interface IConnectorFactory
    {
        IConnector Create(string connectorKey);
    }
}