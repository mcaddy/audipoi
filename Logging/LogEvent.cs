//-----------------------------------------------------------------------
// <copyright file="LogEvent.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Logging
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Azure Function
    /// </summary>
    public static class LogEvent
    {
        /// <summary>
        /// Azure Function - Log Event
        /// </summary>
        /// <param name="req">Http Request</param>
        /// <param name="username">The Username</param>
        /// <param name="message">The Message</param>
        /// <param name="log">Logger Instance</param>
        /// <returns>A Task</returns>
        [FunctionName("LogEvent")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "LogEvent/{username}/{Message}")] HttpRequest req,
            string username,
            string message,
            ILogger log)
        {
            log.LogInformation($"Log Event invoked for {username}");

            Persist(username, message);

            return (ActionResult)new OkObjectResult($"Event Logged");
        }

        /// <summary>
        /// Log the event to persistent store
        /// </summary>
        /// <param name="username">username of the caller</param>
        /// <param name="message">message from the caller</param>
        private static async void Persist(string username, string message)
        {
            string accountKey = Environment.GetEnvironmentVariable("AccountKey");
            string accountName = Environment.GetEnvironmentVariable("AccountName");

            // Implement the accout, set true for https for SSL.
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);
            CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

            // Setup our container we are going to use and create it.
            CloudBlobContainer container = blobClient.GetContainerReference("logs");
            await container.CreateIfNotExistsAsync();

            // Build my typical log file name.
            DateTime date = DateTime.Today;
            DateTime dateLogEntry = DateTime.Now;

            // This creates a reference to the append blob we are going to use.
            CloudAppendBlob appBlob = container.GetAppendBlobReference(
                string.Format("{0}{1}", date.ToString("yyyyMMdd"), ".log"));

            // Now we are going to check if todays file exists and if it doesn't we create it.
            if (!await appBlob.ExistsAsync())
            {
                await appBlob.CreateOrReplaceAsync();
            }

            // Add the entry to our log.
            await appBlob.AppendTextAsync($"{dateLogEntry.ToString("o")}-{username}-{message}\r\n");
        }
    }
}
