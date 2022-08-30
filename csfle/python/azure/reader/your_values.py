import re

_credentials = {
    # Mongo Paths + URI
    "MONGODB_URI": "<your MongoDB URI here>",
    "MONGOCRYPTD_PATH": "<your MongoCryptd path>",
    # Azure Credentials
    "AZURE_TENANT_ID": "<your Azure tenant ID here>",
    "AZURE_CLIENT_ID": "<your Azure client ID here>",
    "AZURE_CLIENT_SECRET": "<your cleint secret here>",
    "AZURE_KEY_NAME": "<your key name here>",
    "AZURE_KEY_VERSION": "<your key version here>",
    "AZURE_KEY_VAULT_ENDPOINT": "<your key vault endpoint here>",
}


def check_for_placeholders():
    """check if credentials object contains placeholder values"""
    error_buffer = []
    placeholder_pattern = re.compile("^<*.>$")
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
