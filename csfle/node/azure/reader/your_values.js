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
  SHARED_LIB_PATH: "<path to CSFLE shared library>",

  // Azure Credentials
  AZURE_TENANT_ID: "<your Azure tenant ID here>",
  AZURE_CLIENT_ID: "<your Azure client ID here>",
  AZURE_CLIENT_SECRET: "<your cleint secret here>",
  AZURE_KEY_NAME: "<your key name here>",
  AZURE_KEY_VERSION: "<your key version here>",
  AZURE_KEY_VAULT_ENDPOINT: "<your key vault endpoint here>",
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
