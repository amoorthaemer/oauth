# OAuth REPO

This repository contains resources to setup a WSO2 Identity Server (IdP) on (Docker) Kubernetes

Required deployments:
- Cluster issuer for NgInx/Ingress certificates
- MSSQL server (either container or local instance)
- WSO2 IdP

Before deploying WSO2 IdP make certain the required databases have been initialized in MSSQL. There is no automated deployment of the databases (yet)

For setting up the minimal databases (WSO2_SHARED_DB and WSO2_IDENTITY_DB)  and other stuff visit: https://is.docs.wso2.com/en/latest/deploy/change-to-mssql/
