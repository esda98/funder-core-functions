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
    public static class PaymentFunctions {
        [FunctionName("AddPayment")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log) {
            Logger.initialize(log);
            Logger.write("Entered function");

            Payment payment;
            try {
                var bodyString = await req.ReadAsStringAsync();
                payment = JsonConvert.DeserializeObject<Payment>(bodyString);
            } catch (Exception ex) {
                Logger.write($"Invalid Add given exception: {ex.Message} with stack trace: {ex.StackTrace}");
                return new BadRequestObjectResult("Invalid Input for Payment");
            }
            //ensure a valid item was given and parsed out
            if (payment == null) {
                return new BadRequestObjectResult("Invalid Input for Payment");
            }

            var result = await PaymentFunctionsModel.add(payment);
            return new OkObjectResult(result);
        }
    }
}
