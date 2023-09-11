const mongodb = require("mongodb");
const { MongoClient, Binary, ClientEncryption } = mongodb;

const { getCredentials } = require("./your_credentials");
credentials = getCredentials();

// start-kmsproviders
const provider = "gcp";
const kmsProviders = {
  gcp: {
    email: credentials["GCP_EMAIL"],
    privateKey: credentials["GCP_PRIVATE_KEY"],
  },
};
// end-kmsproviders

// start-datakeyopts
const masterKey = {
  projectId: credentials["GCP_PROJECT_ID"],
  location: credentials["GCP_LOCATION"],
  keyRing: credentials["GCP_KEY_RING"],
  keyName: credentials["GCP_KEY_NAME"],
};
// end-datakeyopts

async function main() {
  // start-create-index
  const uri = credentials.MONGODB_URI;
  const keyVaultDatabase = "encryption";
  const keyVaultCollection = "__keyVault";
  const keyVaultNamespace = `${keyVaultDatabase}.${keyVaultCollection}`;
  const keyVaultClient = new MongoClient(uri);
  await keyVaultClient.connect();
  const keyVaultDB = keyVaultClient.db(keyVaultDatabase);
  // Drop the Key Vault Collection in case you created this collection
  // in a previous run of this application.
  await keyVaultDB.dropDatabase();
  // Drop the database storing your encrypted fields as all
  // the DEKs encrypting those fields were deleted in the preceding line.
  await keyVaultClient.db("medicalRecords").dropDatabase();
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
  const client = new MongoClient(uri, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
  });
  await client.connect();

  const encryption = new ClientEncryption(client, {
    keyVaultNamespace,
    kmsProviders,
  });
  const key = await encryption.createDataKey(provider, {
    masterKey: masterKey,
    keyAltNames: ["demo-data-key"],
  });
  console.log("DataKeyId [base64]: ", key.toString("base64"));
  await keyVaultClient.close();
  await client.close();
  // end-create-dek
}
main();
