//-----------------------------------------------------------------------
// <copyright file="POIs.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace POILoaderBackend
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Mcaddy;
    using Mcaddy.AudiPoiDatabase;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function - Gets POIs
    /// </summary>
    public static class POIs
    {
        /// <summary>
        /// Azure Function - Gets POIs
        /// </summary>
        /// <param name="req">Source Http Request</param>
        /// <param name="categoriesRequested">The requested Categories</param>
        /// <param name="log">Logger Instance</param>
        /// <returns>Http Response</returns>
        [FunctionName("POIs")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "POIs/{categoriesRequested}")] HttpRequest req, string categoriesRequested, ILogger log)
        {
            log.LogInformation($"POIs - Categories = {categoriesRequested}");

            Collection<PointOfInterestCategory> results = new Collection<PointOfInterestCategory>();

            CategoryEnum categories = CategoryEnum.None;

            // Convert the parameter to a Enum Flag
            if (int.TryParse(categoriesRequested, out int categoriesNumber))
            {
                categories = (CategoryEnum)categoriesNumber;
            }

            if (categories.HasFlag(CategoryEnum.EnglishHeritage))
            {
                results.Add(GetFromPoiGraves(log, CategoryEnum.EnglishHeritage));
            }

            if (categories.HasFlag(CategoryEnum.NationalTrust))
            {
                results.Add(GetFromPoiGraves(log, CategoryEnum.NationalTrust));
            }

            if (categories.HasFlag(CategoryEnum.HistoricHouses))
            {
                results.Add(GetFromPoiGraves(log, CategoryEnum.HistoricHouses));
            }

            if (categories.HasFlag(CategoryEnum.HistoricScotland))
            {
                results.Add(GetFromPoiGraves(log, CategoryEnum.HistoricScotland));
            }

            if (categories.HasFlag(CategoryEnum.NationalTrustScotland))
            {
                results.Add(GetFromPoiGraves(log, CategoryEnum.NationalTrustScotland));
            }

            if (categories.HasFlag(CategoryEnum.RSPBReserves))
            {
                results.Add(GetRSBPReserves(log));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string jsonToReturn = JsonConvert.SerializeObject(results, settings);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Get the Latest RSBP Reserves file
        /// </summary>
        /// <param name="log">Logger Instance</param>
        /// <returns>Category complete with reserves</returns>
        private static PointOfInterestCategory GetRSBPReserves(ILogger log)
        {
            log.LogInformation("POIs - Category = RSPB Reserves");
            PointOfInterestCategory rspbReserves = new PointOfInterestCategory((int)CategoryEnum.RSPBReserves, CategoryEnum.RSPBReserves.ToDescriptionString(), null);

            WebClient client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string reservesCsv = client.DownloadString("https://www.rspb.org.uk/globalassets/downloads/documents/reserves/csv-data-file---reserves.csv");
            log.LogInformation($"POIs - CSV Size = {reservesCsv.Length}");

            rspbReserves.Items.AddRange(POIs.ProcessCsv(reservesCsv));
            log.LogInformation($"POIs - Line Count = {rspbReserves.Items.Count}");
            return rspbReserves;
        }

        /// <summary>
        /// Gets the Requested POI file from the POIGraves site
        /// </summary>
        /// <param name="log">Logger Instance</param>
        /// <param name="category">Category to be loaded</param>
        /// <returns>Populated Category</returns>
        private static PointOfInterestCategory GetFromPoiGraves(ILogger log, CategoryEnum category)
        {
            log.LogInformation($"POIs - Category = {category}");
            PointOfInterestCategory currentCategory = new PointOfInterestCategory((int)category, category.ToDescriptionString(), null);
            byte[] categoryZip = POIs.Download(category);
            log.LogInformation($"POIs - Zip Size = {categoryZip.Length}");
            string categoryCsv = POIs.Unpack(categoryZip);
            log.LogInformation($"POIs - CSV Size = {categoryCsv.Length}");
            currentCategory.Items.AddRange(POIs.ProcessCsv(categoryCsv));
            log.LogInformation($"POIs - Line Count = {currentCategory.Items.Count}");
            return currentCategory;
        }

        /// <summary>
        /// Process the CSV file
        /// </summary>
        /// <param name="csvFileContents">File Contents</param>
        /// <returns>Collection of Pois</returns>
        private static IEnumerable<PointOfInterest> ProcessCsv(string csvFileContents)
        {
            Collection<PointOfInterest> entries = new Collection<PointOfInterest>();

            string[] csvLines = csvFileContents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string csvLine in csvLines)
            {
                string[] csvEntry = csvLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                PointOfInterest newPoi = new PointOfInterest()
                {
                    Longitude = double.Parse(csvEntry[0]),
                    Latitude = double.Parse(csvEntry[1]),
                    Name = csvEntry[2].Replace("\"", string.Empty)
                };

                if (csvEntry.Length > 3)
                {
                    newPoi.Phone = csvEntry[3];
                }

                entries.Add(newPoi);
            }

            return entries;
        }

        /// <summary>
        /// Unpack the POI file
        /// </summary>
        /// <param name="zipFile">Zip File to unpack</param>
        /// <returns>Contents of the SDV file</returns>
        private static string Unpack(byte[] zipFile)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(zipFile))
                {
                    using (ZipArchive archive = new ZipArchive(ms))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                            {
                                StreamReader streamReader = new StreamReader(entry.Open());
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //// TODO report the error?
            }

            return string.Empty;
        }

        /// <summary>
        /// Downloads the relevant files from the server
        /// </summary>
        /// <param name="category">POI Category</param>
        /// <returns>Downloaded Data</returns>
        private static byte[] Download(CategoryEnum category)
        {
            string code = string.Empty;
            switch (category)
            {
                case CategoryEnum.NationalTrust:
                    code = "nt";
                    break;
                case CategoryEnum.EnglishHeritage:
                    code = "eh";
                    break;
                case CategoryEnum.HistoricHouses:
                    code = "hh";
                    break;
                case CategoryEnum.HistoricScotland:
                    code = "hs";
                    break;
                case CategoryEnum.NationalTrustScotland:
                    code = "ns";
                    break;
            }

            WebClient client = new WebClient();
            client.Headers.Add("Referer", $"https://www.poigraves.uk/pages/page{code}.php");
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.DownloadData($"https://www.poigraves.uk/downloads/{code}garcsvdl.php");
        }
    }
}
