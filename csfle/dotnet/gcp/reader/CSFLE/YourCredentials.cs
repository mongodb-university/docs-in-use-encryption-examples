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
                {"MONGOCRYPTD_PATH", "<path to mongocryptd>"},
                
                // GCP Credentials
                {"GCP_EMAIL", "<your GCP email>"},
                {"GCP_PRIVATE_KEY", "<your GCP private key>"},
                {"GCP_PROJECT_ID", "<your project id>"},
                {"GCP_LOCATION", "<your location>"},
                {"GCP_KEY_RING", "<your key ring>"},
                {"GCP_KEY_NAME", "<your key name>"},
                {"GCP_KEY_VERSION", "<your key version>"},

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