FROM ubuntu:22.04

ENV LANG='en_US.UTF-8' LANGUAGE='en_US:en' LC_ALL='en_US.UTF-8'
ENV TZ=Europe/Amsterdam

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends tzdata curl ca-certificates locales ntp jq \
    && echo "en_US.UTF-8 UTF-8" >> /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

ARG USER=opa
ARG USER_ID=802
ARG USER_GROUP=opa
ARG USER_GROUP_ID=802
ARG USER_HOME=/home/${USER}

ARG OPA_HOME=/var/opt/${USER}
ARG OPA_ARGS=
ARG OPA_VERSION=0.49.2

ENV OPA_SERVER_HOME=${OPA_HOME}
ENV OPA_SERVER_VERSION=${OPA_VERSION}
ENV OPA_CONFIG_DIR=${OPA_SERVER_HOME}/conf.d
ENV OPA_STORAGE_DIR=${OPA_SERVER_HOME}/data
ENV OPA_PERSISTENCE_DIR=${OPA_SERVER_HOME}/persistence
ENV OPA_EXTRA_ARGS=${OPA_ARGS}

RUN \
    groupadd --system -g ${USER_GROUP_ID} ${USER_GROUP} \
    && useradd --system --create-home --home-dir ${USER_HOME} --no-log-init -g ${USER_GROUP_ID} -u ${USER_ID} ${USER}

RUN \
    mkdir -p ${OPA_SERVER_HOME}/bin ${OPA_CONFIG_DIR} ${OPA_STORAGE_DIR} ${OPA_PERSISTENCE_DIR} \
    && chmod -R 755 ${OPA_SERVER_HOME} ${OPA_CONFIG_DIR} ${OPA_STORAGE_DIR} ${OPA_PERSISTENCE_DIR} \
    && chown -R ${USER}:${USER_GROUP} ${OPA_SERVER_HOME} ${OPA_CONFIG_DIR} ${OPA_STORAGE_DIR} ${OPA_PERSISTENCE_DIR}

ADD --chown=${USER}:${USER_GROUP} https://github.com/open-policy-agent/opa/releases/download/v${OPA_SERVER_VERSION}/opa_linux_amd64_static ${OPA_SERVER_HOME}/bin/opa
RUN chmod +x ${OPA_SERVER_HOME}/bin/opa

COPY --chown=${USER}}:${USER_GROUP}} docker-entrypoint.sh /
COPY --chown=${USER}}:${USER_GROUP}} config.yml ${OPA_HOME}/conf.d/

USER ${USER}

EXPOSE 8181

ENTRYPOINT [ "/docker-entrypoint.sh" ]
