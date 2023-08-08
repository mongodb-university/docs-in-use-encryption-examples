import re

_credentials = {
    # Mongo Paths + URI
    "MONGODB_URI": "<your MongoDB URI here>",
    "SHARED_LIB_PATH": "<Absolute path of your Automatic Encryption Shared Library>",
    # GCP Credentials
    "GCP_EMAIL": "<your GCP email>",
    "GCP_PRIVATE_KEY": "<your GCP private key>",
    "GCP_PROJECT_ID": "<your project id>",
    "GCP_LOCATION": "<your location>",
    "GCP_KEY_RING": "<your key ring>",
    "GCP_KEY_NAME": "<your key name>",
    "GCP_KEY_VERSION": "<your key version>",
}


def check_for_placeholders():
    """check if credentials object contains placeholder values"""
    error_buffer = []
    placeholder_pattern = re.compile("^<.*>$")
    for key, value in _credentials.items():
        # check for placeholder text
        if placeholder_pattern.match(str(value)):
            error_message = (
                f"You must fill out the {key} field of your credentials object."
            )
            error_buffer.append(error_message)
        # check if value is empty
        elif not value:
            error_message = (
                f"The value for {key} is empty. Please enter something for this value."
            )
    # raise an error if errors in buffer
    if error_buffer:
        message = "\n".join(error_buffer)
        raise ValueError(message)


def get_credentials():
    """return credentials object and ensure it has been populated"""
    check_for_placeholders()
    return _credentials
