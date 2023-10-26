using Kibana.Alerts;
using Kibana.Alerts.Connectors;
using Kibana.Alerts.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("internalgroups.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile("/groups.json", optional: true, reloadOnChange: false);
builder.Services.AddHostedService<Worker>();
builder.Services.AddElasticClient(builder.Configuration);
builder.Services.AddKeyedTransient<IConnector, SmtpConnector>("smtp");
builder.Services.AddKeyedTransient<IConnector, WebHookConnector>("webhook");
builder.Services.AddSingleton<ConnectorFactory>();
var host = builder.Build();
host.Run();
