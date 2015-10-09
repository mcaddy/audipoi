//-----------------------------------------------------------------------
// <copyright file="CookieAwareWebClient.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace System.Net
{
    /// <summary>
    /// A Cookie aware web client
    /// </summary>
    public class CookieAwareWebClient : WebClient
    {
        /// <summary>
        /// The Cookies for this session
        /// </summary>
        private CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// Get web request whilst retaining cookies
        /// </summary>
        /// <param name="address">Address to fetch</param>
        /// <returns>the Web request</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.cookieContainer;
            }

            return request;
        }
    }
}