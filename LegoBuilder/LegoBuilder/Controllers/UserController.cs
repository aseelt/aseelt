using LegoBuilder.Exceptions;
using LegoBuilder.Models;
using LegoBuilder.Security;
using LegoBuilder.SqlDaos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LegoBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Aseel")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class UserController : ControllerBase
    {
        private readonly IUserSqlDao userSqlDao;
        public UserController(IUserSqlDao userSqlDao)
        {
            this.userSqlDao = userSqlDao;
        }

        [HttpPut("Increase/{username}")]
        public ActionResult<bool> IncreaseRole(string username)
        {
            CheckString(username);
            try
            {
                if(User.Identity.Name == username)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                User currentRole = userSqlDao.GetUserByUsername(username);
                if (currentRole.Is_Active == false || currentRole.Role == userSqlDao.AdminRole)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }
                
                if (currentRole != null)
                {
                    userSqlDao.IncreaseUserRole(username);
                    return Ok($"{username}'s role has been increased");
                }
                else
                {
                    return NotFound("That entry was not found");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Decrease/{username}")]
        public ActionResult<bool> DecreaseRole(string username)
        {
            CheckString(username);
            try
            {
                if (User.Identity.Name == username)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                User currentRole = userSqlDao.GetUserByUsername(username);
                if (currentRole.Is_Active == false || currentRole.Role == userSqlDao.DefaultRole)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                if (currentRole != null)
                {
                    userSqlDao.DecreaseUserRole(username);
                    return Ok($"{username}'s role has been decreased");
                }
                else
                {
                    return NotFound("That entry was not found");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Deactivate/{username}")]
        public ActionResult<bool> DeactivateUser(string username)
        {
            CheckString(username);
            try
            {
                if (User.Identity.Name == username)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                User currentRole = userSqlDao.GetUserByUsername(username);
                if (currentRole.Is_Active == userSqlDao.Inactive)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                if (currentRole != null)
                {
                    userSqlDao.DeactivateUser(username);
                    return Ok($"{username} has been deactivated");
                }
                else
                {
                    return NotFound("That entry was not found");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Reactivate/{username}")]
        public ActionResult<bool> ReactivateUser(string username)
        {
            CheckString(username);
            try
            {
                if (User.Identity.Name == username)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                User currentRole = userSqlDao.GetUserByUsername(username);
                if (currentRole.Is_Active == userSqlDao.Active)
                {
                    throw new IncorrectEntryException("Please enter valid information");
                }

                if (currentRole != null)
                {
                    userSqlDao.ReactivateUser(username);
                    return Ok($"{username} has been reactivated");
                }
                else
                {
                    return NotFound("That entry was not found");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private ActionResult<bool> CheckString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return BadRequest("Please enter in a valid value");
            }
            return true;
        }
    }
}
