package main

import (
	"fmt"
	"log"
	"os"
	"strings"
)

var credentials = map[string]string{
	// Mongo Paths + URI
	"MONGODB_URI":     os.Getenv("MONGODB_URI"),
	"SHARED_LIB_PATH": os.Getenv("SHARED_LIB_PATH"),
	// AWS Credentials
	"AWS_ACCESS_KEY_ID":     os.Getenv("AWS_ACCESS_KEY_ID"),
	"AWS_SECRET_ACCESS_KEY": os.Getenv("AWS_SECRET_ACCESS_KEY"),
	"AWS_KEY_REGION":        os.Getenv("AWS_KEY_REGION"),
	"AWS_KEY_ARN":           os.Getenv("AWS_KEY_ARN"),
}

// check if credentials object contains placeholder values
func check_for_placeholders() {
	var error_buffer []string
	for key, value := range credentials {
		// check for placeholder text
		if value == "" {
			error_message := fmt.Sprintf("You must fill out the %s environment variable.", key)
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
