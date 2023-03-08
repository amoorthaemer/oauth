#!/bin/bash

if [ ! -d ${OPA_SERVER_HOME} ];
then
    echo "$OPA_SERVER_HOME is not a directory! Exiting ..."
    exit 1
fi

if [ ! -n ${OPA_STORAGE_DIR} ] || [ ! -d ${OPA_STORAGE_DIR} ];
then
    OPA_STORAGE_DIR=${OPA_SERVER_HOME}/data
    export OPA_STORAGE_DIR
fi

if [ ! -n ${OPA_PERSISTENCE_DIR} ] || [ ! -d ${OPA_PERSISTENCE_DIR} ];
then
    OPA_PERSISTENCE_DIR=${OPA_SERVER_HOME}/persistence
    export OPA_PERSISTENCE_DIR
fi

if [ -f ${OPA_CONFIG_DIR}/config.yml ];
then
    OPA_EXTRA_ARGS="-c $OPA_CONFIG_DIR/config.yml $OPA_EXTRA_ARGS"
    export OPA_EXTRA_ARGS
fi

exec ${OPA_SERVER_HOME}/bin/opa run --server ${OPA_EXTRA_ARGS} $@
