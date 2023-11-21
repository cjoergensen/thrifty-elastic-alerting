using Microsoft.Extensions.Configuration;

namespace ThriftyElasticAlerting.Worker.Tests;

/// <summary>
/// Factory for creating test configurations.
/// </summary>
internal static class ConfigurationTestFactory
{
    

    /// <summary>
    /// Creates a empty test configuration.
    /// </summary>
    public static IConfigurationRoot Empty => null;

    /// <summary>
    /// Creates a full test configuration with predefined settings.
    /// </summary>
    public static IConfigurationRoot Full => BuildConfiguration(new Dictionary<string, string?>()   
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    /// <summary>
    /// Creates a test configuration with no 'Groups' section.
    /// </summary>
    public static IConfigurationRoot WithNoGroups => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
    });

    /// <summary>
    /// Creates a test configuration with no 'Elastic' section.
    /// </summary>
    public static IConfigurationRoot WithNoElasticSettings => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    public static IConfiguration WithNoElasticUrl => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    internal static IConfiguration WithNoElasticUsername => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    internal static IConfiguration WithNoElasticPassword => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    internal static IConfiguration WithNoElasticKibanaUrl => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });



    internal static IConfiguration WithSmtpConnectorButNoSmtpSettings => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
    });

    internal static IConfiguration WithSmtpConnectorButNoSmtpHost => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Port", "25"}
    });

    internal static IConfiguration WithSmtpConnectorButNoSmtpSender => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:DevTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"Groups:OpsTeam:Connectors:Smtp:Recipients:0", "test@example.com"},
        {"Groups:OpsTeam:Connectors:MsTeams:WebHookUrl", "http://teams.webhook.example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
        {"SmtpServer:Port", "25"}
    });

    internal static IConfiguration WithMsTeamsConnectorButNoWebHookUrl => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors:MsTeams", "http://teams.webhook.example.com"},
    });

    public static IConfigurationRoot WithNoConnectors => BuildConfiguration(new Dictionary<string, string?>()
    {
        {"Elastic:Url", "http://elastic.example.com"},
        {"Elastic:UserName", "elasticuser"},
        {"Elastic:Password", "elasticpassword"},
        {"Elastic:KibanaUrl", "http://kibana.example.com"},
        {"Groups:DevTeam:Connectors", ""},
        {"SmtpServer:Sender", "sender@example.com"},
        {"SmtpServer:Host", "smtp.example.com"},
    });


    private static IConfigurationRoot BuildConfiguration(Dictionary<string, string?>? initialData)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(initialData ?? []).Build();
    }

}
