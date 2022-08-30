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
	"SHARED_LIB_PATH": "<path to CSFLE shared library>",
	// AWS Credentials
	"AWS_ACCESS_KEY_ID":     "<your AWS access key ID here>",
	"AWS_SECRET_ACCESS_KEY": "<your AWS secret access key here>",
	"AWS_KEY_REGION":        "<your AWS key region>",
	"AWS_KEY_ARN":           "<your AWS key ARN>",
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
