#!/bin/bash

if [ -n "$MSSQL_TLS_CERT" ] && [ -n "$MSSQL_TLS_KEY" ]; then
    if [ -f $MSSQL_TLS_CERT ] && [ -f $MSSQL_TLS_KEY ]; then
        echo "Configuring TLS..."

        sudo chmod 440 $MSSQL_TLS_CERT
        sudo chmod 440 $MSSQL_TLS_KEY

        sudo /opt/mssql/bin/mssql-conf set network.tlscert $MSSQL_TLS_CERT
        sudo /opt/mssql/bin/mssql-conf set network.tlskey $MSSQL_TLS_KEY

        if [ -n "$MSQL_TLS_PROTOCOLS" ]; then
            sudo /opt/mssql/bin/mssql-conf set network.tlsprotocols $MSSQL_TLS_PROTOCOLS
        fi

        if [ -n "$MSQL_TLS_CIPHERS" ]; then
            sudo /opt/mssql/bin/mssql-conf set network.tlsciphers $MSSQL_TLS_CIPHERS
        fi

        if [ -n "$MSSQL_TLS_FORCE" ] && [ $MSSQL_TLS_FORCE -eq "1" ]; then
            sudo /opt/mssql/bin/mssql-conf set network.forceencryption true
        fi
    fi
fi

/opt/mssql/bin/sqlservr

