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

  // AWS Credentials
  AWS_ACCESS_KEY_ID: "<your AWS access key ID here>",
  AWS_SECRET_ACCESS_KEY: "<your AWS secret access key here>",
  AWS_KEY_REGION: "<your AWS key region>",
  AWS_KEY_ARN: "<your AWS key ARN>",
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
