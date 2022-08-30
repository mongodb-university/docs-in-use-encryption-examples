using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Encryption;
using Credentials;

namespace Key
{

    class MakeDataKey
    {
        public static void MakeKey()
        {

            var credentials = new YourCredentials().GetCredentials();


            // start-kmsproviders
            var kmsProviders = new Dictionary<string, IReadOnlyDictionary<string, object>>();
            var provider = "azure";
            var azureTenantId = credentials["AZURE_TENANT_ID"];
            var azureClientId = credentials["AZURE_CLIENT_ID"];
            var azureClientSecret = credentials["AZURE_CLIENT_SECRET"];
            var azureKmsOptions = new Dictionary<string, object>
            {
               { "tenantId", azureTenantId },
               { "clientId", azureClientId },
               { "clientSecret", azureClientSecret },
            };
            kmsProviders.Add("azure", azureKmsOptions);
            // end-kmsproviders

            // start-datakeyopts
            var azureKeyName = credentials["AZURE_KEY_NAME"];
            var azureKeyVaultEndpoint = credentials["AZURE_KEY_VAULT_ENDPOINT"]; // typically <azureKeyName>.vault.azure.net
            var dataKeyOptions = new DataKeyOptions(
               masterKey: new BsonDocument
               {
                   { "keyName", azureKeyName },
                   { "keyVaultEndpoint", azureKeyVaultEndpoint },
               });
            // end-datakeyopts
            // start-create-index
            var connectionString = credentials["MONGODB_URI"];
            var keyVaultNamespace = CollectionNamespace.FromFullName("encryption.__keyVault");
            var keyVaultClient = new MongoClient(connectionString);
            var indexOptions = new CreateIndexOptions<BsonDocument>();
            indexOptions.Unique = true;
            indexOptions.PartialFilterExpression = new BsonDocument { { "keyAltNames", new BsonDocument { { "$exists", new BsonBoolean(true) } } } };
            var builder = Builders<BsonDocument>.IndexKeys;
            var indexKeysDocument = builder.Ascending("keyAltNames");
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeysDocument, indexOptions);
            var keyVaultDatabase = keyVaultClient.GetDatabase(keyVaultNamespace.DatabaseNamespace.ToString());
            // Drop the Key Vault Collection in case you created this collection
            // in a previous run of this application.  
            keyVaultDatabase.DropCollection(keyVaultNamespace.CollectionName);
            // Drop the database storing your encrypted fields as all
            // the DEKs encrypting those fields were deleted in the preceding line.
            keyVaultClient.GetDatabase("medicalRecords").DropCollection("patients");
            var keyVaultCollection = keyVaultDatabase.GetCollection<BsonDocument>(keyVaultNamespace.CollectionName.ToString());
            keyVaultCollection.Indexes.CreateOne(indexModel);
            // end-create-index


            // start-create-dek
            var clientEncryptionOptions = new ClientEncryptionOptions(
                keyVaultClient: keyVaultClient,
                keyVaultNamespace: keyVaultNamespace,
                kmsProviders: kmsProviders
                );

            var clientEncryption = new ClientEncryption(clientEncryptionOptions);
            List<string> keyNames = new List<string>();
            keyNames.Add("demo-data-key");
            var dataKeyId = clientEncryption.CreateDataKey(provider, dataKeyOptions.With(keyNames), CancellationToken.None);
            var dataKeyIdBase64 = Convert.ToBase64String(GuidConverter.ToBytes(dataKeyId, GuidRepresentation.Standard));
            Console.WriteLine($"DataKeyId [base64]: {dataKeyIdBase64}");
            // end-create-dek
        }
    }
}
