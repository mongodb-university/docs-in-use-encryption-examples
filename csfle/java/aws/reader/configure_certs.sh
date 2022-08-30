#!/bin/sh

KMIP_PATH="$GENERATION_DIR/kmip_utils"

# configure certificates for java
mkdir .generated_certs
openssl pkcs12 -CAfile "$KMIP_PATH"/certs/ca.pem -export -in "$KMIP_PATH"/certs/client.pem -out .generated_certs/client.pkc -password pass:${KEYSTORE_PASSWORD}
cp ${JAVA_HOME}/lib/security/cacerts .generated_certs/mongo-truststore
${JAVA_HOME}/bin/keytool -importcert -trustcacerts -file "$KMIP_PATH"/certs/ca.pem -keystore .generated_certs/mongo-truststore -storepass ${KEYSTORE_PASSWORD} -storetype JKS -noprompt

# specify password in maven config file
mkdir .mvn
cp maven.config.tmpl .mvn/maven.config
sed -i '' -e "s/REPLACE-WITH-KEYSTORE-PASSWORD/$KEYSTORE_PASSWORD/g" .mvn/maven.config
sed -i '' -e "s/REPLACE-WITH-TRUSTSTORE-PASSWORD/$TRUSTSTORE_PASSWORD/g" .mvn/maven.config
