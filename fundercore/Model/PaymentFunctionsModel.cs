using fundercore.Classes;
using fundercore.Data;
using fundercore.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace fundercore.Model {
    class PaymentFunctionsModel {
        public static async Task<Result> add(Payment payment) {
            payment.populateForNew();
            var dbResult = await Postgres.getAll<Result>(new PgQuery(Queries.ADD_PAYMENT, payment.addParams()));
            if (dbResult == null || dbResult.Count != 1) {
                return new Result(false, "Unable to access database");
            }
            return dbResult[0];
        }
    }
}
