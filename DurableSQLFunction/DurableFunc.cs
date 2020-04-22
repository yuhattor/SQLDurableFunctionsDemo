using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace DurableSQLFunction
{
    public class DurableFunc
    {
        private readonly SqlConnection _sql;

        public DurableFunc(SqlConnection sql)
        {
            _sql = sql;
        }

        [FunctionName("DurableFunc")]
        public static async Task<List<object>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<object>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<object>("DurableFunc_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<object>("DurableFunc_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<object>("DurableFunc_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("DurableFunc_Hello")]
        public async Task<IActionResult> SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");

            var s = new SqlCommand("select * from dbo.student", _sql);
            var r = await s.ExecuteReaderAsync();

            return new OkResult();
        }

        [FunctionName("DurableFunc_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunc", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}