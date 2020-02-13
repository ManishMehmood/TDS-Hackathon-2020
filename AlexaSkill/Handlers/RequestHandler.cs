using AlexaSkill.Models;
using ContentBloom.Util.Tridion;

namespace AlexaSkill.Handlers
{
    public class RequestHandler
    {
        const string pageNotFound = "I haven't found any page in the search";
        const string publicationNotFound = "I haven't found any publication in the search";
        /// <summary>
        /// Handle publishing request
        /// </summary>
        /// <param name="slots">Intent slots send by Alexa</param>
        /// <returns>Alexa Response</returns>
        public AlexaResponse PublishPageIntentHandler(dynamic slots)
        {
            string output = CoreServiceHelper.PublishPage(string.Format("{0}", slots["PageID"].value));
            var response = new AlexaResponse(output);
            response.Response.Reprompt.OutputSpeech.Text =
                "You can tell me to publish the page following the page ID, or cancel to exit.";
            response.Response.ShouldEndSession = false;
            return response;
        }
        /// <summary>
        /// Handle getting page list
        /// </summary>
        /// <returns>Alexa Response</returns>
        public AlexaResponse GetPageListHandler()
        {
            string result = CoreServiceHelper.GetPages();
            result = string.IsNullOrEmpty(result) ? pageNotFound : string.Format("Here is the list of pages {0} \n Which page you want to publish?", result);
            var response = new AlexaResponse(result, false);
            response.Response.ShouldEndSession = false;
            return response;
        }

        /// <summary>
        /// Handle getting publication list
        /// </summary>
        /// <param name="slots">Intent slots send by Alexa</param>
        /// <returns>Alexa Response</returns>
        public AlexaResponse GetPublicationListHandler(dynamic slots)
        {
            string result = CoreServiceHelper.GetPublications(slots);
            result = string.IsNullOrEmpty(result) ? publicationNotFound : string.Format("Here is the list of publications {0}",result);
            var response = new AlexaResponse(result, false);
            response.Response.ShouldEndSession = false;
            return response;
        }
    }
}