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
	// Azure Credentials
	"AZURE_TENANT_ID":          "<your Azure tenant ID here>",
	"AZURE_CLIENT_ID":          "<your Azure client ID here>",
	"AZURE_CLIENT_SECRET":      "<your cleint secret here>",
	"AZURE_KEY_NAME":           "<your key name here>",
	"AZURE_KEY_VERSION":        "<your key version here>",
	"AZURE_KEY_VAULT_ENDPOINT": "<your key vault endpoint here>",
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
