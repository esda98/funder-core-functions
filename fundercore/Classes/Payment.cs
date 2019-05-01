using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace fundercore.Classes {
    class Payment {
        public Guid id;
        [JsonProperty("withdrawal_id")]
        public Guid withdrawalId;
        [JsonProperty("payment_type")]
        public string paymentType;
        public decimal amount;
        [JsonProperty("payment_time")]
        public DateTime paymentTime;
        [JsonProperty("handler_user_id")]
        public string handler;

        public void populateForNew() {
            id = Guid.NewGuid();
            paymentTime = DateTime.Now;
        }

        //@id, @wiId, @payType, @amt, @time, @handler
        public NpgsqlParameter[] addParams() {
            return new NpgsqlParameter[] { new NpgsqlParameter("@id", id), new NpgsqlParameter("wiId", withdrawalId), new NpgsqlParameter("@payType", paymentType),
                new NpgsqlParameter("@amt", amount), new NpgsqlParameter("@time", paymentTime), new NpgsqlParameter("@handler", handler) };
        }
    }
}
