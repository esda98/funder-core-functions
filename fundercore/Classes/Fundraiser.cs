using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;

namespace fundercore.Classes {
    class Fundraiser {
        [JsonProperty("id")]
        public Guid id;
        [JsonProperty("account_id")]
        public Guid accountId;
        [JsonProperty("fundraiser_name")]
        public string name;
        [JsonProperty("start_date")]
        public DateTime start;
        [JsonProperty("end_date")]
        public DateTime end;

        public void generateId() {
            id = Guid.NewGuid();
        }

        public Fundraiser() {

        }

        public Fundraiser(Guid givenId, Guid givenAcctId, string givenName, DateTime givenStart, DateTime givenEnd) {
            id = givenId;
            accountId = givenAcctId;
            name = givenName;
            start = givenStart;
            end = givenEnd;
        }

        public bool validBeforeAdd() {
            return accountId != Guid.Empty && name != null && name.Trim() != "" && start != null && end != null;
        }

    }

    class TiedFundraiser : Fundraiser {
        [JsonProperty("is_tied")]
        public bool isTied;
    }

    class FundraiserWithItems : Fundraiser {
        public TiedItem[] items;
    }
}
