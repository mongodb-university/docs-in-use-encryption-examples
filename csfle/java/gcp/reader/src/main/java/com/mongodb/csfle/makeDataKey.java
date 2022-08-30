package com.mongodb.csfle;
/*
 * Copyright 2008-present MongoDB, Inc.

 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

import java.util.*;

import java.util.HashMap;
import java.util.Map;
import java.util.Base64;

import org.bson.BsonBinary;
import org.bson.BsonDocument;
import org.bson.BsonString;
import org.bson.BsonBoolean;
import org.bson.BsonInt32;

import com.mongodb.ClientEncryptionSettings;
import com.mongodb.ConnectionString;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.MongoClientSettings;
import com.mongodb.client.model.vault.DataKeyOptions;
import com.mongodb.client.vault.ClientEncryption;
import com.mongodb.client.vault.ClientEncryptions;
import com.mongodb.client.model.IndexOptions;


/*
 * - Reads master key from file "master-key.txt" in root directory of project, or creates one on a KMS
 * - Locates existing local encryption key from encryption.__keyVault collection, or from a KMS
 * - Prints base 64-encoded value of the data encryption key
 */
public class MakeDataKey {

    public static void main(String[] args) throws Exception {
        Map<String, String> credentials = YourValues.getCredentials();

        // start-kmsproviders
        String kmsProvider = "gcp";
        Map<String, Map<String, Object>> kmsProviders = new HashMap<String, Map<String, Object>>();
        Map<String, Object> providerDetails = new HashMap<>();
        providerDetails.put("email", credentials.get("GCP_EMAIL"));
        providerDetails.put("privateKey", credentials.get("GCP_PRIVATE_KEY"));
        kmsProviders.put(kmsProvider, providerDetails);
        // end-kmsproviders

        // start-datakeyopts
        BsonDocument masterKeyProperties = new BsonDocument();
        masterKeyProperties.put("provider", new BsonString(kmsProvider));
        masterKeyProperties.put("projectId", new BsonString(credentials.get("GCP_PROJECT_ID")));
        masterKeyProperties.put("location", new BsonString(credentials.get("GCP_LOCATION")));
        masterKeyProperties.put("keyRing", new BsonString(credentials.get("GCP_KEY_RING")));
        masterKeyProperties.put("keyName", new BsonString(credentials.get("GCP_KEY_NAME")));        
        // end-datakeyopts

        // start-create-index
        String connectionString = credentials.get("MONGODB_URI");
        String keyVaultDb = "encryption";
        String keyVaultColl = "__keyVault";
        String keyVaultNamespace = keyVaultDb + "." + keyVaultColl;
        MongoClient keyVaultClient = MongoClients.create(connectionString);

        // Drop the Key Vault Collection in case you created this collection
        // in a previous run of this application.
        keyVaultClient.getDatabase(keyVaultDb).getCollection(keyVaultColl).drop();
        // Drop the database storing your encrypted fields as all
        // the DEKs encrypting those fields were deleted in the preceding line.
        keyVaultClient.getDatabase("medicalRecords").getCollection("patients").drop();


        MongoCollection keyVaultCollection = keyVaultClient.getDatabase(keyVaultDb).getCollection(keyVaultColl);
        IndexOptions indexOpts = new IndexOptions().partialFilterExpression(new BsonDocument("keyAltNames", new BsonDocument("$exists", new BsonBoolean(true) ))).unique(true);
        keyVaultCollection.createIndex(new BsonDocument("keyAltNames", new BsonInt32(1)), indexOpts);
        keyVaultClient.close();
        // end-create-index 

        // start-create-dek
        ClientEncryptionSettings clientEncryptionSettings = ClientEncryptionSettings.builder()
                .keyVaultMongoClientSettings(MongoClientSettings.builder()
                        .applyConnectionString(new ConnectionString(connectionString))
                        .build())
                .keyVaultNamespace(keyVaultNamespace)
                .kmsProviders(kmsProviders)
                .build();
        
        MongoClient regularClient = MongoClients.create(connectionString);

        ClientEncryption clientEncryption = ClientEncryptions.create(clientEncryptionSettings);
        List keyAltNames = new ArrayList<String>();
        keyAltNames.add("demo-data-key");
        BsonBinary dataKeyId = clientEncryption.createDataKey(kmsProvider, new DataKeyOptions().masterKey(masterKeyProperties).keyAltNames(keyAltNames));
        String base64DataKeyId = Base64.getEncoder().encodeToString(dataKeyId.getData());
        System.out.println("DataKeyId [base64]: " + base64DataKeyId);
        clientEncryption.close();
        // end-create-dek
    }
}
