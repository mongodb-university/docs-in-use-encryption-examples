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

  // GCP Credentials
  GCP_EMAIL: "<your GCP email>",
  GCP_PRIVATE_KEY: "<your GCP private key>",
  GCP_PROJECT_ID: "<your project id>",
  GCP_LOCATION: "<your location>",
  GCP_KEY_RING: "<your key ring>",
  GCP_KEY_NAME: "<your key name>",
  GCP_KEY_VERSION: "<your key version>",
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
