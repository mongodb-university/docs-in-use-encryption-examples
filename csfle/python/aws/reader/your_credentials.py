import re

_credentials = {
    # Mongo Paths + URI
    "MONGODB_URI": "<your MongoDB URI here>",
    "SHARED_LIB_PATH": "<Absolute path of your Automatic Encryption Shared Library>",
    # AWS Credentials
    "AWS_ACCESS_KEY_ID": "<your AWS access key ID here>",
    "AWS_SECRET_ACCESS_KEY": "<your AWS secret access key here>",
    "AWS_KEY_REGION": "<your AWS key region>",
    "AWS_KEY_ARN": "<your AWS key ARN>",
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
