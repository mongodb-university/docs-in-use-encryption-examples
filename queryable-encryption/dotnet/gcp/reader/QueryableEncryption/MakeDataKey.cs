﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Encryption;
using Credentials;

namespace QueryableEncryption
{
    internal static class MakeDataKey
    {
        public static void MakeKey()
        {
            var credentials = new YourCredentials().GetCredentials();

            // start-kmsproviders
            var kmsProviders = new Dictionary<string, IReadOnlyDictionary<string, object>>();
            const string provider = "gcp";
            var gcpPrivateKey = credentials["GCP_PRIVATE_KEY"];
            var gcpEmail = credentials["GCP_EMAIL"];
            var gcpKmsOptions = new Dictionary<string, object>
            {
               { "privateKey", gcpPrivateKey },
               { "email", gcpEmail },
            };
            kmsProviders.Add(provider, gcpKmsOptions);

            var gcpDataKeyProjectId = credentials["GCP_PROJECT_ID"];
            var gcpDataKeyLocation = credentials["GCP_LOCATION"];
            var gcpDataKeyKeyRing = credentials["GCP_KEY_RING"];
            var gcpDataKeyKeyName = credentials["GCP_KEY_NAME"];
            // end-kmsproviders

            // start-datakeyopts
            DataKeyOptions GetDataKeyOptions(List<string> altNames)
            {
                var dataKeyOptions = new DataKeyOptions(
                   alternateKeyNames: altNames,
                   masterKey: new BsonDocument
                   {
                       { "projectId", gcpDataKeyProjectId },
                       { "location", gcpDataKeyLocation } ,
                       { "keyRing", gcpDataKeyKeyRing },
                       { "keyName", gcpDataKeyKeyName },
                   });
                return dataKeyOptions;
            }
            // end-datakeyopts

            // start-create-index
            var connectionString = credentials["MONGODB_URI"];
            var keyVaultNamespace = CollectionNamespace.FromFullName("encryption.__keyVault");
            var keyVaultClient = new MongoClient(connectionString);
            var indexOptions = new CreateIndexOptions<BsonDocument>
            {
                Unique = true,
                PartialFilterExpression = new BsonDocument
                    {{"keyAltNames", new BsonDocument {{"$exists", new BsonBoolean(true)}}}}
            };
            var builder = Builders<BsonDocument>.IndexKeys;
            var indexKeysDocument = builder.Ascending("keyAltNames");
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeysDocument, indexOptions);
            var keyVaultDatabase = keyVaultClient.GetDatabase(keyVaultNamespace.DatabaseNamespace.DatabaseName);
            // Drop the Key Vault Collection in case you created this collection
            // in a previous run of this application.
            keyVaultDatabase.DropCollection(keyVaultNamespace.CollectionName);
            var keyVaultCollection = keyVaultDatabase.GetCollection<BsonDocument>(keyVaultNamespace.CollectionName);
            keyVaultCollection.Indexes.CreateOne(indexModel);
            // end-create-index

            // start-create-dek
            var clientEncryptionOptions = new ClientEncryptionOptions(
                keyVaultClient,
                keyVaultNamespace,
                kmsProviders);
            var clientEncryption = new ClientEncryption(clientEncryptionOptions);
            var dataKeyOptions1 = GetDataKeyOptions(new List<string> { "dataKey1" });
            var dataKeyOptions2 = GetDataKeyOptions(new List<string> { "dataKey2" });
            var dataKeyOptions3 = GetDataKeyOptions(new List<string> { "dataKey3" });
            var dataKeyOptions4 = GetDataKeyOptions(new List<string> { "dataKey4" });


            BsonBinaryData CreateKeyGetID(DataKeyOptions options)
            {
                var dateKeyGuid = clientEncryption.CreateDataKey(provider, options, CancellationToken.None);
                return new BsonBinaryData(dateKeyGuid, GuidRepresentation.Standard);
            }

            var dataKeyId1 = CreateKeyGetID(dataKeyOptions1);
            var dataKeyId2 = CreateKeyGetID(dataKeyOptions2);
            var dataKeyId3 = CreateKeyGetID(dataKeyOptions3);
            var dataKeyId4 = CreateKeyGetID(dataKeyOptions4);
            // end-create-dek

            // start-create-enc-collection
            var encryptedCollectionNamespace = CollectionNamespace.FromFullName("medicalRecords.patients");
            var encryptedFieldsMap = new Dictionary<string, BsonDocument>
            {
                {
                    encryptedCollectionNamespace.FullName, new BsonDocument
                    {
                        {
                            "fields", new BsonArray
                            {
                                new BsonDocument
                                {
                                    {"keyId", dataKeyId1},
                                    {"path", new BsonString("patientId")},
                                    {"bsonType", new BsonString("int")},
                                    {
                                        "queries", new BsonDocument
                                        {
                                            {"queryType", new BsonString("equality")}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {"keyId", dataKeyId2},
                                    {"path", new BsonString("medications")},
                                    {"bsonType", new BsonString("array")},
                                },
                                new BsonDocument
                                {
                                    {"keyId", dataKeyId3},
                                    {"path", new BsonString("patientRecord.ssn")},
                                    {"bsonType", new BsonString("string")},
                                    {
                                        "queries", new BsonDocument
                                        {
                                            {"queryType", new BsonString("equality")}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {"keyId", dataKeyId4},
                                    {"path", new BsonString("patientRecord.billing")},
                                    {"bsonType", new BsonString("object")},
                                },
                            }
                        }
                    }
                }
            };

            var extraOptions = new Dictionary<string, object>()
            {
                {"cryptSharedLibPath", credentials["SHARED_LIB_PATH"]},
            };

            var autoEncryptionOptions = new AutoEncryptionOptions(
                keyVaultNamespace,
                kmsProviders,
                encryptedFieldsMap: encryptedFieldsMap,
                extraOptions: extraOptions);

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
            clientSettings.AutoEncryptionOptions = autoEncryptionOptions;
            var secureClient = new MongoClient(clientSettings);
            var encryptedDatabase = secureClient.GetDatabase(encryptedCollectionNamespace.DatabaseNamespace.DatabaseName);
            // Drop the encrypted collection in case you created this collection
            // in a previous run of this application.
            encryptedDatabase.DropCollection(encryptedCollectionNamespace.CollectionName);
            encryptedDatabase.CreateCollection(encryptedCollectionNamespace.CollectionName);
            Console.WriteLine("Created encrypted collection!");
            // end-create-enc-collection
        }
    }
}
