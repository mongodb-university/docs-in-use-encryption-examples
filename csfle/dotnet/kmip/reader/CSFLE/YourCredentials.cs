using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Credentials
{
    class YourCredentials
    {

        private Dictionary<string, string> credentials = new Dictionary<string, string>()
            {
                // Mongo Paths + URI
                {"MONGODB_URI", "<your MongoDB URI>"},
                {"SHARED_LIB_PATH", "<Full path to your Automatic Encryption Shared Library>"},
                
                // KMIP Credentials
                {"KMIP_KMS_ENDPOINT", "<endpoint for your KMIP KMS. Default is 'localhost:5698'>"},
                {"KMIP_TLS_CA_FILE", "<full path to your KMIP certificate authority file. Default is '<path to this repo>/kmip_utils/certs/ca.pem'>"},
                {"KMIP_TLS_CERT_FILE", "<full path to your client certificate file. Default is '<path to this repo>/kmip_utils/certs/client.pem'>"},
                {"KMIP_TLS_CERT_P12", "<full path to your client certificate p12 file. Default is '<path to this repo>/kmip_utils/certs/pcks_client.p12'>"},

            };

        private void CheckThatValuesAreSet()
        {
            var placeholder = new Regex("^<.*>$");
            var errorBuffer = new List<String>();
            foreach (KeyValuePair<string, string> entry in credentials)
            {
                if (entry.Value != null && placeholder.IsMatch(Convert.ToString(entry.Value)))
                {
                    var message = String.Format("You must fill out the {0} field of your credentials object.", entry.Key);
                    errorBuffer.Add(message);
                }
            }
            if (errorBuffer.Count > 0)
            {
                var message = String.Join("\n", errorBuffer);
                throw new Exception(message);
            }
        }

        public Dictionary<string, string> GetCredentials()
        {
            CheckThatValuesAreSet();
            return credentials;
        }

    }
}