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
                {"SHARED_LIB_PATH", "<Absolute path of your Automatic Encryption Shared Library>"},


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