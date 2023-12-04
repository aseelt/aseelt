using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LegoBuilder.Models
{
    /// <summary>
    /// Model for Incoming. Used for registration
    /// </summary>
    public class NewUser
    {
        [MinLength(2, ErrorMessage = "Please create a unique username")]
        public string Username { get; set; }
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters")]
        public string Password { get; set; }
        [MinLength(2, ErrorMessage = "Please enter your first name, or an alias")]
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid eamail address")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Model to accept login parameters
    /// Incoming. Used for registration and signing back in
    /// </summary>
    public class LoginUser
    {
        [MinLength(2, ErrorMessage = "Please enter a username")]
        public string Username { get; set; }
        [MinLength(12, ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
    }

    /// <summary>
    /// Model to return upon successful login
    /// Goes back to the front end after registration
    /// </summary>
    public class ReturnUser
    {
        [Required]
        [JsonPropertyName("id")]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Token { get; set; }
    }

    /// <summary>
    /// Sent back to the front end with the token
    /// After login successful
    /// </summary>
    public class LoginResponseDto
    {
        public User user { get; set; }
        public string token { get; set; }
    }

    /// <summary>
    /// Used to store the details from the usersqldao
    /// for password matching
    /// </summary>
    public class User
    {
        [JsonIgnore]
        public int User_Id { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password_Hash { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }

        [JsonIgnore]
        public string First_Name { get; set; }
        [JsonIgnore]
        public string Last_Name { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public string Role { get; set; }
        [JsonIgnore]
        public bool Is_Active { get; set; }
        [JsonIgnore]
        public DateTime LB_Creation_Date { get; set; }
        [JsonIgnore]
        public DateTime LB_Update_Date { get; set; }
    }
    
    
    
}
