using HandlebarsDotNet;
using Kibana.Alerts.Model;
using System.Net.Mail;

namespace Kibana.Alerts.Connectors;
public class SmtpSettings
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Sender { get; set; }
    public List<string> Audience {  get; set; }
}
public sealed class SmtpConnector(IConfiguration configuration) : IConnector
{
    private readonly IConfiguration configuration = configuration;

    public async Task<bool> TrySend(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default)
    {
        var port = int.Parse(configuration["Smtp:Port"]);
        var host = configuration["Smtp:Host"];
        var sender = int.Parse(configuration["Smtp:Sender"]);

        var settings = new SmtpSettings();
        configuration.Bind(settings);

        var bodyTemplate = Handlebars.Compile(settings.Body);
        var body = bodyTemplate(alert);
        SmtpClient client = new(host, port);
        MailMessage message = new()
        {
            IsBodyHtml = true,
            Body = body,
            Priority = MailPriority.High,
            Subject = alert.Name
        };

        foreach (var recipient in settings.Audience)
        {
            message.Bcc.Add(recipient);
        }

        await client.SendMailAsync(message, cancellationToken);
        return true;
    }
}
