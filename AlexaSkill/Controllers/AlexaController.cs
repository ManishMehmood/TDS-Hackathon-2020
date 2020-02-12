using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AlexaSkill.Helper;
using AlexaSkill.Models;
using Tridion.ContentManager.CoreService.Client;

namespace AlexaSkill.Controllers
{
    public class AlexaController : ApiController
    {
        private const string pageNotFound = "I haven't found any page in the search";

        [HttpPost, Route("api/alexa/demo")]
        public AlexaResponse HelloWorld(AlexaRequest request)
        {
            AlexaResponse res = null;
            switch (request.Request.Intent.Name)
            {
                case "PublishPageIntent":
                    res = PublishPageIntentHandler(request.Request.Intent.Slots);
                    break;
                case "HelloWorldIntent":
                    res = PublishPageIntentHandler(request.Request.Intent.Slots);
                    break;
                default:
                    // res = new AlexaResponse("Please enter the ID of the page to be published",false);
                    res = GetPageListHandler();
                    break;
            }
            //    res = PublishPageIntentHandler("010 Federal Student Loans");
            return res;
        }
        private AlexaResponse PublishPageIntentHandler(dynamic slots)
        {
            string output = Helper.Helper.PublishPage(string.Format("{0}", slots["PageID"].value));
            //  string output = Helper.Helper.PublishPage("010 Federal Student Loans");
            var response = new AlexaResponse(output);
            response.Response.Reprompt.OutputSpeech.Text =
                "You can tell me to publish the page following the page ID, or cancel to exit.";
            response.Response.ShouldEndSession = false;
            return response;
        }

        private AlexaResponse GetPageListHandler()
        {
            var output = Helper.Helper.GetAllPages();
            string result = pageNotFound;
            if (output != null)
            {
                result = string.Join(",", output.Select(x => x.Title).ToArray());
            }
            var response = new AlexaResponse(string.Format("Here is the list of pages {0} \n Which page you want to publish?", result), false);
            response.Response.ShouldEndSession = false;
            return response;
        }
    }
}
