using AlexaSkill.Models;
using ContentBloom.Util.Tridion;

namespace AlexaSkill.Handlers
{
    public class RequestHandler
    {
        const string pageNotFound = "I haven't found any page in the search";
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
            result = string.IsNullOrEmpty(result) ? pageNotFound : result;
            var response = new AlexaResponse(string.Format("Here is the list of pages = {0} \n Which page you want to publish?", result), false);
            response.Response.ShouldEndSession = false;
            return response;
        }
    }
}