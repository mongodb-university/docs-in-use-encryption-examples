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
                {"SHARED_LIB_PATH", "<path to automatic encryption shared library>"},
                
                // Azure Credentials
                {"AZURE_TENANT_ID", "<your Azure tenant ID here>"},
                {"AZURE_CLIENT_ID", "<your Azure client ID here>"},
                {"AZURE_CLIENT_SECRET", "<your cleint secret here>"},
                {"AZURE_KEY_NAME", "<your key name here>"},
                {"AZURE_KEY_VERSION", "<your key version here>"},
                {"AZURE_KEY_VAULT_ENDPOINT", "<your key vault endpoint here>"},

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