FROM mcr.microsoft.com/mssql/server:2022-latest

ENV LANG='en_US.UTF-8' LANGUAGE='en_US:en' LC_ALL='en_US.UTF-8'
ENV TZ=Europe/Amsterdam

ENV ACCEPT_EULA=Y
ENV CONFIG_EDGE_BUILD=0

ENV MSSQL_PID=Standard

ENV MSSQL_TLS_CERT=
ENV MSSQL_TLS_KEY=
ENV MSSQL_TLS_PROTOCOLS=1.2
ENV MSSQL_TLS_CIPHERS=
ENV MSSQL_TLS_FORCE=0

USER root
RUN apt-get update && apt-get -y upgrade && apt-get -y install sudo

RUN adduser mssql sudo
RUN echo 'mssql ALL=(ALL) NOPASSWD: ALL' > /etc/sudoers.d/sudo-mssql

USER mssql
WORKDIR /home/mssql

COPY ./docker-entrypoint.sh /home/mssql

ENTRYPOINT "/home/mssql/docker-entrypoint.sh"

CMD exec /bin/bash -c "trap : TERM INT; sleep infinity & wait"

