using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Classes {
    class FundraiserItems {
        [JsonProperty("fundraiser_id", Required = Required.Always)]
        public Guid fundId;
        [JsonProperty("item_ids", Required = Required.Always)]
        public Guid[] itemIds;

        public bool valid() {
            return fundId != Guid.Empty;
        }

    }
}
