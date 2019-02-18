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
            var results = await Postgres.getAllRecordsFromColumnZero(Queries.GET_TEST_STRINGS, new NpgsqlParameter[] {});
            Logger.write("retrieved data");
            return results;
        }
    }
}
