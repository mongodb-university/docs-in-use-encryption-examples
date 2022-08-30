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

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.regex.Pattern;

public class YourCredentials {
    private static Map<String, String> yourCredentials;
    static {
        yourCredentials = new HashMap<>();
        // Mongo Paths + URI
        yourCredentials.put("MONGODB_URI", "<your MongoDB URI here>");
        yourCredentials.put("MONGOCRYPTD_PATH", "<path to mongocryptd>");
        // KMIP Credentials
        yourCredentials.put("KMIP_KMS_ENDPOINT", "<endpoint for your KMIP KMS. Default is 'localhost:5698'>");
        yourCredentials.put("KMIP_TLS_CA_FILE", "<full path to your KMIP certificate authority file. Default is '<path to this repo>/kmip_utils/certs/ca.pem'>");
        yourCredentials.put("KMIP_TLS_CERT_FILE", "<full path to your client certificate file. Default is '<path to this repo>/kmip_utils/certs/client.pem'>");
        yourCredentials.put("KMIP_TLS_CERT_P12", "<full path to your client certificate p12 file. Default is '<path to this repo>/kmip_utils/certs/pcks_client.p12'>");

    }
    private static void checkPlaceholders() throws Exception {
        Pattern p = Pattern.compile("<.*>$");
        ArrayList<String> errorBuffer = new ArrayList<String>();
        for (Map.Entry<String,String> entry : yourCredentials.entrySet()) {
            if(p.matcher(String.valueOf(entry.getValue())).matches()){
                String message = String.format("The value for %s is empty. Please enter something for this value.", entry.getKey());
                errorBuffer.add(message);
            }
        }
        if (!errorBuffer.isEmpty()){
            String message = String.join("\n", errorBuffer);
            throw new Exception(message);
        }
    }
    public static Map<String, String> getCredentials() throws Exception {
        checkPlaceholders();
        return yourCredentials;
    }
}
