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
using Newtonsoft.Json.Converters;

namespace fundercore {
    public static class FundraiserFunctions {
        [FunctionName("AddFundraiser")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            FundraiserWithItems fund;
            try {
                var bodyString = await req.ReadAsStringAsync();
                fund = JsonConvert.DeserializeObject<FundraiserWithItems>(bodyString, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            } catch (Exception ex) {
                Logger.write($"Invalid Add given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Fundraiser");
            }
            if (fund == null || !fund.validBeforeAdd()) {
                return new BadRequestObjectResult("Invalid Input for Fundraiser");
            }
            
            var result = await FundraiserFunctionsModel.addFundraiser(fund);
            return result.success
                ? (ActionResult)new OkObjectResult(result)
                : new BadRequestObjectResult("Could not create fundraiser");
        }

        [FunctionName("GetFundraisersForAccount")]
        public static async Task<IActionResult> getFundraisers([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
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

            var result = await FundraiserFunctionsModel.getFundraisers(accountId.Value);
            return new OkObjectResult(result);
        }

        [FunctionName("GetItemsForFundraiser")]
        public static async Task<IActionResult> getItemsForFundraiser([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Guid? accountId = null;
            Guid? fundraiserId = null;
            try {
                //parse out the account id and fundraiser id from query string
                accountId = Guid.Parse(req.Query["accountId"]);
                fundraiserId = Guid.Parse(req.Query["fundraiserId"]);
            } catch (Exception ex) {
                Logger.write($"Invalid Get given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input");
            }
            //ensure a valid return
            if (accountId == null) {
                return new BadRequestObjectResult("Invalid Input for Account ID");
            }
            if (fundraiserId == null) {
                return new BadRequestObjectResult("Invalid Input for Fundraiser ID");
            }

            var result = await FundraiserFunctionsModel.getItemsForFundraiser(accountId.Value, fundraiserId.Value);
            return new OkObjectResult(result);
        }

        [FunctionName("EditFundraiser")]
        public static async Task<IActionResult> EditFundraiser([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            FundraiserWithItems fund;
            try {
                var bodyString = await req.ReadAsStringAsync();
                fund = JsonConvert.DeserializeObject<FundraiserWithItems>(bodyString, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            } catch (Exception ex) {
                Logger.write($"Invalid Edit given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Fundraiser");
            }
            if (fund == null || !fund.validBeforeAdd()) {
                return new BadRequestObjectResult("Invalid Input for Fundraiser");
            }

            var result = await FundraiserFunctionsModel.editFundraiser(fund);
            return new OkObjectResult(result);
        }

        [FunctionName("SetFundraiserItems")]
        public static async Task<IActionResult> SetFundraiserItems([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            FundraiserItems fundItems;
            try {
                //force both properties to be present or error
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Error;
                var bodyString = await req.ReadAsStringAsync();
                fundItems = JsonConvert.DeserializeObject<FundraiserItems>(bodyString, settings);
            } catch (Exception ex) {
                Logger.write($"Invalid Edit given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Fundraiser Items");
            }
            if (fundItems == null || !fundItems.valid()) {
                return new BadRequestObjectResult("Invalid Input for Fundraiser");
            }

            var result = await FundraiserFunctionsModel.setFundraiserItems(fundItems);
            return new OkObjectResult(result);
        }

    }
}
