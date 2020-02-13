using AlexaSkill.Handlers;
using AlexaSkill.Models;
using System.Web.Http;

namespace AlexaSkill.Controllers
{
    public class AlexaController : ApiController
    {
        [HttpPost, Route("api/alexa/demo")]
        public AlexaResponse AlexaSkills(AlexaRequest request)
        {
            AlexaResponse res;
            RequestHandler handler = new RequestHandler();
            switch (request.Request.Intent.Name)
            {
                case "PublishPageIntent":
                    res = handler.PublishPageIntentHandler(request.Request.Intent.Slots);
                    break;
                default:
                    res = handler.GetPageListHandler();
                    break;
            }
            return res;
        }
        [HttpPost, Route("api/alexa/getpublcations")]
        public AlexaResponse GetPublications(AlexaRequest request)
        {
            AlexaResponse res;
            RequestHandler handler = new RequestHandler();
               
            switch (request.Request.Intent.Name)
            {
                case "GetPublicationList":
                    res = handler.GetPublicationListHandler(request.Request.Intent.Slots);
                    break;
                default:
                    res = handler.GetPublicationListHandler(null);
                    break;
            }
            return res;
        }
    }
}
