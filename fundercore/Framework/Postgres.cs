using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using fundercore.Data;
using Npgsql;

namespace fundercore.Framework
{
    public static class Postgres
    {
        private static PostgresSecret secret;
        public static async Task<List<dynamic>> getAllRecordsFromColumnZero(string query, NpgsqlParameter[] parameters)
        {
            var columnResults = new List<dynamic>();
            //get secret info if not already generated
            if (await warmConnection() == false) { return null; }
            try
            {
                using (var conn = new NpgsqlConnection(secret.generateConnectionString()))
                {
                    Logger.write("about to open connection");
                    conn.Open();
                    Logger.write("opened the connection");
                    //once connected to the database, get all the tenants
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);
                        cmd.Prepare();
                        //go through all records, read their returned json object into the list, and return it
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                columnResults.Add(JsonConvert.DeserializeObject<dynamic>(reader.GetString(0)));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.write($"CAUGHT ERR - Could not retrieve records for query {query} with parameter count {parameters.Length} ExceptionMessage: {ex.Message}");
                return null;
            }
            //return found objects
            return columnResults;
        }

        public static async Task<bool> warmConnection()
        {
            if (secret == null)
            {
                var newSecret = new PostgresSecret();
                if (await newSecret.retrieve() == false)
                {
                    return false;
                }
                secret = newSecret;
            }
            return true;
        }
    }

    class PostgresSecret
    {
        private string host, username, password, database;

        public async Task<bool> retrieve()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            try {
                var hostReq = keyVaultClient.GetSecretAsync(AzureConstants.KEY_VAULT_ENDPOINT, AzureConstants.SECRET_NAME_HOST);
                var userReq = keyVaultClient.GetSecretAsync(AzureConstants.KEY_VAULT_ENDPOINT, AzureConstants.SECRET_NAME_NAME);
                var passReq = keyVaultClient.GetSecretAsync(AzureConstants.KEY_VAULT_ENDPOINT, AzureConstants.SECRET_NAME_PASSWORD);
                var dbReq = keyVaultClient.GetSecretAsync(AzureConstants.KEY_VAULT_ENDPOINT, AzureConstants.SECRET_NAME_DATABASE);
                host = (await hostReq).Value;
                username = (await userReq).Value;
                password = (await passReq).Value;
                database = (await dbReq).Value;
            } catch (Exception ex) {
                Logger.write("Failed to retrieve secret: " + ex.Message);
                return false;
            }
            Logger.write("Successfully retrieved secret");
            return true;
        }

        public bool isValid()
        {
            Logger.write($"checking if is valid: {host},{username},{password},{database}");
            //ensure values are valid
            if (host == null || host.Trim() == "")
            {
                return false;
            }

            if (username == null || username.Trim() == "")
            {
                return false;
            }

            if (password == null || password.Trim() == "")
            {
                return false;
            }

            if (database == null || database.Trim() == "")
            {
                return false;
            }

            //clean values
            host = host.Trim();
            username = username.Trim();
            password = password.Trim();
            database = database.Trim();
            return true;
        }

        public string generateConnectionString()
        {
            return $"Host={host};Username={username};Password='{password}';Database={database};SSLMode='Prefer'";
        }
    }
}