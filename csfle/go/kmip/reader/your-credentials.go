package main

import (
	"fmt"
	"log"
	"regexp"
	"strings"
)

var credentials = map[string]string{
	// Mongo Paths + URI
	"MONGODB_URI":     "<your MongoDB URI here>",
	"SHARED_LIB_PATH": "<Full path to your Automatic Encryption Shared Library>",
	// KMIP Credentials
	"KMIP_KMS_ENDPOINT":  "<endpoint for your KMIP KMS. Default is 'localhost:5698'>",
	"KMIP_TLS_CA_FILE":   "<full path to your KMIP certificate authority file. Default is '<path to this repo>/kmip_utils/certs/ca.pem'>",
	"KMIP_TLS_CERT_FILE": "<full path to your client certificate file. Default is '<path to this repo>/kmip_utils/certs/client.pem'>",
	"KMIP_TLS_CERT_P12":  "<full path to your client certificate p12 file. Default is '<path to this repo>/kmip_utils/certs/pcks_client.p12'>",
}

// check if credentials object contains placeholder values
func check_for_placeholders() {
	var error_buffer []string
	placeholder_pattern, _ := regexp.Compile("^<.*>$")
	for key, value := range credentials {
		// check for placeholder text
		if placeholder_pattern.MatchString(string(value)) {
			error_message := fmt.Sprintf("You must fill out the %s field of your credentials object.\n", key)
			error_buffer = append(error_buffer, error_message)
		}
	}
	// raise an error if errors in buffer
	if len(error_buffer) > 0 {
		message := strings.Join(error_buffer[:], "\n")
		log.Fatal(message)
	}
}

// return credentials object and ensure it has been populated
func GetCredentials() map[string]string {
	check_for_placeholders()
	return credentials
}
