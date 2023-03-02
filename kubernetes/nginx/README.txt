Step 1.

exec: kubectl edit svc -n ingress-nginx ingress-nginx-controller

under ports add:

- name: redis
  port: 6379
  protocol: TCP
  targetPort: 6379
- name: redis-insight
  port: 8001
  protocol: TCP
  targetPort: 8001
- name: mssql
  port: 1433
  protocol: TCP
  targetPort: 1433
- appProtocol: https
  name: wso2is
  port: 9443 
  targetPort: 9443 


Step 2.

exec: kubectl edit deployment -n ingress-nginx ingress-nginx-controller

under spec/containers/args add:

  - --tcp-services-configmap=$(POD_NAMESPACE)/tcp-services
  - --udp-services-configmap=$(POD_NAMESPACE)/udp-services
