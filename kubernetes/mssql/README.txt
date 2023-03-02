
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Yry4k3x!3648ah24a"

RESTORE DATABASE WSO2_IDENTITY_DB FROM DISK = "/var/opt/mssql/backup/WSO2_IDENTITY_DB.bak" \
    WITH MOVE "WSO2_IDENTITY_DB" TO "/var/opt/mssql/data/WSO2_IDENTITY_DB.mdf", \
    MOVE "WSO2_IDENTITY_DB_log" TO "/var/opt/mssql/data/WSO2_IDENTITY_DB_log.ldf"

RESTORE DATABASE WSO2_SHARED_DB FROM DISK = "/var/opt/mssql/backup/WSO2_SHARED_DB.bak" \
    WITH MOVE "WSO2_SHARED_DB" TO "/var/opt/mssql/data/WSO2_SHARED_DB.mdf", \
    MOVE "WSO2_SHARED_DB_log" TO "/var/opt/mssql/data/WSO2_SHARED_DB_log.ldf"

