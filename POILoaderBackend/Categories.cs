//-----------------------------------------------------------------------
// <copyright file="Categories.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------

namespace POILoaderBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Mcaddy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function - Categories
    /// </summary>
    public static class Categories
    {
        /// <summary>
        /// Azure Function - Categories
        /// </summary>
        /// <param name="req">Calling request</param>
        /// <param name="log">Logger instance</param>
        /// <returns>A Http Response</returns>
        [FunctionName("Categories")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Categories Requested");

            Dictionary<int, string> categoriesList = new Dictionary<int, string>();

            foreach (CategoryEnum item in Enum.GetValues(typeof(CategoryEnum)))
            {
                if (item != CategoryEnum.None)
                {
                    categoriesList.Add((int)item, item.ToDescriptionString());
                }
            }

            string jsonToReturn = JsonConvert.SerializeObject(categoriesList);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }
}
