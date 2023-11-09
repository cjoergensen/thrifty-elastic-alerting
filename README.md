# thrifty-elastic-alerting
Are you financially challenged, skint, or just to cheep to purchase an Elastic license? Still, you're desperately yearning for those snazzy Kibana alerting features? Well, look no further because this repository has got your penny-pinching back!

## Quick start guide

Given you're already up and running with an Elastic stack, either using Elastic ECK or using a home backed docker compose setup (see [here for ECK](#ECK) or [here for Docker](#Docker)), the next step is to create a user that TEA will use to monitor alert changes in an index in Elasticsearch.

### Create an 'alerting' user in Kibana

Add 


Thrifty Elastic Alerting need the follow environment variables in order to connect to the Elasticsearch endpoint

```
Elastic__Url="https://<elastichost>:8200"
Elastic__UserName="<username>"
Elastic__Password="<password>"
```

## Elastic ECK Deployment
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

## Docker comopose deployment
<a id="Docker"></a>
``` yaml
Somethings missing here!
```

## To do's
What is still needed
 * How to create the 'alerting' user in Kibana
 * Documentation of docker settings
 * How to tag an alert in kibana and add the tag to a group in groups.json
 * Build script
 * Deploy script to Docker Hub
 * Kubectl command pointing to latest release depoyment.yaml


