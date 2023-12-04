using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.SqlDaos;
using LegoBuilder.Utilities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LegoBuilder.Services
{
    public class AuthenticatedRebrickableApiServiceAll : AuthenticatedRebrickableApiServiceBase, IAuthenticatedApiServiceAll
    {
        //this class gets all the items, we won't use the parameters
        public AuthenticatedRebrickableApiServiceAll(IRebrickableAPI rebrickableAPI) : base(rebrickableAPI)
        {
        }

        public AllColours GetAllColours()
        {
            RestRequest request = new RestRequest($"colors/?page_size={base.PageSize}"/*$"colors/?key={base._ApiInfo.ApiKey}&page_size={base.PageSize}"*/);
            IRestResponse<AllColours> response = client.Get<AllColours>(request);
            CreationUpdateLog.WriteLog("API Call", "AllColours", $"{response.Data.Count} records were retrieved");
            CheckForError(response);
            return response.Data;
        }

        public AllPartCats GetAllPartCategories()
        {
            RestRequest request = new RestRequest($"part_categories/?page_size={base.PageSize}");
            IRestResponse<AllPartCats> response = client.Get<AllPartCats>(request);
            CreationUpdateLog.WriteLog("API Call", "AllPartCats", $"{response.Data.Count} records were retrieved");
            CheckForError(response);
            return response.Data;
        }

        public AllParts GetAllParts()
        {
            // start off at page 1 in the for loop
            // keep going for the for loop
            // need it this way as I need the counter for the page
            // holder for the output
            bool keepGoing = true;
            AllParts output = new AllParts();

            for (int i = 1; /*i < 11*/ keepGoing; i++)
            {
                // make the request
                RestRequest request = new RestRequest($"parts/?page={i}&page_size={base.PageSize}");
                IRestResponse<AllParts> response = client.Get<AllParts>(request);
                CheckForError(response);

                // store the request
                AllParts returnedParts = response.Data;

                // iterate through the returned results and add to the overall output
                foreach (Part part in returnedParts.Results)
                {
                    output.Results.Add(part);
                }

                // if the list is less than the page size I want (currently 1000), i know I'm at the end
                if (returnedParts.Results.Count < base.PageSize)
                {
                    keepGoing = false;
                }
                Debug.WriteLine($"Total count pulled from API is {output.Results.Count}");
                // add a delay because the API needs 1 second between requests
                Thread.Sleep(5000);
            }
            CreationUpdateLog.WriteLog("API Call", "AllParts", $"{output.Results.Count} records were retrieved");
            return output;
        }

        public AllSets GetAllSets()
        {
            bool keepGoing = true;
            AllSets output = new AllSets();

            for (int i = 1; /*i < 11*/ keepGoing; i++)
            {
                RestRequest request = new RestRequest($"sets/?page={i}&page_size={base.PageSize}");
                IRestResponse<AllSets> response = client.Get<AllSets>(request);
                CheckForError(response);

                AllSets returnedSets = response.Data;

                foreach (Set set in returnedSets.Results)
                {
                    output.Results.Add(set);
                }

                if (returnedSets.Results.Count < base.PageSize)
                {
                    keepGoing = false;
                }
                Debug.WriteLine($"Total count pulled from API is {output.Results.Count}");
                Thread.Sleep(5000);
            }
            CreationUpdateLog.WriteLog("API Call", "AllSets", $"{output.Results.Count} records were retrieved");
            return output;

        }

        public AllThemes GetAllThemes()
        {
            RestRequest request = new RestRequest($"themes/?page_size={base.PageSize}");
            IRestResponse<AllThemes> response = client.Get<AllThemes>(request);
            CreationUpdateLog.WriteLog("API Call", "AllThemes", $"{response.Data.Count} records were retrieved");
            CheckForError(response);
            return response.Data;
        }

        public SetParts GetAllSetParts(string setNum)
        {
            bool keepGoing = true;
            SetParts output = new SetParts();

            for (int i = 1; /*i < 11*/ keepGoing; i++)
            {
                RestRequest request = new RestRequest($"sets/{setNum}/parts/?page={i}&page_size={base.PageSize}");
                IRestResponse<SetParts> response = client.Get<SetParts>(request);
                CheckForError(response);

                SetParts returnedSetParts = response.Data;

                foreach (ResultsSetParts setPart in returnedSetParts.Results)
                {
                    output.Results.Add(setPart);
                }

                if (returnedSetParts.Results.Count < base.PageSize)
                {
                    keepGoing = false;
                }
                Debug.WriteLine($"Total count pulled from API is {output.Results.Count}");
                Thread.Sleep(5000);
            }
            CreationUpdateLog.WriteLog("API Call", "SetParts", $"{output.Results.Count} records were retrieved");
            return output;

        }
    }
}
