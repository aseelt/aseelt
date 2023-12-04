using LegoBuilder.Exceptions;
using LegoBuilder.Models;
using LegoBuilder.Security;
using LegoBuilder.Security.Models;
using System;
using System.Data.SqlClient;

namespace LegoBuilder.SqlDaos
{
    public class UserSqlDao : BaseSqlDao, IUserSqlDao
    {
        public UserSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        } 

        public User CreateUser(NewUser incomingUserParam)
        {
            CheckString(incomingUserParam.Username, "Please enter all required fields to register");
            CheckString(incomingUserParam.Password, "Please enter all required fields to register");
            CheckString(incomingUserParam.First_Name, "Please enter all required fields to register");
            CheckString(incomingUserParam.Email, "Please enter all required fields to register");
            
            User newUser = null;
            int newUserId = 0;

            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(incomingUserParam.Password);

            string sql = "INSERT INTO users (username, password_hash, salt, first_name, last_name, email, role) " +
                "OUTPUT inserted.user_id " +
                "VALUES (@username, @password_hash, @salt, @first_name, @last_name, @email, @role);";
            
            try
            {
                using(SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", incomingUserParam.Username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.Parameters.AddWithValue("@first_name", incomingUserParam.First_Name);
                    cmd.Parameters.AddWithValue("@last_name", incomingUserParam.Last_Name);
                    cmd.Parameters.AddWithValue("@email", incomingUserParam.Email);
                    cmd.Parameters.AddWithValue("@role", DefaultRole);

                    newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                newUser = GetUserById(newUserId);
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return newUser;
        }

        public User GetUserById(int id)
        {
            User user ;

            string sql = "SELECT user_id, username, password_hash, salt, first_name, last_name, email, role, is_active, lb_creation_date, lb_update_date " +
                "FROM users " +
                "WHERE user_id = @user_id;";
            
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"user_id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user = MapRowToUser(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        public User GetUserByUsername(string username)
        {
            CheckString(username);

            User user;

            string sql = "SELECT user_id, username, password_hash, salt, first_name, last_name, email, role, is_active, lb_creation_date, lb_update_date " +
                "FROM users " +
                "WHERE username = @username;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user = MapRowToUser(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        public User IncreaseUserRole(string username)
        {
            CheckString(username);

            User user;

            string sql = "UPDATE users " +
                "SET role = @role, lb_update_date = @now " +
                "WHERE username = @username;";
            
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"role", AdminRole);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();
                    if(numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                    }                                       
                }
                user = GetUserByUsername(username);
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        public User DecreaseUserRole(string username)
        {
            CheckString(username);

            User user;

            string sql = "UPDATE users " +
                "SET role = @role, lb_update_date = @now " +
                "WHERE username = @username;";
             
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"role", DefaultRole);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();
                    if (numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                    }
                }
                user = GetUserByUsername(username);
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        public User DeactivateUser(string username)
        {
            CheckString(username);

            User user;

            string sql = "UPDATE users " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE username = @username;";

            string sqlUserSets = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username);";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"is_active", Inactive);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();

                    SqlCommand cmdUserSets = new SqlCommand(sqlUserSets, conn);
                    cmdUserSets.Parameters.AddWithValue(@"username", username);
                    cmdUserSets.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdUserSets.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsUserSets = cmd.ExecuteNonQuery();
                    // can have a user with zero sets who wants to be inactive
                    // so don't check for that
                    if (numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                    }
                }
                user = GetUserByUsername(username);
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        public User ReactivateUser(string username)
        {
            CheckString(username);

            User user;

            string sql = "UPDATE users " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE username = @username;";

            string sqlUserSets = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username);";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"is_active", Active);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();

                    SqlCommand cmdUserSets = new SqlCommand(sqlUserSets, conn);
                    cmdUserSets.Parameters.AddWithValue(@"username", username);
                    cmdUserSets.Parameters.AddWithValue(@"is_active", Active);
                    cmdUserSets.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsUserSets = cmd.ExecuteNonQuery();
                    // can have a user with zero sets who wants to be reactivated
                    // so don't check for that
                    if (numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                    }
                }
                user = GetUserByUsername(username);
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return user;
        }

        private User MapRowToUser(SqlDataReader reader)
        {
            User user = new User();
            user.User_Id = Convert.ToInt32(reader["user_id"]);
            user.Username = Convert.ToString(reader["username"]);
            user.Password_Hash = Convert.ToString(reader["password_hash"]);
            user.Salt = Convert.ToString(reader["salt"]);
            user.First_Name = Convert.ToString(reader["first_name"]);
            user.Last_Name = Convert.ToString(reader["last_name"]);
            user.Email = Convert.ToString(reader["email"]);
            user.Role = Convert.ToString(reader["role"]);
            user.Is_Active = Convert.ToBoolean(reader["is_active"]);
            user.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            user.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return user;
        }
    }
}
