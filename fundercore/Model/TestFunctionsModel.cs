using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fundercore.Framework;
using fundercore.Data;
using Npgsql;
using Newtonsoft.Json;
using System.Text;

namespace fundercore.Model
{
    static class TestFunctionsModel
    {
        public static async Task<List<dynamic>> getTestStrings() {
            Logger.write("retrieving data");
            var results = await Postgres.getAll<dynamic>(new PgQuery(Queries.GET_TEST_STRINGS));
            Logger.write("retrieved data");
            return results;
        }
    }
}
