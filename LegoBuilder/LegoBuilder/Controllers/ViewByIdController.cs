using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.Services;
using LegoBuilder.SqlDaos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LegoBuilder.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ViewByIdController : CommonController
    {
        // TODO these read from the sql database, they don't need api service objects
        // TODO this should also probably be admin only

        // TODO change the location of this interface
        private ILegoUrlGetter legoUrlGetter;
        public ViewByIdController(IRebrickableAPI rebrickableAPI, IAuthenticatedApiServiceById apiServiceById, IAuthenticatedApiServiceAll apiServiceAll,
         IUserSqlDao userSqlDao, IColourSqlDao colourSqlDao, IPartCatIdSqlDao partCatIdsSqlDao, IPartSqlDao partSqlDao, ISetSqlDao setSqlDao, 
         IThemeSqlDao themeSqlDao, ISetPartsSqlDao setPartsSqlDao, IUserSetsSqlDao userSetsSqlDao, ILegoUrlGetter urlGetter)
         : base(rebrickableAPI, apiServiceById, apiServiceAll, userSqlDao, colourSqlDao, partCatIdsSqlDao, partSqlDao, setSqlDao, themeSqlDao, setPartsSqlDao, userSetsSqlDao)
        {
            this.legoUrlGetter = urlGetter;
        }

        [HttpGet("Colour/{id}")]
        public ActionResult<Colour> GetColourById(string id)
        {
            CheckString(id);
            try
            {
                Colour apiOutput;
                apiOutput = _ApiServiceById.GetColourById(id);
                apiOutput = _ColourSqlDao.GetColourById(id);
                if (apiOutput != null)
                {
                    return Ok(apiOutput);

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
        [HttpGet("Part_Category/{id}")]
        public ActionResult<PartCatId> GetPartCategoriesById(int id)
        {
            try
            {
                PartCatId output;
                output = _ApiServiceById.GetPartCategoriesById(id);

                if (output != null)
                {
                    return Ok(output);

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

        [HttpGet("Theme/{id}")]
        public ActionResult<Theme> GetThemeById(string id)
        {
            CheckString(id);

            try
            {
                Theme output;
                output = _ApiServiceById.GetThemeById(id);
                if (output != null)
                {
                    return Ok(output);

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

        [HttpGet("Part/{partnumber}")]
        public ActionResult<Part> GetPartById(string partnumber)
        {
            CheckString(partnumber);

            try
            {
                Part output;
                output = _ApiServiceById.GetPartById(partnumber);
                if (output != null)
                {
                    return Ok(output);

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

        [HttpGet("Set/{setNumber}")]
        public ActionResult<Set> GetSetById(string setNumber)
        {
            CheckString(setNumber); 

            try
            {
                Set output;
                output = _ApiServiceById.GetSetById(setNumber);

                // if you're getting the set
                // might as well check to see if it needs updating
                AllSets toUpdate = new AllSets();
                toUpdate.Results.Add(output);
                _setSqlDao.UpdateDatabase(toUpdate);

                // then if you're checking the set and if it needs updating
                // might as well check on the parts too
                GetSetPartsById(setNumber);
                if (output != null)
                {
                    return Ok(output);

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

        [HttpGet("Set_Parts/{setnumber}")]
        public ActionResult<SetParts> GetSetPartsById(string setNumber)
        {
            CheckString(setNumber);
            try
            {
                SetParts apiOutput;
                SetParts result;
                apiOutput = _ApiServiceById.GetSetPartsById(setNumber);
                result = _setPartsSqlDao.GetSetPartsBySetNum(setNumber);
                //result = _setPartsSqlDao.UpdateDatabase(apiOutput);
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

        [HttpGet("Get_Random_Sets_Parts/")]
        public ActionResult<FullSetInfo> GetRandomSetsParts()
        {
            try
            {
                FullSetInfo result;
                string setNumber = null;
                setNumber = _setPartsSqlDao.GetRandomSetNumber();
                result = _setPartsSqlDao.GetFullSetInfo(setNumber);
                //result = _setPartsSqlDao.UpdateDatabase(apiOutput);
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

        [HttpGet("UserSets/{username}")]
        public ActionResult<UserBase> GetUserSetsByUsername(string username)
        {
            CheckString(username);
            try
            {
                UserFullInfo result0 = _userSetsSqlDao.GetFullInfoByUsername(username);
                List<UserSetInfo> result1 = _userSetsSqlDao.GetSetOnlyInfoByUsername(username);
                List<UserPartColourInfo> result2 = _userSetsSqlDao.GetPartAndColourInfoByUsername(username);
                List<UserPartInfo> result3 = _userSetsSqlDao.GetPartOnlyInfoByUsername(username);
                List<UserColourInfo> result4 = _userSetsSqlDao.GetColourOnlyInfoByUsername(username);
                List<UserPartCatInfo> result5 = _userSetsSqlDao.GetPartCategoriesInfoByUsername(username);
                List<UserThemeInfo> result6 = _userSetsSqlDao.GetThemesInfoByUsername(username);
                UserTotalPartInfo result7 = _userSetsSqlDao.GetTotalPartInfoByUsername(username);
                if (result0 != null)
                {
                    return Ok(result1);

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

        [HttpGet("AddUserSets/{username}")]
        public ActionResult<UsersSets> AddSetToUser(string username, string setNumber, int quantity)
        {
            CheckString(username);
            try
            {
                UsersSets result = _userSetsSqlDao.AddNewSet(username, setNumber, quantity);
                if (result != null)
                {
                    return Created("/", result);

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
        
        [HttpGet("ChangeUserSetsQuantity/{username}")]
        public ActionResult<string> ChangeSetQuantity(string username, string setNumber, int quantity)
        {
            CheckString(username);
            try
            {
                string result = _userSetsSqlDao.ChangeSetQuantity(username, setNumber, quantity);
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
        [HttpGet("WildcardBrokenUpSetsPartsSearch/")]
        public ActionResult<List<FullSetInfo>> WildcardSearchByFieldSetParts(string sets, string parts, string colours)
        {
            CheckString(sets);
            CheckString(parts);
            CheckString(colours);
            try
            {
                List<FullSetInfo> result = _setPartsSqlDao.WildcardSearchByFieldSetParts(sets, parts, colours);
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
        [HttpGet("WildcardUsernameUserSetSearch/")]
        public ActionResult<UserFullInfo> WildcardSearchAllFieldsUsersSets(string username, string query)
        {
            CheckString(username);
            CheckString(query);
            try
            {
                UserFullInfo result = _userSetsSqlDao.WildcardSearchAllFieldsUsersSets(username, query);
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
        [HttpGet("CheckLegoUrl/{setNumber}")]
        public ActionResult<int> GetLegoUrl(string setNumber)
        {
            CheckString(setNumber);
            try
            {
                int getCode = legoUrlGetter.UrlChecker(setNumber).Result;

                if (getCode != null)
                {
                    return Ok(getCode);

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
