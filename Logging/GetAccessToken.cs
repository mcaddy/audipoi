//-----------------------------------------------------------------------
// <copyright file="GetAccessToken.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Logging
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// Azure Function - Get Access Token
    /// </summary>
    public static class GetAccessToken
    {
        /// <summary>
        /// Azure Function - Get Access Token
        /// </summary>
        /// <param name="req">Source Http Request</param>
        /// <param name="username">Username for the token</param>
        /// <param name="log">Logger Instance</param>
        /// <returns>A token if we are happy for user to store csv in the cloud</returns>
        [FunctionName("GetAccessToken")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAccessToken/{username}")] HttpRequest req, string username, ILogger log)
        {
            string clientIp = GetIpFromRequestHeaders(req);

            log.LogInformation($"Access Token Request for {username} from {clientIp}");

            // Don't issue tokens unless we've got a username
            if (string.IsNullOrEmpty(username))
            {
                return new BadRequestObjectResult(string.Empty);
            }

            string accountKey = Environment.GetEnvironmentVariable("AccountKey");
            string accountName = Environment.GetEnvironmentVariable("AccountName");

            // To create the account SAS, you need to use your shared key credentials. Modify for your account.
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create a new access policy for the account.
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.Write | SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            return (ActionResult)new OkObjectResult(storageAccount.GetSharedAccessSignature(policy));
        }

        /// <summary>
        /// Get the IP address from the request headers
        /// </summary>
        /// <param name="request">Source request</param>
        /// <returns>IP Address if one can be identified</returns>
        private static string GetIpFromRequestHeaders(HttpRequest request)
        {
            if (request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues values))
            {
                return values.FirstOrDefault().Split(new char[] { ',' }).FirstOrDefault().Split(new char[] { ':' }).FirstOrDefault();
            }

            return string.Empty;
        }
    }
}
