
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Yry4k3x!3648ah24a"

RESTORE DATABASE WSO2_IDENTITY_DB FROM DISK = "/var/opt/mssql/backup/WSO2_IDENTITY_DB.bak" \
    WITH MOVE "WSO2_IDENTITY_DB" TO "/var/opt/mssql/data/WSO2_IDENTITY_DB.mdf", \
    MOVE "WSO2_IDENTITY_DB_log" TO "/var/opt/mssql/data/WSO2_IDENTITY_DB_log.ldf"

RESTORE DATABASE WSO2_SHARED_DB FROM DISK = "/var/opt/mssql/backup/WSO2_SHARED_DB.bak" \
    WITH MOVE "WSO2_SHARED_DB" TO "/var/opt/mssql/data/WSO2_SHARED_DB.mdf", \
    MOVE "WSO2_SHARED_DB_log" TO "/var/opt/mssql/data/WSO2_SHARED_DB_log.ldf"

====================
mssql.conf

[network]
tlscert = /etc/ssl/certs/mssql.pem
tlskey = /etc/ssl/private/mssql.key
tlsprotocols = 1.2
forceencryption = 1


- mountPath: /etc/ssl/certs/cenc-encryption-certificate
  name: cenc-encryption-certificate
  readOnly: true


- name: cenc-encryption-certificate
secret:
    optional: false
    secretName: cenc-encryption-certificate
    items:
    - key: tls.crt
      path: tls.crt
    - key: tls.key
      path: tls.key

=========================

- name: wp-julienhedoux-fr-hitch
        image: gcr.io/poc-docker-87/hitch:1.4.6-15
        imagePullPolicy: Always
        volumeMounts:
          - mountPath: /ssl
            name: wp-julienhedoux-fr-pem
          - mountPath: /secrets
            name: wp-julienhedoux-fr-tls
        ports:
        - containerPort: 443
The volume part :


volumes:
      - name: wp-julienhedoux-fr-pem
        emptyDir: {}
      - name: wp-julienhedoux-fr-tls
        secret:
          secretName: wp-julienhedoux-fr-tls
          items:
          - key: tls.key
            path: tls.key
          - key: tls.crt
            path: tls.crt