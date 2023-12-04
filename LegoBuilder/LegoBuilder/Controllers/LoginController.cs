using LegoBuilder.Exceptions;
using LegoBuilder.Models;
using LegoBuilder.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoBuilder.Exceptions;
using LegoBuilder.SqlDaos;
using System;

namespace LegoBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserSqlDao userSqlDao;

        public LoginController(ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IUserSqlDao userSqlDao)
        {
            this.tokenGenerator = tokenGenerator;
            this.passwordHasher = passwordHasher;
            this.userSqlDao = userSqlDao;
        }

        [HttpPost("/register")]
        public IActionResult Register(NewUser incomingUserParam)
        {
            // Default generic error message
            const string ErrorMessage = "An error occurred and user was not created.";

            IActionResult result = BadRequest(new { message = ErrorMessage });

            // is username already taken?
            try
            {
                User existingUser = userSqlDao.GetUserByUsername(incomingUserParam.Username);
                if (existingUser != null)
                {
                    return Conflict(new { message = "Username already taken. Please choose a different username." });
                }
            }
            catch (DaoException)
            {
                return StatusCode(500, ErrorMessage);
            }

            // create new user
            User newUser;
            try
            {
                newUser = userSqlDao.CreateUser(incomingUserParam);
            }
            catch (IncorrectEntryException iee)
            {
                return BadRequest("You need a username or password to register.");
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorMessage);
            }

            if (newUser != null)
            {
                // Create a ReturnUser object to return to the client
                ReturnUser returnUser = new ReturnUser() { UserId = newUser.User_Id, Username = newUser.Username };

                result = Created("/login", returnUser);
            }

            return result;
        }

        [HttpPost("/login")]
        public IActionResult Authenticate(LoginUser loginUserParam)
        {
            // Default to bad username/password message
            IActionResult result = Unauthorized(new { message = "Username or password is incorrect." });

            User user;
            // Get the user by username
            try
            {
                user = userSqlDao.GetUserByUsername(loginUserParam.Username);
            }
            catch (Exception)
            {
                // return default Unauthorized message instead of indicating a specific error
                return result;
            }

            // If we found a user and the password hash matches
            if (user != null && passwordHasher.VerifyHashMatch(user.Password_Hash, loginUserParam.Password, user.Salt))
            {
                // Create an authentication token
                string token = tokenGenerator.GenerateToken(user.User_Id, user.Username, user.Role);

                // Create a ReturnUser object to return to the client
                LoginResponseDto retUser = new LoginResponseDto() { user = user, token = token };

                // Switch to 200 OK
                result = Ok(retUser);
            }

            return result;
        }
    }
}
