using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using Tridion.ContentManager.CoreService.Client;
namespace AlexaSkill.Helper
{
    public static class Helper
    {
        private static SessionAwareCoreServiceClient _client;

        public static SessionAwareCoreServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    string endpointName = ConfigurationManager.AppSettings["CoreServiceEndpoint"];
                    if (String.IsNullOrEmpty(endpointName))
                    {
                        throw new ConfigurationErrorsException("CoreServiceEndpoint missing from appSettings");
                    }

                    _client = new SessionAwareCoreServiceClient(endpointName);

                    string username = ConfigurationManager.AppSettings["TridionUsername"];
                    string password = ConfigurationManager.AppSettings["TridionPassword"];

                    if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                    {
                        _client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(username, password);
                    }
                }
                return _client;
            }
        }
       
        public static string PublishPage(string pageTitle)
        {
            try
            {
          //   string id=GetAllPages().Any(x=>x.Id).Where(x=>x.Title==pageTitle)
             //   PageData page = (PageData)Client.Read(id, new ReadOptions());
             string id = GetAllPages().Where(x => x != null && x.Title.ToLower().Contains(pageTitle.ToLower())).Select(x=>x.Id).ToList()[0];
              //  Console.WriteLine("Page title: " + page.Title);

                // Publish the page
                string[] pageUris = { id };
                string[] destinationTargetUris = { "tcm:0-11-65538" };
                var publishInstruction = new PublishInstructionData
                {
                    RenderInstruction = new RenderInstructionData(),
                    ResolveInstruction = new ResolveInstructionData()
                };
                PublishTransactionData[] publishTransactions = Client.Publish(pageUris, publishInstruction,
                                                                                destinationTargetUris, PublishPriority.Normal,
                                                                                new ReadOptions());
                Console.WriteLine("Published page; transaction id: " + publishTransactions[0].Id);
                return pageTitle;
            }
            catch (FaultException<CoreServiceFault> e)
            {
                // handle the exception
                Console.WriteLine(String.Format("Error; Core Service said: {0}; {1} ", e.Detail.ErrorCode,
                                                string.Join(", ", e.Detail.Messages)));
                return String.Format("Error; Core Service said: {0}; {1} ", e.Detail.ErrorCode,
                                                string.Join(", ", e.Detail.Messages));
            }
        }
        public static IdentifiableObjectData[] GetAllPages()
        {
           // IdentifiableObjectData[] result = null;
            return Client.GetList(
                "tcm:24-561-4",
                new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] {ItemType.Page },
                    Recursive = false,
                });
          //  if (tridionItems == null)
           //     return result;
            //foreach(var page in tridionItems)
            //{
            //    result = string.Join(page.Title,",");
            //}
           //  result = string.Join(",", tridionItems.Select(x => x.Title).ToArray());
         //   return result;
        }
    }
}


