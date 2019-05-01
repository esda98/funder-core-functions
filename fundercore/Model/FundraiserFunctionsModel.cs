using System;
using System.Collections.Generic;
using System.Text;
using fundercore.Data;
using fundercore.Classes;
using fundercore.Framework;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace fundercore.Model {
    static class FundraiserFunctionsModel {
        public static async Task<Result> addFundraiser(FundraiserWithItems fund) {
            //generate a new id for this fundraiser
            fund.generateId();
            Logger.write("New Fundraiser: " + JsonConvert.SerializeObject(fund));
            //execute query to database
            var addResult = await Postgres.getAll<Result>(new PgQuery(Queries.ADD_FUNDRAISER, new NpgsqlParameter("@id", fund.id),
                new NpgsqlParameter("@acctId", fund.accountId), new NpgsqlParameter("@name", fund.name),
                new NpgsqlParameter("@start", NpgsqlDbType.Date) { Value = fund.start }, new NpgsqlParameter("@end", NpgsqlDbType.Date) { Value = fund.end }));

            Logger.write("Result: " + JsonConvert.SerializeObject(addResult));
            //ensure valid response was returned
            if (addResult == null || addResult.Count != 1 || addResult[0].success == false) {
                return new Result(false, "Unable to create fundraiser");
            }

            //add the items to this fundraiser
            var fundItems = new FundraiserItems { fundId = fund.id };
            var itemIds = new List<Guid>();
            foreach (var i in fund.items) {
                if (i.isTied) {
                    itemIds.Add(i.id);
                }
            }
            fundItems.itemIds = itemIds.ToArray();
            return await setFundraiserItems(fundItems);           
        }

        public static async Task<Result> getFundraisers(Guid accountId) {
            var getResult = await Postgres.getAll<Fundraiser>(new PgQuery(Queries.GET_FUNDRAISER, new NpgsqlParameter("@acctId", accountId)));
            if (getResult == null) {
                return new Result(false, "Failed to retrieve fundraisers for given account");
            }
            return new Result(true, JsonConvert.SerializeObject(getResult));
        }

        public static async Task<Result> getItemsForFundraiser(Guid accountId, Guid fundraiserId) {
            var itemsTied = await Postgres.getAll<TiedItem>(new PgQuery(Queries.GET_FUNDRAISER_ITEMS,
                new NpgsqlParameter("@acctId", accountId), new NpgsqlParameter("@fundId", fundraiserId)));
            if (itemsTied == null) {
                return new Result(false, "Failed to query database");
            } else {
                return new Result(true, JsonConvert.SerializeObject(itemsTied));
            }
        }

        public static async Task<Result> editFundraiser(FundraiserWithItems fund) {
            var editResult = await Postgres.getAll<Result>(new PgQuery(Queries.EDIT_FUNDRAISER,
                new NpgsqlParameter("@fund", NpgsqlDbType.Json) { Value = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(fund)) }));
            if (editResult == null || editResult.Count != 1 || editResult[0].success == false) {
                return new Result(false, "Failed to access database");
            }
            //set the items to this fundraiser
            var fundItems = new FundraiserItems { fundId = fund.id };
            var itemIds = new List<Guid>();
            foreach (var i in fund.items) {
                if (i.isTied) {
                    itemIds.Add(i.id);
                }
            }
            fundItems.itemIds = itemIds.ToArray();
            return await setFundraiserItems(fundItems);
        }

        public static async Task<Result> setFundraiserItems(FundraiserItems fundItems) {
            var setResult = await Postgres.getAll<Result>(new PgQuery(Queries.SET_FUNDRAISER_ITEMS,
                new NpgsqlParameter("@fundId", fundItems.fundId), new NpgsqlParameter("@itemIds", fundItems.itemIds)));
            if (setResult == null || setResult.Count != 1) {
                return new Result(false, "Failed to access database");
            } else {
                return setResult[0];
            }
        }


    }
}
