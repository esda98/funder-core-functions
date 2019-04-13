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

namespace fundercore
{
    public static class TestFunctions
    {
        [FunctionName("testPGRetrieve")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post", Route = null)]HttpRequest req, ILogger log)
        {
            Logger.initialize(log);
            Logger.write("Entered function");
            var results = await TestFunctionsModel.getTestStrings();
            return results != null
                ? (ActionResult)new OkObjectResult(results)
                : new BadRequestObjectResult("Failed to retrieve test strings from DB");
        }
    }
}
