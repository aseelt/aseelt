using LegoBuilder.Models;
using LegoBuilder.Models.All;
using LegoBuilder.Services;
using LegoBuilder.SqlDaos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;


namespace LegoBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CommonController : ControllerBase
    {
        // TODO common controller has the sql daos, not the api service
        // only the fill controller should have the api stuff, that's the only one getting the info from them
        private readonly IRebrickableAPI _ApiInfo;
        protected readonly IAuthenticatedApiServiceById _ApiServiceById;
        protected readonly IAuthenticatedApiServiceAll _ApiServiceAll;
        protected readonly IUserSqlDao _UserSqlDao;
        protected readonly IColourSqlDao _ColourSqlDao;
        protected readonly IPartCatIdSqlDao _partCatIdsSqlDao;
        protected readonly IPartSqlDao _partSqlDao;
        protected readonly ISetSqlDao _setSqlDao;
        protected readonly IThemeSqlDao _themeSqlDao;
        protected readonly ISetPartsSqlDao _setPartsSqlDao;
        protected readonly IUserSetsSqlDao _userSetsSqlDao;

        // common controller needs the sql daos
        // because view all and by id need the sql daos
        // but they don't need the api service
        // only the fill controller needs the api service
        // so make the parent not have the api service, put that in the lower level ones
        public CommonController(IRebrickableAPI rebrickableAPI, IAuthenticatedApiServiceById apiServiceById, IAuthenticatedApiServiceAll apiServiceAll, 
            IUserSqlDao userSqlDao, IColourSqlDao colourSqlDao, IPartCatIdSqlDao partCatIdsSqlDao, IPartSqlDao partSqlDao, ISetSqlDao setSqlDao, 
            IThemeSqlDao themeSqlDao, ISetPartsSqlDao setPartsSqlDao, IUserSetsSqlDao userSetsSqlDao)
        {
            _ApiInfo = rebrickableAPI;
            _ApiServiceById = apiServiceById;
            _ApiServiceAll = apiServiceAll;
            _UserSqlDao = userSqlDao;
            _ColourSqlDao = colourSqlDao;
            _partCatIdsSqlDao = partCatIdsSqlDao;
            _partSqlDao = partSqlDao;
            _setSqlDao = setSqlDao;
            _themeSqlDao = themeSqlDao;
            _setPartsSqlDao = setPartsSqlDao;
            _userSetsSqlDao = userSetsSqlDao;
        }
        protected ActionResult<bool> CheckString(string input, string message = "")
        {
            if (string.IsNullOrEmpty(input))
            {
                return BadRequest((message == "" ? "Please enter in a valid value" : message));
            }
            return true;
        }
    }
}
