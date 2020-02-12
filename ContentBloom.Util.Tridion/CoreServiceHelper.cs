using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Tridion.ContentManager.CoreService.Client;

namespace ContentBloom.Util.Tridion
{
    public class CoreServiceHelper
    {
        public static string PublishPage(string pageTitle)
        {
            try
            {
                string id = GetAllPages().Where(x => x != null && x.Title.ToLower().Contains(pageTitle.ToLower())).Select(x => x.Id).ToList()[0];
                // Publish the page
                string[] pageUris = { id };
                string destinationTarget = ConfigurationManager.AppSettings["PublishTargetIdStaging"];
                string[] destinationTargetUris = { destinationTarget };
                var publishInstruction = new PublishInstructionData
                {
                    RenderInstruction = new RenderInstructionData(),
                    ResolveInstruction = new ResolveInstructionData()
                };
                PublishTransactionData[] publishTransactions = CoreServiceClient.Client.Publish(pageUris, publishInstruction,
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
            string rootSGId = ConfigurationManager.AppSettings["RootStructureGroupId"];
            return CoreServiceClient.Client.GetList(
                rootSGId,
                new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Page },
                    Recursive = false,
                });
        }
        public static string GetPages()
        {
            string result = "";
            string rootSGId = ConfigurationManager.AppSettings["RootStructureGroupId"];
            var output = CoreServiceClient.Client.GetList(
                rootSGId,
                new OrganizationalItemItemsFilterData
                {
                    ItemTypes = new[] { ItemType.Page },
                    Recursive = false,
                });
            if (output != null)
            {
                result = string.Join(",", output.Select(x => x.Title).ToArray());
            }
            return result;
        }
    }
}
