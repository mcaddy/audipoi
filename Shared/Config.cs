//-----------------------------------------------------------------------
// <copyright file="Config.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// Config Manger Class
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Dictionary of Key Values pairs 
        /// </summary>
        private Dictionary<string, string> configSettings = null;

        /// <summary>
        /// Load the config from the internet, if this fails default to the last config
        /// file downloaded or the one that shipped with the product
        /// </summary>
        public void Load()
        {
            // Get Location from App Settings
            string configLocation = Properties.Settings.Default.OnlineConfig;
            string configFile;
            
            // Try to load from the net to get latest settings
            try
            {
                WebClient client = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                configFile = client.DownloadString(configLocation);

                // Update the local config so that we can function offline or in the event of a github failure
                File.WriteAllText(Properties.Settings.Default.OfflineConfig, configFile);
            }
            catch (WebException)
            {
                // If failed try to use the local version
                configFile = File.ReadAllText(Properties.Settings.Default.OfflineConfig);
            }
            
            this.configSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(configFile);
        }

        /// <summary>
        /// Get a referenced configuration value
        /// </summary>
        /// <param name="key">Key to select</param>
        /// <returns>value of the requested key</returns>
        public string Get(string key)
        {
            if (this.configSettings == null)
            {
                this.Load();
            }

            this.configSettings.TryGetValue(key, out string value);

            return value;
        }
    }
}
