using System;
using System.Collections.Generic;
using System.Text;
using fundercore.Data;
using fundercore.Classes;
using fundercore.Framework;
using Npgsql;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fundercore.Model {
    class SellerFunctionsModel {
        public static async Task<Result> getSellers(Guid accountId) {
            var sellerResult = await Postgres.getAll<Seller>(new PgQuery(Queries.GET_SELLERS,
                new NpgsqlParameter("@acctId", accountId)));
            //ensure result returned properly
            if (sellerResult == null) {
                return new Result(false, "Failed to query database");
            }
            return new Result(true, JsonConvert.SerializeObject(sellerResult));
        }
    }
}
