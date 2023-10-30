using HandlebarsDotNet;
using Kibana.Alerts.Model;
using System.Net.Mail;

namespace Kibana.Alerts.Connectors;
public class SmtpSettings
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Recipients {  get; set; }
}
public sealed class SmtpConnector(IConfiguration configuration, IHandlebars handlebars) : IConnector
{
    private readonly IConfiguration configuration = configuration;
    private const string DefaultSubjectTemplate = "Important Update: {{Name}} is now: {{ExecutionStatus.Status}}";
    private const string DefaultBodyTemplate = """
    <html>
      <body>
        <p>
          Important Update: The alert "<strong>{{Name}}</strong>" has change status to <strong>{{ExecutionStatus.Status}}</strong>.
        </p>
        <p>
          For comprehensive details regarding this alert's status modification, please access the information through this <a href="{{RuleUrl}}">link</a>.
        </p>
      </body>
    </html>
    """;
    

    public async Task Send(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default)
    {
        var port = int.Parse(configuration["Smtp:Port"]);
        var host = configuration["Smtp:Host"];
        var sender = configuration["Smtp:Sender"];

        var settings = new SmtpSettings();
        configurationSection.Bind(settings);

        var bodyTemplate = handlebars.Compile(settings.Body ?? DefaultBodyTemplate);
        var body = bodyTemplate(alert);

        var subjectTemplate = handlebars.Compile(settings.Subject ?? DefaultSubjectTemplate);
        var subject = subjectTemplate(alert);

        SmtpClient client = new(host, port);
        MailMessage message = new()
        {
            IsBodyHtml = true,
            Body = body,
            BodyEncoding = System.Text.Encoding.UTF8,
            From = new MailAddress(sender),
            Priority = MailPriority.High,
            Subject = subject
        };

        foreach (var recipient in settings.Recipients)
        {
            message.Bcc.Add(recipient);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
