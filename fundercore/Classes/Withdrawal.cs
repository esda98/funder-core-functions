using Newtonsoft.Json;
using System;
using Npgsql;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Classes {
    class Withdrawal {
        [JsonProperty("id")]
        public Guid id;
        [JsonProperty("item_id")]
        public Guid itemId;
        [JsonProperty("seller_id")]
        public Guid sellerId;
        [JsonProperty("price_at_withdrawal")]
        public decimal price;
        [JsonProperty("withdrawal_time")]
        public DateTime timeOf;
        [JsonProperty("handler_user_id")]
        public string handler;
        [JsonProperty("quantity")]
        public int quantity;

        public bool validBeforeAdd() {
            return itemId != Guid.Empty && sellerId != Guid.Empty && price >= 0 && handler != null && handler.Trim() != "" && quantity > 0;
        }

        public void populateForNew() {
            id = Guid.NewGuid();
            timeOf = DateTime.Now;
        }

        public NpgsqlParameter[] addParams() {
            return new NpgsqlParameter[] { new NpgsqlParameter("@id", id), new NpgsqlParameter("@itemId", itemId), new NpgsqlParameter("@sellerId", sellerId),
                new NpgsqlParameter("@price", price), new NpgsqlParameter("@time", timeOf), new NpgsqlParameter("@handler", handler), new NpgsqlParameter("@quantity", quantity)};
        }

    }


    class WithdrawalBalance : Withdrawal {
        [JsonProperty("balance")]
        public decimal balance;
        [JsonProperty("item_name")]
        public string itemName;
    }

}
