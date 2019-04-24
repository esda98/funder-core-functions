using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fundercore.Classes;
using fundercore.Data;
using fundercore.Framework;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace fundercore.Model {
    static class ItemFunctionsModel {
        public static async Task<Result> getItems(Guid accountId) {
            var getResult = await Postgres.getAll<Item>(new PgQuery(Queries.GET_ITEM, new NpgsqlParameter("@acctId", accountId)));
            if (getResult == null) {
                return new Result(false, "Failed to retrieve fundraisers for given account");
            }
            return new Result(true, JsonConvert.SerializeObject(getResult));
        }


        public static async Task<Result> addItem(Item item) {
            //generate a new id for this fundraiser
            item.generateId();
            Logger.write("New Item: " + JsonConvert.SerializeObject(item));
            //execute query to database
            var addResult = await Postgres.getAll<Result>(new PgQuery(Queries.ADD_ITEM, new NpgsqlParameter("@id", item.id),
                new NpgsqlParameter("@acctId", item.accountId), new NpgsqlParameter("@name", item.itemName),
                new NpgsqlParameter("@price", NpgsqlDbType.Numeric) { Value = item.price }, 
                new NpgsqlParameter("@quantity", NpgsqlDbType.Integer) { Value = item.quantity }));

            Logger.write("Result: " + JsonConvert.SerializeObject(addResult));
            //ensure valid response was returned
            if (addResult == null || addResult.Count != 1) {
                return new Result(false, "Unable to create fundraiser");
            } else {
                return addResult[0];
            }
        }

        public static async Task<Result> getFundraisersForItem(Guid accountId, Guid itemId) {
            var tiedFunds = await Postgres.getAll<TiedFundraiser>(new PgQuery(Queries.GET_ITEMS_FUNDRAISERS,
                new NpgsqlParameter("@acctId", accountId), new NpgsqlParameter("@itemId", itemId)));
            if (tiedFunds == null) {
                return new Result(false, "Failed to query database");
            } else {
                return new Result(true, JsonConvert.SerializeObject(tiedFunds));
            }
        }


    }
}
