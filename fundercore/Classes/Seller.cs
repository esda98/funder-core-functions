using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Classes {
    class Seller {
        [JsonProperty("id")]
        public Guid id;
        [JsonProperty("account_id")]
        public Guid accountId;
        [JsonProperty("first_name")]
        public string firstName;
        [JsonProperty("last_name")]
        public string lastName;
        [JsonProperty("custom_identifier")]
        public int customId;
    }
}
