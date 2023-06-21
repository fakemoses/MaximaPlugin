using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MaximaPlugin.MInstaller
{
    class JsonDataFetcher
    {
        public async Task<string> GetWindowsReleaseUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                string windowsUrl = data.platform_releases.windows.url;

                return windowsUrl;
            }
        }
    }
}
