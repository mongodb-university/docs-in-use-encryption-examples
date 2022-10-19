/*
return credentials object and ensure it has been populated
**/
function getCredentials() {
  checkForPlaceholders();
  return credentials;
}

const credentials = {
  // Mongo Paths + URI
  MONGODB_URI: "<your MongoDB URI here>",
  SHARED_LIB_PATH: "<path to crypt_shared library>",

  // KMIP Credentials
  KMIP_KMS_ENDPOINT:
    "<endpoint for your KMIP KMS. Default is 'localhost:5698'>",
  KMIP_TLS_CA_FILE:
    "<full path to your KMIP certificate authority file. Default is '<path to this repo>/kmip_utils/certs/ca.pem'>",
  KMIP_TLS_CERT_FILE:
    "<full path to your client certificate file. Default is '<path to this repo>/kmip_utils/certs/client.pem'>",
  KMIP_TLS_CERT_P12:
    "<full path to your client certificate p12 file. Default is '<path to this repo>/kmip_utils/certs/pcks_client.p12'>",
};

/*
check if credentials object contains placeholder values
**/
function checkForPlaceholders() {
  const errorBuffer = Array();
  const placeholderPattern = /^<.*>$/;
  for (const [key, value] of Object.entries(credentials)) {
    // check for placeholder text
    if (`${value}`.match(placeholderPattern)) {
      errorMessage = `You must fill out the ${key} field of your credentials object.`;
      errorBuffer.push(errorMessage);
    }
    // check if value is empty
    else if (value == undefined) {
      error_message = `The value for ${key} is empty. Please enter something for this value.`;
    }
  }
  // raise an error if errors in buffer
  if (errorBuffer.length > 0) {
    message = errorBuffer.join("\n");
    throw message;
  }
}

module.exports = { getCredentials };
