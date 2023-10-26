# thrifty-elastic-alerting
Are you financially challenged, skint, or just hilariously frugal to purchase an Elastic license? 
Still, you're desperately yearning for those snazzy Kibana alerting features? 
Well, look no further because this repository has got your penny-pinching back!

## Quick start guide

Thrifty Elastic Alerting need the follow environment variables in order to connect to the Elasticsearch endpoint

```
Elastic__Url="https://<elastichost>:8200"
Elastic__UserName="<username>"
Elastic__Password="<password>"
```

If you're deploying TEA straight into your running ECK stack on Kubernetes, you can use the following deployment YAML to make everything work

``` yaml
---
apiVersion: v1
kind: Secret
metadata:
  name: elasticsearch-es-alerting-user
  namespace: elasticsearch # Change this to match your deployed ECK stack namespace
type: kubernetes.io/basic-auth
stringData:
  username: alerting
  password: A13rt1n9
  roles: kibana_system
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: groups-json-configmap
  namespace: elasticsearch # Change this to match your deployed ECK stack namespace
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
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: thrifty-elastic-alerting-deployment
  namespace: elasticsearch # Change this to match your deployed ECK stack namespace
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
        image: cjoergensen/thrifty-elastic-alerting:latest
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

## Docker setup


## To do's
What is still needed
	* Quick start
		* Create alerting user in Kibana with correct rights
		* Kubernetes deployment
	* Documentation of docker settings
	* Build script
	* Deploy script to Docker Hub

