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
        var sender = configuration["Smtp:Sender"];

        var settings = new SmtpSettings();
        configurationSection.Bind(settings);

        var bodyTemplate = Handlebars.Compile(settings.Body);
        var body = bodyTemplate(alert);

        var subjectTemplate = Handlebars.Compile(settings.Subject);
        var subject = subjectTemplate(alert);

        SmtpClient client = new(host, port);
        MailMessage message = new()
        {
            IsBodyHtml = true,
            Body = body,
            From = new MailAddress(sender),
            Priority = MailPriority.High,
            Subject = subject
        };

        foreach (var recipient in settings.Audience)
        {
            message.Bcc.Add(recipient);
        }

        await client.SendMailAsync(message, cancellationToken);
        return true;
    }
}
