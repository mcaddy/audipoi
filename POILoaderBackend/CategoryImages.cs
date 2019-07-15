//-----------------------------------------------------------------------
// <copyright file="CategoryImages.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace POILoaderBackend
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function - Category Images
    /// </summary>
    public static class CategoryImages
    {
        /// <summary>
        /// Azure Function - Category Images
        /// </summary>
        /// <param name="req">Source request</param>
        /// <param name="categoriesRequested">Route Parameter</param>
        /// <param name="log">Logger Instance</param>
        /// <returns>Http Response</returns>
        [FunctionName("CategoryImages")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CategoryImages/{categoriesRequested}")] HttpRequest req, string categoriesRequested, ILogger log)
        {
            log.LogInformation($"Category Images Requested - {categoriesRequested}");

            // Convert the parameter to a Enum Flag
            CategoryEnum categories = CategoryEnum.None;
            if (int.TryParse(categoriesRequested, out int categoriesNumber))
            {
                categories = (CategoryEnum)categoriesNumber;
            }

            Dictionary<int, string> results = new Dictionary<int, string>();

            if (categories.HasFlag(CategoryEnum.NationalTrust))
            {
                results.Add((int)CategoryEnum.NationalTrust, Properties.Resources.NationalTrustIconBase64);
            }

            if (categories.HasFlag(CategoryEnum.EnglishHeritage))
            {
                results.Add((int)CategoryEnum.EnglishHeritage, Properties.Resources.EnglishHeritageIconBase64);
            }

            if (categories.HasFlag(CategoryEnum.RSPBReserves))
            {
                results.Add((int)CategoryEnum.RSPBReserves, Properties.Resources.RSBPIconBase64);
            }

            if (categories.HasFlag(CategoryEnum.HistoricHouses))
            {
                results.Add((int)CategoryEnum.HistoricHouses, Properties.Resources.HistoricHousesIconBase64);
            }

            if (categories.HasFlag(CategoryEnum.NationalTrustScotland))
            {
                results.Add((int)CategoryEnum.NationalTrustScotland, Properties.Resources.NationalTrustIconBase64);
            }

            if (categories.HasFlag(CategoryEnum.HistoricScotland))
            {
                results.Add((int)CategoryEnum.HistoricScotland, Properties.Resources.HistoricScotlandIconBase64);
            }

            string jsonToReturn = JsonConvert.SerializeObject(results);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }
}
