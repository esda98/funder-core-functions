using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Classes {
    class ItemFundraisers {
        [JsonProperty("item_id", Required = Required.Always)]
        public Guid itemId;
        [JsonProperty("fundraiser_ids", Required = Required.Always)]
        public Guid[] fundIds;

        public bool valid() {
            return itemId != Guid.Empty;
        }

    }
}
