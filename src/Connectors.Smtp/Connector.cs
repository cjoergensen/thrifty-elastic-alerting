using HandlebarsDotNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using ThriftyElasticAlerting.Abstractions.Connectors;
using ThriftyElasticAlerting.Model;

namespace ThriftyElasticAlerting.Connectors.Smtp;
public sealed class Connector(IConfiguration configuration, IHandlebars handlebars) : IConnector
{
    public const string Key = "Smtp";

    private readonly IConfiguration configuration = configuration;
    private const string DefaultSubjectTemplate = "Important Update: {{Name}} is now {{ExecutionStatus.Status}}";
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
        if(int.TryParse(configuration["SmtpServer:Port"], out int port) == false)
        {
            port = 25;
        }

        var host = configuration["SmtpServer:Host"];
        var sender = configuration["SmtpServer:Sender"];
        _ = bool.TryParse(configuration["SmtpServer:UseSsl"], out bool useSsl);
        var userName = configuration["SmtpServer:UserName"];
        var password = configuration["SmtpServer:Password"];

        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentException.ThrowIfNullOrWhiteSpace(sender);

        var settings = new Settings();
        configurationSection.Bind(settings);

        var bodyTemplate = handlebars.Compile(settings.Body ?? DefaultBodyTemplate);
        var body = bodyTemplate(alert);

        var subjectTemplate = handlebars.Compile(settings.Subject ?? DefaultSubjectTemplate);
        var subject = subjectTemplate(alert);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(sender, sender));
        foreach (var recipient in settings.Recipients)
        {
            message.To.Add(new MailboxAddress(recipient, recipient));
        }

        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var client = new SmtpClient();
        
        var socketOptions = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;
        await client.ConnectAsync(host, port, socketOptions, cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            await client.AuthenticateAsync(userName, password, cancellationToken);
        
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}