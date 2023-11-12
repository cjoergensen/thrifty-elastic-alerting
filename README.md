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

## Examples

### Docker Compose

### ECK and k8s

<a id="ECK"></a>
If you're deploying TEA straight into your running ECK stack on Kubernetes, you can use the following deployment YAML to make everything work

``` yaml
---
apiVersion: v1
kind: Secret
metadata:
  name: elasticsearch-es-alerting-user
  namespace: elastic # Change this to match your deployed ECK stack namespace
type: kubernetes.io/basic-auth
stringData:
  username: alerting
  password: A13rt1n9 # Change this please!
  roles: kibana_system
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: groups-json-configmap
  namespace: elastic # Change this to match your deployed ECK stack namespace
data:
  groups.json: |
    {
      "Groups": {
        "DevTeam": {
          "Connectors": {
            "Smtp" : {
              "Subject": "Some delicious subject {alert.name}",
              "Body": "Some body for the email being sent out with substitutions like this one {alert.ExecutionStatus.Status}",
              "Audience": [
                "mail@nowhere-to-run.com"
              ]
            }
          }
        }
      }
    }
  connectors.json: |
    {
      "Smtp": {
        "Sender": "noreply@thrifty-stuff.com",
        "Host": "smtp.somedomain.com",
        "Port": 25
      }
    }
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: thrifty-elastic-alerting-deployment
  namespace: elastic # Change this to match your deployed ECK stack namespace
spec:
  selector:
    matchLabels:
      app:  thrifty-elastic-alerting
  template:
    metadata:
      labels:
        app:  thrifty-elastic-alerting
    spec:
      volumes:
      - name: elastic-cert
        secret:
          secretName: 'elasticsearch-es-http-certs-public'
          optional: true
      containers:
      - name:  thrifty-elastic-alerting
        imagePullPolicy: Always
        image: cjoergensendk/thrifty-elastic-alerting:latest
        volumeMounts:
        - name: elastic-cert
          mountPath: /etc/ssl/certs/elastic.crt
          subPath: tls.crt
          readOnly: false
        resources:
          requests:
            memory: 250Mi
            cpu: 250m
          limits:
            memory: 500Mi
            cpu: 500m 
        env:
        - name: Elastic__Url
          value: 'https://elasticsearch-es-internal-http:9200'
        - name: Elastic__PublicUrl
          value: 'https://mdm-dev.seas.local:30443/kibana/'
        - name: Elastic__UserName
          valueFrom:
            secretKeyRef:
              name: elasticsearch-es-alerting-user
              key: username
              optional: false
        - name: Elastic__Password
          valueFrom:
            secretKeyRef:
              name: elasticsearch-es-alerting-user
              key: password
              optional: false
```


## To do's
What is still needed
 * How to create the 'alerting' user in Kibana
 * Documentation of docker settings
 * How to tag an alert in kibana and add the tag to a group in groups.json
 * Build script
 * Deploy script to Docker Hub
 * Kubectl command pointing to latest release depoyment.yaml


