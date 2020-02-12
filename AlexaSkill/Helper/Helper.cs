using System;
using System.Collections.Generic;
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
                List<string> ids = GetAllPages().Where(x => x != null && x.Title.ToLower().Equals(pageTitle.ToLower())).Select(x => x.Id).ToList();
                if (ids.Count == 1)
                {
                    string[] pageUris = { ids[0] };
                    string destinationTarget = ConfigurationManager.AppSettings["PublishTargetIdStaging"];
                    string[] destinationTargetUris = { destinationTarget };
                    var publishInstruction = new PublishInstructionData
                    {
                        RenderInstruction = new RenderInstructionData(),
                        ResolveInstruction = new ResolveInstructionData()
                    };
                    PublishTransactionData[] publishTransactions = Client.Publish(pageUris, publishInstruction,
                                                                                    destinationTargetUris, PublishPriority.Normal,
                                                                                    new ReadOptions());
                    Console.WriteLine("Published page; transaction id: " + publishTransactions[0].Id);
                    // return pageTitle;
                    return string.Format("The page {0} has been sent to the publishing queue", pageTitle);
                }
                else
                {
                    ids = GetAllPages().Where(x => x != null && x.Title.ToLower().Contains(pageTitle.ToLower())).Select(x => x.Id).ToList();
                    if (ids.Count == 0)
                    {
                        return "Alexa doesn't found any matching page in CMS";
                    }
                    var pages = GetAllPages().Where(x => x != null && x.Title.ToLower().Contains(pageTitle.ToLower())).Select(x => x.Title).ToList();
                    return string.Format("Alexa found {0} pages. Which one you want to publish?", string.Join(",", pages));
                }
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
            string rootSGId = ConfigurationManager.AppSettings["RootStructureGroupId"];
            return Client.GetList(
                rootSGId,
                new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Page },
                    Recursive = false,
                });
        }
    }
}


