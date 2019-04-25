using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using fundercore.Data;
using Npgsql;

namespace fundercore.Framework {
    public static class Postgres {
        private static PostgresSecret secret;

        public static async Task<List<T>> getAll<T>(PgQuery prepQuery) {
            var columnResults = new List<T>();
            //get secret info if not already generated
            if (secret == null && await warmConnection() == false) { return null; }
            using (var conn = new NpgsqlConnection(secret.generateConnectionString(1))) {
                try {
                    //Console.WriteLine("about to open connection");
                    conn.Open();
                    conn.TypeMapper.UseJsonNet();
                    //Console.WriteLine("opened the connection");
                    using (var cmd = new NpgsqlCommand(prepQuery.query, conn)) {
                        foreach (var p in prepQuery.parameters)
                            cmd.Parameters.Add(p);
                        cmd.Prepare();
                        //go through all records, read their returned json object into the list, and return it
                        using (var reader = cmd.ExecuteReader())
                            columnResults = read<T>(reader);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(
                        $"CAUGHT ERR - Could not retrieve records for query {prepQuery.query} with parameter count {prepQuery.parameters.Length} ExceptionMessage: {ex.Message}");
                    conn.Close();
                    return null;
                }
                conn.Close();
            }

            //return found objects
            return columnResults;
        }

        public static async Task<List<List<T>>> getAll<T>(PgTran tran) {
            var start = DateTime.Now;
            var columnResults = new List<List<T>>();
            //get secret info if not already generated
            if (secret == null && await warmConnection() == false) { return null; }
            using (var conn = new NpgsqlConnection(secret.generateConnectionString(1))) {
                try {
                    //Console.WriteLine("about to open connection");
                    conn.Open();
                    //conn.TypeMapper.UseJsonNet();
                    //Console.WriteLine("opened the connection");
                    //once connected to the database, get all the tenants
                    using (var cmd = new NpgsqlCommand(tran.buildQuery(), conn)) {
                        foreach (var p in tran.parameters)
                            cmd.Parameters.Add(p);
                        cmd.Prepare();
                        //go through all records, read their returned json object into the list, and return it
                        using (var reader = cmd.ExecuteReader())
                            do {
                                columnResults.Add(read<T>(reader));
                            } while (reader.NextResult());
                    }
                } catch (Exception ex) {
                    Console.WriteLine(
                        $"CAUGHT ERR - Could not retrieve records for query {tran.buildQuery()} with parameter count {tran.parameters.Length} ExceptionMessage: {ex.Message}");
                    conn.Close();
                    var end1 = DateTime.Now;
                    Console.WriteLine($"Failed Query Duration: {(end1 - start).TotalMilliseconds}");
                    return null;
                }

                conn.Close();
                var end = DateTime.Now;
                Console.WriteLine($"Query Duration: {(end - start).TotalMilliseconds}");
            }
            //return found objects
            return columnResults;
        }

        private static List<T> read<T>(NpgsqlDataReader reader) {
            var results = new List<T>();
            while (reader.Read()) {
                if (reader[0] == DBNull.Value) { continue; }
                //when object returned by function is string, serialize it to given type, otherwise attempt to add it to the return results as is
                var valueAtZero = reader.GetValue(0);
                if (valueAtZero is string)
                    results.Add(JsonConvert.DeserializeObject<T>((string)valueAtZero));
                else
                    results.Add((T)valueAtZero);
            }
            return results;
        }

        public static async Task<bool> warmConnection(int poolMinimum = 1) {
            if (secret == null) {
                var newSecret = new PostgresSecret();
                if (await newSecret.retrieve() == false) {
                    Console.WriteLine("Failed to retrieve secret");
                    return false;
                }
                secret = newSecret;
            }
            //var connStr = secret.generateConnectionString(poolMinimum);
            //conn = new NpgsqlConnection(connStr);
            //Console.WriteLine($"Connection Created with Pool Minimum of {poolMinimum}");
            //Console.WriteLine("Connection Full State: " + conn.FullState);
            return true;
        }
    }

    public class PgQuery {
        public string query;
        public NpgsqlParameter[] parameters;

        public PgQuery(string givenQuery) {
            query = givenQuery;
            parameters = new NpgsqlParameter[] { };
        }

        public PgQuery(string givenQuery, params NpgsqlParameter[] givenParameters) {
            query = givenQuery;
            parameters = givenParameters;
        }

    }
    public class PgTran {
        public List<string> queries;
        public NpgsqlParameter[] parameters;

        public PgTran() {
            queries = new List<string>();
            parameters = new NpgsqlParameter[] { };
        }

        public string buildQuery() {
            return String.Join(";", queries);
        }

        public void setParams(params NpgsqlParameter[] newParams) {
            parameters = newParams;
        }

        public void setQueries(params string[] newQueries) {
            queries = new List<string>(newQueries);
        }

    }

    class PostgresSecret {
        private string host, username, password, database;

        public async Task<bool> retrieve() {
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

        public bool isValid() {
            Logger.write($"checking if is valid: {host},{username},{password},{database}");
            //ensure values are valid
            if (host == null || host.Trim() == "") {
                return false;
            }

            if (username == null || username.Trim() == "") {
                return false;
            }

            if (password == null || password.Trim() == "") {
                return false;
            }

            if (database == null || database.Trim() == "") {
                return false;
            }

            //clean values
            host = host.Trim();
            username = username.Trim();
            password = password.Trim();
            database = database.Trim();
            return true;
        }

        public string generateConnectionString(int minPool = 1) {
            return $"Host={host};Username={username};Password='{password}';Database={database};SSLMode='Prefer';MinPoolSize={minPool}";
        }
    }
}