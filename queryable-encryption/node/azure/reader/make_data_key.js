const { MongoClient, Binary } = require("mongodb");
const { ClientEncryption } = require("mongodb-client-encryption");

const keyVaultDatabase = "encryption";
const keyVaultCollection = "__keyVault";
const keyVaultNamespace = `${keyVaultDatabase}.${keyVaultCollection}`;
const secretDB = "medicalRecords";
const secretCollection = "patients";

const { getCredentials } = require("./your_credentials");
credentials = getCredentials();

// start-kmsproviders
const provider = "azure";
const kmsProviders = {
  azure: {
    tenantId: credentials["AZURE_TENANT_ID"],
    clientId: credentials["AZURE_CLIENT_ID"],
    clientSecret: credentials["AZURE_CLIENT_SECRET"],
  },
};
// end-kmsproviders

// start-datakeyopts
const masterKey = {
  keyVaultEndpoint: credentials["AZURE_KEY_VAULT_ENDPOINT"],
  keyName: credentials["AZURE_KEY_NAME"],
};
// end-datakeyopts

async function run() {
  // start-create-index
  const uri = credentials.MONGODB_URI;
  const keyVaultClient = new MongoClient(uri);
  await keyVaultClient.connect();
  const keyVaultDB = keyVaultClient.db(keyVaultDatabase);
  // Drop the Key Vault Collection in case you created this collection
  // in a previous run of this application.
  await keyVaultDB.dropDatabase();
  const keyVaultColl = keyVaultDB.collection(keyVaultCollection);
  await keyVaultColl.createIndex(
    { keyAltNames: 1 },
    {
      unique: true,
      partialFilterExpression: { keyAltNames: { $exists: true } },
    }
  );
  // end-create-index

  // start-create-dek
  const clientEnc = new ClientEncryption(keyVaultClient, {
    keyVaultNamespace: keyVaultNamespace,
    kmsProviders: kmsProviders,
  });
  const dek1 = await clientEnc.createDataKey(provider, {
    masterKey: masterKey,
    keyAltNames: ["dataKey1"],
  });
  const dek2 = await clientEnc.createDataKey(provider, {
    masterKey: masterKey,
    keyAltNames: ["dataKey2"],
  });
  const dek3 = await clientEnc.createDataKey(provider, {
    masterKey: masterKey,
    keyAltNames: ["dataKey3"],
  });
  const dek4 = await clientEnc.createDataKey(provider, {
    masterKey: masterKey,
    keyAltNames: ["dataKey4"],
  });
  // end-create-dek

  // start-create-enc-collection
  const encryptedFieldsMap = {
    [`${secretDB}.${secretCollection}`]: {
      fields: [
        {
          keyId: dek1,
          path: "patientId",
          bsonType: "int",
          queries: { queryType: "equality" },
        },
        {
          keyId: dek2,
          path: "medications",
          bsonType: "array",
        },
        {
          keyId: dek3,
          path: "patientRecord.ssn",
          bsonType: "string",
          queries: { queryType: "equality" },
        },
        {
          keyId: dek4,
          path: "patientRecord.billing",
          bsonType: "object",
        },
      ],
    },
  };
  const extraOptions = {
    cryptSharedLibPath: credentials["SHARED_LIB_PATH"],
  };
  const encClient = new MongoClient(uri, {
    autoEncryption: {
      keyVaultNamespace,
      kmsProviders,
      extraOptions,
      encryptedFieldsMap,
    },
  });
  await encClient.connect();
  const newEncDB = encClient.db(secretDB);
  // Drop the encrypted collection in case you created this collection
  // in a previous run of this application.
  await newEncDB.dropDatabase();
  await newEncDB.createCollection(secretCollection);
  console.log("Created encrypted collection!");
  // end-create-enc-collection
  await keyVaultClient.close();
  await encClient.close();
}

run().catch(console.dir);
