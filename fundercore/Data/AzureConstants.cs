using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Data
{
    static class AzureConstants
    {
        public const string KEY_VAULT_ENDPOINT = "https://funder-keys.vault.azure.net/";
        public const string SECRET_NAME_DATABASE = "pgDatabase";
        public const string SECRET_NAME_HOST = "pgHost";
        public const string SECRET_NAME_NAME = "pgName";
        public const string SECRET_NAME_PASSWORD = "pgPassword";
    }
}
