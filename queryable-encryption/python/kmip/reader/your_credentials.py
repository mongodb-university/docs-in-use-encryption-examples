import re

_credentials = {
    # Mongo Paths + URI
    "MONGODB_URI": "<your MongoDB URI here>",
    "SHARED_LIB_PATH": "<path to CSFLE shared library>",
    # KMIP Credentials
    "KMIP_KMS_ENDPOINT": "<endpoint for your KMIP KMS. Default is 'localhost:5698'>",
    "KMIP_TLS_CA_FILE": "<full path to your KMIP certificate authority file. Default is '<path to this repo>/kmip_utils/certs/ca.pem'>",
    "KMIP_TLS_CERT_FILE": "<full path to your client certificate file. Default is '<path to this repo>/kmip_utils/certs/client.pem'>",
    "KMIP_TLS_CERT_P12": "<full path to your client certificate p12 file. Default is '<path to this repo>/kmip_utils/certs/pcks_client.p12'>",
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
