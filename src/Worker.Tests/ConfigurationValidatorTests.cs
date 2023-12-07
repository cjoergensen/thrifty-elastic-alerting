using Microsoft.Extensions.Configuration;

namespace ThriftyElasticAlerting.Worker.Tests;

public sealed class ConfigurationValidatorTests
{
    public static List<object[]> BasicConfigurationTestData =
    [
        [ConfigurationTestFactory.Empty, "Configuration Error: Configuration is null."],
        [ConfigurationTestFactory.WithNoGroups, "Configuration Error: 'Groups' section is missing or empty."],
        [ConfigurationTestFactory.WithNoConnectors, "Configuration Error: Group 'DevTeam' needs at least one connector of type 'Smtp' or 'MsTeams'."]

    ];
    public static List<object[]> ElasticConfigurationTestData =
    [
        [ConfigurationTestFactory.WithNoElasticSettings, "Configuration Error: Required 'Elastic' section is missing or empty. Section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'."],
        [ConfigurationTestFactory.WithNoElasticUrl, "Configuration Error: 'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'."],
        [ConfigurationTestFactory.WithNoElasticUsername,"Configuration Error: 'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'."],
        [ConfigurationTestFactory.WithNoElasticPassword, "Configuration Error: 'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'."],
        [ConfigurationTestFactory.WithNoElasticKibanaUrl, "Configuration Error: 'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'."],
    ];

    public static List<object[]> SmtpConnectorConfigurationTestData =
    [
        [ConfigurationTestFactory.WithSmtpConnectorButNoSmtpSettings, "Configuration Error: Group 'DevTeam' is using the 'Smtp' connector, but no SmtpServer settings are present (or empty) in the configuration."],
        [ConfigurationTestFactory.WithSmtpConnectorButNoSmtpSender, "Configuration Error: Invalid 'SmtpServer' settings. 'Sender', 'Host', and 'Port' are required."],
        [ConfigurationTestFactory.WithSmtpConnectorButNoSmtpHost, "Configuration Error: Invalid 'SmtpServer' settings. 'Sender', 'Host', and 'Port' are required."],
    ];

    public static List<object[]> MsTeamsConnectorConfigurationTestData =
    [
        [ConfigurationTestFactory.WithMsTeamsConnectorButNoWebHookUrl, "Configuration Error: For Group 'DevTeam', the 'MsTeams' connector requires a WebHookUrl."],
    ];


    [Fact]
    public void ValidConfiguration_ThrowsNoExceptions()
    {
        ConfigurationValidator.ValidateConfiguration(ConfigurationTestFactory.Full);
    }

    [Theory]
    [MemberData(nameof(BasicConfigurationTestData))]
    [MemberData(nameof(ElasticConfigurationTestData))]
    [MemberData(nameof(SmtpConnectorConfigurationTestData))]
    [MemberData(nameof(MsTeamsConnectorConfigurationTestData))]
    public void InvalidConfiguration_ThrowsConfigurationException(IConfigurationRoot configurationRoot, string expectedException)
    {
        var exception = Assert.Throws<ConfigurationException>(() => ConfigurationValidator.ValidateConfiguration(configurationRoot));
        Assert.Equal(expectedException, exception.Message);
    }
}