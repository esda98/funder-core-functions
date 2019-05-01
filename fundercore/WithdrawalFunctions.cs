using fundercore.Classes;
using fundercore.Data;
using fundercore.Framework;
using fundercore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace fundercore {
    public static class WithdrawalFunctions {
        [FunctionName("AddWithdrawal")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Withdrawal withdrawal;
            try {
                var bodyString = await req.ReadAsStringAsync();
                withdrawal = JsonConvert.DeserializeObject<Withdrawal>(bodyString);
            } catch (Exception ex) {
                Logger.write($"Invalid Add given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Item");
            }
            //ensure a valid item was given and parsed out
            if (withdrawal == null || !withdrawal.validBeforeAdd()) {
                return new BadRequestObjectResult("Invalid Input for Item");
            }

            var result = await WithdrawalFunctionsModel.add(withdrawal);
            return new OkObjectResult(result);
        }

        [FunctionName("AddWithdrawals")]
        public static async Task<IActionResult> AddBulkWithdrawals([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            List<Withdrawal> withdrawals;
            try {
                var bodyString = await req.ReadAsStringAsync();
                withdrawals = JsonConvert.DeserializeObject<List<Withdrawal>>(bodyString);
            } catch (Exception ex) {
                Logger.write($"Invalid Add given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Item");
            }
            //ensure a valid item was given and parsed out
            if (withdrawals == null) {
                return new BadRequestObjectResult("Invalid Input for Item");
            }
            //ensure all passed objects are valid
            foreach (var w in withdrawals) {
                if (!w.validBeforeAdd()) {
                    return new BadRequestObjectResult("Invalid Input for Item");
                }
            }

            List<Result> results = new List<Result>();
            foreach(var w in withdrawals) {
                results.Add(await WithdrawalFunctionsModel.add(w));
            }
            return new OkObjectResult(results);
        }

        [FunctionName("GetWithdrawals")]
        public static async Task<IActionResult> GetWithdrawals([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Guid? accountId = null;
            try {
                //parse out the account id from query string
                accountId = Guid.Parse(req.Query["accountId"]);
            } catch (Exception ex) {
                Logger.write($"Invalid Get given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Account ID");
            }
            //ensure a valid return
            if (accountId == null) {
                return new BadRequestObjectResult("Invalid Input for Account ID");
            }

            var result = await WithdrawalFunctionsModel.get(accountId.Value);
            return new OkObjectResult(result);
        }

    }
}
