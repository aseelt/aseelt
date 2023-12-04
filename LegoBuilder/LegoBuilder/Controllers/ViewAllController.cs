using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.Services;
using LegoBuilder.SqlDaos;
using LegoBuilder.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LegoBuilder.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin, Aseel")]
    // TODO remove before final
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ViewAllController : CommonController
    {
        // TODO interesting... the all functions don't return all the same details if you search by id
        // found it on part
        // TODO make another admin controller class that just refreshes the database
        // probably the user-sets one needs math doing update methods
        // probably need a method that checks "hey chcek this against the api"
        // tests will be important in case they change the api (especially the one that builds the object from the json)
        // at this time, can't just add pieces

        // TODO these read from the sql database, they don't need api service objects
        public ViewAllController(IRebrickableAPI rebrickableAPI, IAuthenticatedApiServiceById apiServiceById, IAuthenticatedApiServiceAll apiServiceAll,
         IUserSqlDao userSqlDao, IColourSqlDao colourSqlDao, IPartCatIdSqlDao partCatIdsSqlDao, IPartSqlDao partSqlDao, ISetSqlDao setSqlDao, 
         IThemeSqlDao themeSqlDao, ISetPartsSqlDao setPartsSqlDao, IUserSetsSqlDao userSetsSqlDao)
         : base(rebrickableAPI, apiServiceById, apiServiceAll, userSqlDao, colourSqlDao, partCatIdsSqlDao, partSqlDao, setSqlDao, themeSqlDao, setPartsSqlDao, userSetsSqlDao)
        {
        }

        [HttpGet("Colours/")]
        public ActionResult<AllColours> GetAllColours()
        {
            try
            {
                AllColours apiOutput;
                AllColours result;
                apiOutput = _ApiServiceAll.GetAllColours();
                result = _ColourSqlDao.UpdateDatabase(apiOutput);
                if (result != null)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Part_Categories/")]
        public ActionResult<AllPartCats> GetAllPartCategories()
        {
            try
            {
                AllPartCats apiOutput;
                AllPartCats result;
                apiOutput = _ApiServiceAll.GetAllPartCategories();
                result = _partCatIdsSqlDao.UpdateDatabase(apiOutput);
                if (result != null)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Parts/")]
        public ActionResult<AllParts> GetAllParts()
        {
            try
            {
                AllParts apiOutput;
                AllParts result;
                apiOutput = _ApiServiceAll.GetAllParts();
                result = _partSqlDao.UpdateDatabase(apiOutput);
                if (result.Count != 0)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Sets/")]
        public ActionResult<AllSets> GetAllSets()
        {
            // as you get the different sets, you'll have to go back and get the colours of the parts
            try
            {
                AllSets apiOutput;
                AllSets result;
                apiOutput = _ApiServiceAll.GetAllSets(); 
                result = _setSqlDao.UpdateDatabase(apiOutput);

                // if you are getting all the sets and that's causing an update on the database, 
                // then check if there are any adds
                SetPartsLoad();
                if (result.Count != 0)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Themes/")]
        public ActionResult<AllThemes> GetAllThemes()
        {
            try
            {
                AllThemes apiOutput;
                AllThemes result;
                apiOutput = _ApiServiceAll.GetAllThemes();
                result = _themeSqlDao.UpdateDatabase(apiOutput);
                if (result != null)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("AllSetInfo/{id}")]
        public ActionResult<FullSetInfo> GetFullSetInfo(string id)
        {
            try
            {
                FullSetInfo result;
                result = _setPartsSqlDao.GetFullSetInfo(id);
                if (result != null)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("WildcardSetSearch/{id}")]
        public ActionResult<List<Set>> WildcardSearchSets(string id)
        {
            try
            {
                List<Set> result;
                result = _setSqlDao.WildcardSearchSets(id);
                if (result != null)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("SetPartsLoad/")]
        public ActionResult<bool> SetPartsLoad()
        {
            try
            {
                // get all the sets I have
                List<Set> allDbSets = _setSqlDao.GetAllSets();

                // gets the persistent list of loaded sets, just in case the process breaks
                List<string> setsLoaded = AllSetPartsLoadingLog.GetLog();

                int counter = 1;

                // get all the sets in the database - passed in in allDbSets
                foreach (Set set in allDbSets)
                {
                    if (!setsLoaded.Contains(set.Set_Num))
                    {
                        SetParts apiProvidedSet = _ApiServiceAll.GetAllSetParts(set.Set_Num);

                        // if the results array is empty, don't add it to the sets parts database
                        if (apiProvidedSet.Results.Count != 0)
                        {
                            // then we need to add it to the database
                            _setPartsSqlDao.UpdateDatabase(apiProvidedSet);

                            // then update our list of setsloaded
                            AllSetPartsLoadingLog.WriteLog(set.Set_Num);

                            CreationUpdateLog.WriteLog("API Call", "AllSetPartsLoad", $"{set.Set_Num} was added to the database");

                            // then wait
                            Random rnd = new Random();
                            Thread.Sleep(rnd.Next(1500, 5000));
                        }
                        else
                        {
                            // still add it to the log so we don't have to keep dealing with it
                            AllSetPartsLoadingLog.WriteLog(set.Set_Num);
                        }
                    }
                    // for debug 
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        Debug.WriteLine($"Processed {counter} sets");
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet("UpdateInstructions/")]
        public ActionResult<bool> UpdateInstructions()
        {
            try
            {
                bool result = _setSqlDao.UpdateInstructions();
                if (result)
                {
                    return Ok(result);

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
