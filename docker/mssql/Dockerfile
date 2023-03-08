FROM mcr.microsoft.com/mssql/server:2022-latest

ENV LANG='en_US.UTF-8' LANGUAGE='en_US:en' LC_ALL='en_US.UTF-8'
ENV TZ=Europe/Amsterdam

ENV ACCEPT_EULA=Y
ENV CONFIG_EDGE_BUILD=0

ENV MSSQL_PID=Standard

USER root
RUN apt-get update \
    && apt-get -y upgrade \
    && apt-get -y install netcat
