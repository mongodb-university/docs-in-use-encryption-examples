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
	"SHARED_LIB_PATH": "<Absolute path of your Automatic Encryption Shared Library>",
	// GCP Credentials
	"GCP_EMAIL":       "<your GCP email>",
	"GCP_PRIVATE_KEY": "<your GCP private key>",
	"GCP_PROJECT_ID":  "<your project id>",
	"GCP_LOCATION":    "<your location>",
	"GCP_KEY_RING":    "<your key ring>",
	"GCP_KEY_NAME":    "<your key name>",
	"GCP_KEY_VERSION": "<your key version>",
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
