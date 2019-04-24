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
    public static class ItemFunctions {
        [FunctionName("GetItemsForAccount")]
        public static async Task<IActionResult> getItems([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
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

            var result = await ItemFunctionsModel.getItems(accountId.Value);
            return new OkObjectResult(result);
        }

        [FunctionName("AddItem")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Item item;
            try {
                var bodyString = await req.ReadAsStringAsync();
                item = JsonConvert.DeserializeObject<Item>(bodyString);
            } catch (Exception ex) {
                Logger.write($"Invalid Add given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Item");
            }
            //ensure a valid item was given and parsed out
            if (item == null || !item.validBeforeAdd()) {
                return new BadRequestObjectResult("Invalid Input for Item");
            }

            var result = await ItemFunctionsModel.addItem(item);
            return new OkObjectResult(result);
        }


        [FunctionName("GetFundraisersForItem")]
        public static async Task<IActionResult> getFundraisersForItem([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Guid? accountId = null;
            Guid? itemId = null;
            try {
                //parse out the account id and item id from query string
                accountId = Guid.Parse(req.Query["accountId"]);
                itemId = Guid.Parse(req.Query["itemId"]);
            } catch (Exception ex) {
                Logger.write($"Invalid Get given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input");
            }
            //ensure a valid return
            if (accountId == null) {
                return new BadRequestObjectResult("Invalid Input for Account ID");
            }
            if (itemId == null) {
                return new BadRequestObjectResult("Invalid Input for Item ID");
            }

            var result = await ItemFunctionsModel.getFundraisersForItem(accountId.Value, itemId.Value);
            return new OkObjectResult(result);
        }
    }
}
