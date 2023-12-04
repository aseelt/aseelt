using LegoBuilder.Exceptions;
using LegoBuilder.Models;
using System.Data.SqlClient;
using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using LegoBuilder.Models.All;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Diagnostics;
using LegoBuilder.Utilities;

namespace LegoBuilder.SqlDaos
{
    public class ThemeSqlDao : BaseSqlDao, IThemeSqlDao
    {
        public ThemeSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }


        public Theme GetThemeById(string id)
        {
            CheckString(id);

            Theme output;

            string sql = "SELECT themes_id, id, parent_id, name, is_active, lb_creation_date, lb_update_date " +
                "FROM themes " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    output = GetThemeInUsingBlock(sql, conn, "@id", id);
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
            return output;
        }
        public List<Theme> WildcardSearchThemes(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<Theme> output = new List<Theme>();

            string sql = "SELECT themes_id, id, parent_id, name, is_active, lb_creation_date, lb_update_date, " +
                "FROM themes " +
                "WHERE name LIKE @searchId " +
                "ORDER BY name;";


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"searchId", searchId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Theme newRow = MapRowToTheme(reader);
                        output.Add(newRow);
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
            return output;
        }
        public List<Theme> GetAllThemes()
        {
            List<Theme> output = new List<Theme>();

            string sql = "SELECT themes_id, id, parent_id, name, is_active, lb_creation_date, lb_update_date " +
                "FROM themes " +
                "ORDER BY themes_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Theme newTheme = MapRowToTheme(reader);
                        output.Add(newTheme);
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
            return output;
        }

        public string DeleteTheme(string id)
        {
            CheckString(id);

            string sql = "UPDATE themes " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"id", id);
                    cmd.Parameters.AddWithValue(@"is_active", Inactive);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();
                    if (numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
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

            return $"Theme {id} has been inactivated";
        }

        public string UnDeleteTheme(string id)
        {
            CheckString(id);

            string sql = "UPDATE themes " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"id", id);
                    cmd.Parameters.AddWithValue(@"is_active", Active);
                    cmd.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRows = cmd.ExecuteNonQuery();
                    if (numberOfRows != 1)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
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

            return $"Theme {id} has been inactivated";
        }

        public AllThemes UpdateDatabase(AllThemes allThemes)
        {
            CheckCount(allThemes.Results.Count);

            AllThemes output = new AllThemes();

            Theme currentTheme = new Theme();

            int newThemeId = 0;
            int updatedRows = 0;

            string sqlGetById = "SELECT themes_id, id, parent_id, name, is_active, lb_creation_date, lb_update_date " +
                "FROM themes " +
                "WHERE id = @id;";

            string sqlGetByThemeId = "SELECT themes_id, id, parent_id, name, is_active, lb_creation_date, lb_update_date " +
                "FROM themes " +
                "WHERE themes_id = @themes_id;";

            string sqlAddToDatabse = "INSERT INTO themes (id, parent_id, name) " +
                "OUTPUT inserted.themes_id " +
                "VALUES (@id, @parent_id, @name);";

            string sqlUpdateDatabase = "UPDATE themes " +
                "SET parent_id = @parent_id, name = @name, lb_update_date = @lb_update_date " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // checks the database for the colour
                    foreach (Theme theme in allThemes.Results)
                    {
                        currentTheme = GetThemeInUsingBlock(sqlGetById, conn, "@id", theme.Id.ToString());

                        if (currentTheme == null)
                        {
                            // add to the database
                            // add to list of added sets
                            Theme newTheme = new Theme();

                            SqlCommand cmdAddTheme = new SqlCommand(sqlAddToDatabse, conn);
                            cmdAddTheme.Parameters.AddWithValue(@"id", theme.Id);
                            cmdAddTheme.Parameters.AddWithValue(@"parent_id", theme.Parent_Id is null ? DBNull.Value : theme.Parent_Id);
                            cmdAddTheme.Parameters.AddWithValue(@"name", theme.Name);

                            newThemeId = Convert.ToInt32(cmdAddTheme.ExecuteScalar());

                            newTheme = GetThemeInUsingBlock(sqlGetByThemeId, conn, "@themes_id", newThemeId.ToString());

                            output.Added_Updated[newTheme.Id.ToString()] = "Added";
                        }
                        else if (CheckObjectMatch(theme, currentTheme))
                        {
                            // assert fail
                            // update database
                            // update updated column
                            // add ot list of updated theme
                            Theme updatedTheme = new Theme();

                            SqlCommand cmdUpdatedTheme = new SqlCommand(sqlUpdateDatabase, conn);
                            cmdUpdatedTheme.Parameters.AddWithValue(@"parent_id", theme.Parent_Id is null ? DBNull.Value : theme.Parent_Id);
                            cmdUpdatedTheme.Parameters.AddWithValue(@"name", theme.Name);
                            cmdUpdatedTheme.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);
                            cmdUpdatedTheme.Parameters.AddWithValue(@"id", theme.Id);

                            updatedRows = cmdUpdatedTheme.ExecuteNonQuery();

                            if (updatedRows != 1)
                            {
                                throw new DaoException("Something went wrong and the wrong number of rows were updated");
                            }

                            updatedTheme = GetThemeInUsingBlock(sqlGetById, conn, "@id", theme.Id.ToString());

                            output.Added_Updated[theme.Id.ToString()] = "Updated";
                        }

                    }
                }
            }
            catch (SqlException sqe)
            {
                CreationUpdateLog.WriteLog("Sql Error", "Themes", $"{output.Added_Updated.Count} records were actioned before failure. {sqe.Message}");
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                int count = output.Count;
                throw new DaoException("There was an error");
            }
            output.Count = output.Added_Updated.Count;
            if (output.Count == 0)
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Themes", $"Zero records were actioned");
            }
            else
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Themes", $"Success! {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            return output;
        }

        private Theme MapRowToTheme(SqlDataReader reader)
        {
            Theme output = new Theme();
            output.Themes_Id = Convert.ToInt32(reader["themes_id"]);
            output.Id = Convert.ToInt32(reader["id"]);
            if (reader["parent_id"] is DBNull)
            {
                output.Parent_Id = null;
            }
            else
            {
                output.Parent_Id = Convert.ToInt32(reader["parent_id"]);
            }
            output.Name = Convert.ToString(reader["name"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }

        private bool CheckObjectMatch(Theme toCheck, Theme inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Name != inDatabase.Name ||
                toCheck.Parent_Id != inDatabase.Parent_Id)
            {
                return true;
            }
            return false;
        }

        private Theme GetThemeInUsingBlock(string query, SqlConnection conn, string parameter, string id)
        {
            Theme output = null;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToTheme(reader);
            }
            reader.Close();
            return output;
        }
    }
}
