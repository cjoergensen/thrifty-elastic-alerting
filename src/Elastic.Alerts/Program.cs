using Elastic.Alerts;
using Elastic.Alerts.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddElasticClient(builder.Configuration);

var host = builder.Build();
host.Run();
