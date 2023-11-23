# thrifty-elastic-alerting (TEA)
Are you financially challenged, skint, or just to cheep to purchase an Elastic license? Still, you're desperately yearning for those snazzy Kibana alerting features? Well, look no further because this repository has got your penny-pinching back!

# About this project
TEA is an open-source project designed for efficient alerting in Elasticsearch environments. It operates as a background process or daemon, continuously monitoring the status of alerting rules. When a change is detected, TEA promptly notifies the relevant recipients, providing a streamlined and responsive alerting system.

TEA has been developed to harness the power of Elastic's alerting rules. For a thorough explanation of this feature and guidance on creating rules, please consult the in-depth documentation available here:

https://www.elastic.co/guide/en/kibana/current/alerting-getting-started.html

# Getting started

Given you're already up and running with an Elastic stack the next step is to establish a user dedicated to TEA for monitoring alert changes within an Elasticsearch index. This user must have the `superuser` role to meet TEA's requirements

Detailed instructions on creating a user can be found in the official documentation here: https://www.elastic.co/guide/en/kibana/current/using-kibana-with-security.html

## Core concepts

### Groups and connectors

In TEA, the concept of `groups` determines the recipient(s) for each alert, while `connectors` specify the methods for delivering alerts to these audiences, such as SMTP.

Alerting rules are associated with groups through tags. When an alerting rule is tagged with a matching group name, the recipients within that group will receive notifications through the specified connector.

For a comprehensive list of supported connectors, please visit: [link to connectors goes here].

### Templates

TEA allows you to personalize the presentation of alerts for each group using the handlebar syntax for templating.

For detailed information and examples, please refer to the template wiki.

## Running TEA

TEA is designed to operate as a background process or daemon. It consistently monitors the status of alerting rules, and upon detecting a change, notifies the relevant groups of the updated state.

### Configuration

To enable TEA to connect to the Elasticsearch endpoint, the following environment variables must be set:

```bash
Elastic__Url="<elastichost>"          # URL to the Elasticsearch backend, e.g., https://elasticsearch-es-internal-http:9200
Elastic__KibanaUrl="<kibanaurl>"      # URL to the Kibana frontend
Elastic__UserName="<username>"        # User with the 'superuser' role
Elastic__Password="<password>"        # User password
```

In addition to the environment variables, TEA requires two configuration files:

### `connectors.json`

This file contains the base configuration for connectors. Each connector has its base configuration. Here's an example for the SMTP Connector:

```json
{
  "Smtp": {
    "Sender": "noreply@thrifty-stuff.com",
    "Host": "smtp.somedomain.com",
    "Port": 25
  }
}
```

You can find the base configurations for each connector in the [connectors documentation](<link-to-connectors-documentation>).

### `groups.json`

This file links alerts to connectors. In this example, alerts with the `DevTeam` tag are notified using both the SMTP and MsTeams connectors:

```json
{
  "Groups": {
    "DevTeam": {
      "Connectors": {
        "Smtp": {
          "Recipients": [
            "devteam@nowhere-to-run.com",
            "tech-support@nowhere-to-run.com"
          ]
        },
        "MsTeams": {
          "WebHookUrl": "https://something.webhook.office.com/webhook2/"
        }
      }
    }
  }
}
```

For detailed descriptions and configuration samples, please refer to the [connectors documentation](<link-to-connectors-documentation>).

