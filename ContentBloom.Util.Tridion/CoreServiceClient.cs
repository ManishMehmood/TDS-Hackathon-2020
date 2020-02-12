using System.Configuration;
using System.Net;
using Tridion.ContentManager.CoreService.Client;

namespace ContentBloom.Util.Tridion
{
    public class CoreServiceClient
    {
        private static SessionAwareCoreServiceClient _client;

        public static SessionAwareCoreServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    string endpointName = ConfigurationManager.AppSettings["CoreServiceEndpoint"];
                    if (string.IsNullOrEmpty(endpointName))
                    {
                        throw new ConfigurationErrorsException("CoreServiceEndpoint missing from appSettings");
                    }

                    _client = new SessionAwareCoreServiceClient(endpointName);

                    string username = ConfigurationManager.AppSettings["TridionUsername"];
                    string password = ConfigurationManager.AppSettings["TridionPassword"];


                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        _client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(username, password);
                    }
                }
                return _client;
            }
        }
    }
}
