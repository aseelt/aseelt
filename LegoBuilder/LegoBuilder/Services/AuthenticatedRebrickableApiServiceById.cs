using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.Utilities;
using RestSharp;
using System.Diagnostics;
using System.Threading;

namespace LegoBuilder.Services
{
    public class AuthenticatedRebrickableApiServiceById : AuthenticatedRebrickableApiServiceBase, IAuthenticatedApiServiceById
    {
        public AuthenticatedRebrickableApiServiceById(IRebrickableAPI rebrickableAPI) : base(rebrickableAPI)
        {
        }

        public Colour GetColourById(string id)
        {
            RestRequest request = new RestRequest($"colors/{id}/"/*$"colors/{id}/?key={base._ApiInfo.ApiKey}"*/);
            IRestResponse<Colour> response = client.Get<Colour>(request);
            CheckForError(response);
            return response.Data;
        }

        public PartCatId GetPartCategoriesById(int id)
        {
            RestRequest request = new RestRequest($"part_categories/{id}/");
            IRestResponse<PartCatId> response = client.Get<PartCatId>(request);
            CheckForError(response);
            return response.Data;
        }

        public Part GetPartById(string id)
        {
            RestRequest request = new RestRequest($"parts/{id}/");
            IRestResponse<Part> response = client.Get<Part>(request);
            CheckForError(response);
            return response.Data;
        }


        public Set GetSetById(string id)
        {
            RestRequest request = new RestRequest($"sets/{id}/");
            IRestResponse<Set> response = client.Get<Set>(request);
            CheckForError(response);
            return response.Data;
        }

        public SetParts GetSetPartsById(string id)
        {
            bool keepGoing = true;
            SetParts output = new SetParts();

            for (int i = 1; /*i < 11*/ keepGoing; i++)
            {
                // make the request
                RestRequest request = new RestRequest($"sets/{id}/parts/?page{i}&page_size={base.PageSize}");
                IRestResponse<SetParts> response = client.Get<SetParts>(request);
                CheckForError(response);

                // store the request
                SetParts returnedSetParts = response.Data;

                // iterate through the returned results and add to the overall output
                foreach (ResultsSetParts setPart in returnedSetParts.Results)
                {
                    output.Results.Add(setPart);
                }

                // if the list is less than the page size I want (currently 1000), i know I'm at the end
                if (returnedSetParts.Results.Count < base.PageSize)
                {
                    keepGoing = false;
                }
                Debug.WriteLine($"Total count pulled from API is {output.Results.Count} parts for set number {returnedSetParts.Set_Num}");
                // add a delay because the API needs 1 second between requests
                Thread.Sleep(1500);
            }
            CreationUpdateLog.WriteLog("API Call", "Set Parts", $"{output.Results.Count} parts for set number {output.Set_Num} were retrieved");
            return output; 
        }

        public Theme GetThemeById(string id)
        {
            RestRequest request = new RestRequest($"themes/{id}/");
            IRestResponse<Theme> response = client.Get<Theme>(request);
            CheckForError(response);
            return response.Data;
        }
    }
}
