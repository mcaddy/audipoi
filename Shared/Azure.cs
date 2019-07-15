//-----------------------------------------------------------------------
// <copyright file="Azure.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System.Net;

    /// <summary>
    /// Static class with helper functions for azure
    /// </summary>
    public static class Azure
    {
        /// <summary>
        /// Invoke an Azure Function
        /// </summary>
        /// <param name="path">Function Path</param>
        /// <param name="apiKey">API Key</param>
        /// <returns>The output of the function</returns>
        public static string InvokeFunction(string path, string apiKey)
        {
            WebClient client = new WebClient();
            client.Headers.Add("x-functions-key", apiKey);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.DownloadString(path);
        }
    }
}
