using fundercore.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using fundercore.Model;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using fundercore.Classes;
using System;

namespace fundercore {
    public static class SellerFunctions {
        [FunctionName("GetSellersForAccount")]
        public static async Task<IActionResult> getSellers([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
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

            var result = await SellerFunctionsModel.getSellers(accountId.Value);
            return new OkObjectResult(result);
        }
    }
}
