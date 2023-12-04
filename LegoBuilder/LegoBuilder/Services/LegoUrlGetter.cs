using LegoBuilder.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LegoBuilder.Services
{
    public class LegoUrlGetter : ILegoUrlGetter
    {
        public string Lego_Url { get; set; }
        // from stack overflow
        // this prevents a redirect
        HttpClient Client = new HttpClient( new HttpClientHandler { AllowAutoRedirect = false });
        public LegoUrlGetter(string url)
        {
            this.Lego_Url = url;
        }
        public async Task<int> UrlChecker(string truncatedSetNumber)
        {
            if (string.IsNullOrEmpty(truncatedSetNumber))
            {
                throw new IncorrectEntryException("Please enter a valid input");
            }

            try
            {
                // stack overflow had a class that was deprecated and replaced with HttpResponse message
                // found out how to do this myself!
                // http response message, because that's what we're getting back
                // use .GetAsync and the url - this sends the request out to a website
                // .GetAsync uses await, so must use that in my code
                // then have to make the method's signature async to get it to work
                // then I get a task object back (bool), so need to write Task<bool>
                // it's an object, not a simple bool, so when calling this method and I want the result, I have to use .result on the object
                HttpResponseMessage response = await Client.GetAsync(Lego_Url + truncatedSetNumber);

                // redirect codes are 301, moved permanently, 302, redirect, 307, temporary redirect
                // want to check for response.Headers.Location for these

                // TODO ask Tom why I can't get a 301
                return (int)response.StatusCode;
            }
            catch (Exception e)
            {
                throw new IncorrectEntryException("This input caused an error");
            }
        }
    }
}
