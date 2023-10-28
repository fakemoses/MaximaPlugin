using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Windows.Forms;

namespace MaximaPlugin.MInstaller
{
    /// <summary>
    /// Responsible to fetch JSON file from given URL
    /// </summary>
    class JsonDataFetcher
    {
        /// <summary>
        /// Fetch the latest URL for windows installer
        /// </summary>
        /// <param name="url"> URL for the JSON</param>
        /// <returns></returns>
        public async Task<string> GetWindowsReleaseUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                // define security protocol
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                try
                {
                    // request latest url
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();

                    // deserialize JSON file
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    string windowsUrl = data.platform_releases.windows.url;
                    return windowsUrl;
                } catch (Exception ex)
                {
                    
                }
                return null;
                
            }
        }
    }
}
