using fundercore.Classes;
using fundercore.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fundercore.Framework;
using Npgsql;
using Newtonsoft.Json;

namespace fundercore.Model {
    static class WithdrawalFunctionsModel {
        public static async Task<Result> add(Withdrawal withdrawal) {
            withdrawal.populateForNew();
            var dbResult = await Postgres.getAll<Result>(new PgQuery(Queries.ADD_WITHDRAWAL, withdrawal.addParams()));
            if (dbResult == null || dbResult.Count != 1) {
                return new Result(false, "Unable to access database");
            }
            return dbResult[0];
        }

        public static async Task<Result> get(Guid accountId) {
            var getResult = await Postgres.getAll<WithdrawalBalance>(new PgQuery(Queries.GET_WITHDRAWALS, new NpgsqlParameter("@acctId", accountId)));
            if (getResult == null) {
                return new Result(false, "Failed to retrieve withdrawals for given account");
            }
            return new Result(true, JsonConvert.SerializeObject(getResult));
        }
 
    }
}
