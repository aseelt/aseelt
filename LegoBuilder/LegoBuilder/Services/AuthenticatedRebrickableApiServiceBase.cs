using RestSharp;
using RestSharp.Authenticators;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.Services.Models;
using LegoBuilder.Utilities;

namespace LegoBuilder.Services
{
    public class AuthenticatedRebrickableApiServiceBase : IAuthenticatedApiServiceBase
    {
        public static IRestClient client = null;
        protected readonly IRebrickableAPI _ApiInfo;
        protected int PageSize = 1000;

        public AuthenticatedRebrickableApiServiceBase(IRebrickableAPI rebrickableAPI)
        {
            // got this dependency injection to work!!
            // because a medium article told me to do it this way
            _ApiInfo = rebrickableAPI;
            client = new RestClient(_ApiInfo.RebrickableApiUrl);
            // ask Tom why this header doesn't work - it magically works
            client.AddDefaultHeader("Authorization", "key c045ca46c6de290b657d3cb287fab9ac");
        }


        protected void CheckForError(IRestResponse response)
        {
            string message;
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                message = $"Error occurred - unable to reach server. Response status was '{response.ResponseStatus}'.";
                CreationUpdateLog.WriteLog("API Call", "", message);
                throw new HttpRequestException(message, response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                // Set an appropriate error message
                message = $"An http error occurred. Status code {(int)response.StatusCode} {response.StatusDescription}";

                // Throw an HttpRequestException with the appropriate message
                if ((int)response.StatusCode == 401)
                {
                    message = $"You are not authorized. Status code '{(int)response.StatusCode}, {response.StatusDescription}'.";
                    CreationUpdateLog.WriteLog("API Call", "", message);
                    throw new HttpRequestException(message, response.ErrorException);
                }
                else if ((int)response.StatusCode == 403)
                {
                    message = $"You do not have permission. Status code '{(int)response.StatusCode}, {response.StatusDescription}'.";
                    CreationUpdateLog.WriteLog("API Call", "", message);
                    throw new HttpRequestException(message, response.ErrorException);
                }
                else
                {
                    CreationUpdateLog.WriteLog("API Call", "", message);
                    throw new HttpRequestException(message, response.ErrorException);
                }
            }
        }
    }
}
