using HandlebarsDotNet;
using Kibana.Alerts;
using Kibana.Alerts.Connectors;
using Kibana.Alerts.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("internalgroups.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile("/groups.json", optional: true, reloadOnChange: false);
builder.Services.AddHostedService<Worker>();


builder.Services.AddSingleton(factory =>
{
    var handlebars = Handlebars.Create();
    handlebars.Configuration.TextEncoder = null;

    return handlebars;
});

builder.Services.AddElasticClient(builder.Configuration);
builder.Services.AddKeyedTransient<IConnector, SmtpConnector>("smtp");

builder.Services.AddHttpClient<IConnector, MsTeamsConnector>();
builder.Services.AddKeyedTransient<IConnector, MsTeamsConnector>("msteams");

builder.Services.AddSingleton<ConnectorFactory>();
var host = builder.Build();
host.Run();
