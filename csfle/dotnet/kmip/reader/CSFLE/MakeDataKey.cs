using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Encryption;
using System.Security.Cryptography.X509Certificates;
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
            var provider = "kmip";
            var kmipEndpoint = credentials["KMIP_KMS_ENDPOINT"];
            var kmipKmsOptions = new Dictionary<string, object>
            {
               { "endpoint", kmipEndpoint },
            };
            kmsProviders.Add(provider, kmipKmsOptions);
            // end-kmsproviders

            // start-datakeyopts
            var dataKeyOptions = new DataKeyOptions(
                masterKey: new BsonDocument { } // an empty key object prompts your KMIP provider to generate a new Customer Master Key
            );
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

            // start-create-tls 
            var tlsOptions = new Dictionary<string, SslSettings>();
            var sslSettings = new SslSettings();
            var clientCertificate = new X509Certificate2(credentials["KMIP_TLS_CERT_P12"]);
            sslSettings.ClientCertificates = new List<X509Certificate>() {
                clientCertificate,
             };
            tlsOptions.Add(provider, sslSettings);
            // end-create-tls

            // start-create-dek
            var clientEncryptionOptions = new ClientEncryptionOptions(
                keyVaultClient: keyVaultClient,
                keyVaultNamespace: keyVaultNamespace,
                kmsProviders: kmsProviders,
                tlsOptions: tlsOptions
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
