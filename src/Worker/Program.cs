using HandlebarsDotNet;
using ThriftyElasticAlerting.Abstractions.Connectors;
using ThriftyElasticAlerting.Connectors.MsTeams;
using ThriftyElasticAlerting.Connectors.Smtp;
using ThriftyElasticAlerting.Worker;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Configuration.AddJsonFile("groups.json", optional: true, reloadOnChange: true);
    builder.Configuration.AddJsonFile("connectors.json", optional: true, reloadOnChange: true);
    ConfigurationValidator.ValidateConfiguration(builder.Configuration);

    builder.Services.AddHostedService<ThriftyElasticAlerting.Worker.BackgroundService>();
    builder.Services.AddSingleton(factory =>
    {
        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        return handlebars;
    });

    builder.Services.AddSmtpConnector();
    builder.Services.AddMsTeamsConnector();
    builder.Services.AddSingleton<ConnectorFactory>();

    var host = builder.Build();
    host.Run();
}
catch (Exception e)
{
    Console.WriteLine($"Fatal Exception! Exception was: {e.Message}");
    Environment.Exit(-1);
}