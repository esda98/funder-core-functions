using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Classes {
    class Item {
        [JsonProperty("id")]
        public Guid id;
        [JsonProperty("account_id")]
        public Guid accountId;
        [JsonProperty("item_name")]
        public string itemName;
        [JsonProperty("quantity")]
        public int quantity;
        [JsonProperty("price")]
        public decimal price;


        public void generateId() {
            id = Guid.NewGuid();
        }

        public bool validBeforeAdd() {
            return accountId != Guid.Empty && itemName != null && itemName.Trim() != "";
        }

    }

    class TiedItem : Item {
        [JsonProperty("is_tied")]
        public bool isTied;
    }

}
