//-----------------------------------------------------------------------
// <copyright file="LoggingClient.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy
{
    using System;
    using System.Net;

    /// <summary>
    /// Utility class to support logging to the cloud
    /// </summary>
    public class LoggingClient
    {
        /// <summary>
        /// Configuration Class
        /// </summary>
        private Config configuration = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingClient"/> class.
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        public LoggingClient(Config configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Log an entry to the Cloud
        /// </summary>
        /// <param name="username">Username to log record against</param>
        /// <param name="message">Message to store</param>
        public void Log(string username, string message)
        {
            if (this.configuration == null)
            {
                this.configuration = new Config();
            }

            try
            {
                string path = string.Format(this.configuration.Get("LogEventFunctionPath"), WebUtility.UrlEncode(username.Replace("\\", "_")), WebUtility.UrlEncode(message));

                Azure.InvokeFunction(path, string.Empty);
            }
            catch (Exception)
            {
                // Catch all errors and do nothing as we don't want to disrupt the user in the event of failed logging.
            }
        }
    }
}
