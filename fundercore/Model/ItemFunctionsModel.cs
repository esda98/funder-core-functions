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

        public static async Task<Result> updateItem(Item item) {
            var editResult = await Postgres.getAll<Result>(new PgQuery(Queries.EDIT_ITEM,
                new NpgsqlParameter("@item", NpgsqlDbType.Json) { Value = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item)) }));
            if (editResult == null || editResult.Count != 1) {
                return new Result(false, "Failed to access the database");
            } else {
                return editResult[0];
            }
        }

        public static async Task<Result> setItemFundraisers(ItemFundraisers itemFunds) {
            var setResult = await Postgres.getAll<Result>(new PgQuery(Queries.SET_ITEM_FUNDRAISERS,
                new NpgsqlParameter("@itemId", itemFunds.itemId), new NpgsqlParameter("@fundIds", itemFunds.fundIds)));
            if (setResult == null || setResult.Count != 1) {
                return new Result(false, "Failed to access the database");
            } else {
                return setResult[0];
            }
        }

        public static async Task<Result> delete(Item item) {
            var deleteResult = await Postgres.getAll<Result>(new PgQuery(Queries.DELETE_ITEM, new NpgsqlParameter("@itemId", item.id)));
            if (deleteResult == null || deleteResult.Count != 1) {
                return new Result(false, "Failed to access the database");
            } else {
                return deleteResult[0];
            }
        }


    }
}
