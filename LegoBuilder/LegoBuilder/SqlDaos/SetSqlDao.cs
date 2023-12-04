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
using System.Diagnostics.Metrics;

namespace LegoBuilder.SqlDaos
{
    public class SetSqlDao : BaseSqlDao, ISetSqlDao
    {
        public SetSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }


        public Set GetSetById(string id)
        {
            CheckString(id);

            Set output;

            string sql = "SELECT sets_id, set_num, sets.name, year, sets.theme_id, themes.name AS theme_name, num_parts, set_img_url, last_modified_dt, sets.is_active, sets.lb_creation_date, sets.lb_update_date, " +
                "url " +
                "FROM sets " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "WHERE set_num = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"id", id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        output = MapRowToSet(reader);
                        output.Theme_Name = Convert.ToString(reader["theme_name"]);
                    }
                    else
                    {
                        output = null;
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
        public List<Set> WildcardSearchSets(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<Set> output = new List<Set>();

            // searches set num, set name, or theme name
            string sql = "SELECT sets_id, set_num, sets.name, year, sets.theme_id, themes.name AS theme_name, num_parts, set_img_url, last_modified_dt, sets.is_active, sets.lb_creation_date, sets.lb_update_date, " +
                "url " +
                "FROM sets " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "WHERE set_num LIKE @searchId OR sets.name LIKE @searchId OR themes.name LIKE @searchId " +
                "ORDER BY sets_num;";


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
                        Set newRow = MapRowToSet(reader, true);
                        newRow.Theme_Name = Convert.ToString(reader["theme_name"]);
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
        public List<Set> GetAllSets()
        {
            List<Set> output = new List<Set>();

            string sql = "SELECT sets_id, set_num, sets.name, year, sets.theme_id, themes.name AS theme_name, num_parts, set_img_url, last_modified_dt, sets.is_active, sets.lb_creation_date, sets.lb_update_date, " +
                "url " +
                "FROM sets " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "ORDER BY set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Set newRow = MapRowToSet(reader, true);
                        newRow.Theme_Name = Convert.ToString(reader["theme_name"]);
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

        public string DeleteSet(string id)
        {
            CheckString(id);

            string sqlSet = "UPDATE sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            string sqlSetParts = "UPDATE sets_parts " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            string sqlInstructions = "UPDATE instructions " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_id = @id;";

            string sqlUsersSets = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmdSet = new SqlCommand(sqlSet, conn);
                    cmdSet.Parameters.AddWithValue(@"id", id);
                    cmdSet.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdSet.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsSet = cmdSet.ExecuteNonQuery();

                    SqlCommand cmdSetParts = new SqlCommand(sqlSetParts, conn);
                    cmdSetParts.Parameters.AddWithValue(@"id", id);
                    cmdSetParts.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdSetParts.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsSetParts = cmdSetParts.ExecuteNonQuery();

                    SqlCommand cmdInstructions = new SqlCommand(sqlInstructions, conn);
                    cmdInstructions.Parameters.AddWithValue(@"id", id);
                    cmdInstructions.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdInstructions.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsInstructions = cmdInstructions.ExecuteNonQuery();

                    if (numberOfRowsSet != 1 || numberOfRowsSet != numberOfRowsSetParts || numberOfRowsSet != numberOfRowsInstructions)
                    {
                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                    }

                    // put this after. Don't care if it throws an exception, not all sets will be in the users_sets list
                    SqlCommand cmdUsersSets = new SqlCommand(sqlUsersSets, conn);
                    cmdUsersSets.Parameters.AddWithValue(@"id", id);
                    cmdUsersSets.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdUsersSets.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    cmdUsersSets.ExecuteNonQuery();
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

            return $"Set {id} has been inactivated";
        }

        public string UnDeleteSet(string id)
        {
            CheckString(id);

            string sqlSet = "UPDATE sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            string sqlSetParts = "UPDATE sets_parts " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            string sqlInstructions = "UPDATE instructions " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_id = @id;";

            string sqlUsersSets = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE set_num = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmdSet = new SqlCommand(sqlSet, conn);
                    cmdSet.Parameters.AddWithValue(@"id", id);
                    cmdSet.Parameters.AddWithValue(@"is_active", Active);
                    cmdSet.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsSet = cmdSet.ExecuteNonQuery();

                    SqlCommand cmdSetParts = new SqlCommand(sqlSetParts, conn);
                    cmdSetParts.Parameters.AddWithValue(@"id", id);
                    cmdSetParts.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdSetParts.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsSetParts = cmdSetParts.ExecuteNonQuery();

                    SqlCommand cmdInstructions = new SqlCommand(sqlInstructions, conn);
                    cmdInstructions.Parameters.AddWithValue(@"id", id);
                    cmdInstructions.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdInstructions.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    int numberOfRowsInstructions = cmdSetParts.ExecuteNonQuery();

                    if (numberOfRowsSet != 1 || numberOfRowsSet != numberOfRowsSetParts || numberOfRowsSet != numberOfRowsInstructions)
                    {
                        {
                            throw new DaoException("Something went wrong and the wrong number of rows were updated");
                        }
                    }
                    // put this after. Don't care if it throws an exception, not all sets will be in the users_sets list
                    SqlCommand cmdUsersSets = new SqlCommand(sqlUsersSets, conn);
                    cmdUsersSets.Parameters.AddWithValue(@"id", id);
                    cmdUsersSets.Parameters.AddWithValue(@"is_active", Inactive);
                    cmdUsersSets.Parameters.AddWithValue(@"now", DateTime.UtcNow);

                    cmdUsersSets.ExecuteNonQuery();
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

            return $"Set {id} has been inactivated";
        }

        public AllSets UpdateDatabase(AllSets allSets)
        {
            CheckCount(allSets.Results.Count);

            AllSets output = new AllSets();

            Set currentSet = new Set();

            int newSetId = 0;
            int newInstructionsId = 0;
            int updatedRows = 0;

            // not returning the whole thing, just the set number, so don't need a join
            string sqlGetById = "SELECT sets_id, set_num, name, year, theme_id, num_parts, set_img_url, last_modified_dt, is_active, lb_creation_date, lb_update_date " +
                "FROM sets " +
                "WHERE set_num = @id;";

            string sqlGetBySetId = "SELECT sets_id, set_num, name, year, theme_id, num_parts, set_img_url, last_modified_dt, is_active, lb_creation_date, lb_update_date " +
                "FROM sets " +
                "WHERE sets_id = @sets_id;";

            string sqlAddToDatabse = "INSERT INTO sets (set_num, name, year, theme_id, num_parts, set_img_url, last_modified_dt) " +
                "OUTPUT inserted.sets_id " +
                "VALUES (@set_num, @name, @year, @theme_id, @num_parts, @set_img_url, @last_modified_dt);";

            string sqlUpdateDatabase = "UPDATE sets " +
                "SET name = @name, year = @year, theme_id = @theme_id, num_parts = @num_parts, set_img_url = @set_img_url, lb_update_date = @lb_update_date " +
                "WHERE set_num = @id;";

            string sqlAddInstructions = "INSERT INTO instructions (set_id, url) " +
                "OUTPUT inserted.instructions_id " +
                "VALUES (@set_id, @url);";

            // for debug
            int counter = 1;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // checks the database for the colour
                    foreach (Set set in allSets.Results)
                    {
                        currentSet = GetSetInUsingBlock(sqlGetById, conn, "@id", set.Set_Num);

                        if (currentSet == null)
                        {
                            // add to the database
                            // add to list of added sets
                            Set newSet = new Set();

                            SqlCommand cmdAddSet = new SqlCommand(sqlAddToDatabse, conn);
                            cmdAddSet.Parameters.AddWithValue(@"set_num", set.Set_Num);
                            cmdAddSet.Parameters.AddWithValue(@"name", set.Name);
                            cmdAddSet.Parameters.AddWithValue(@"year", set.Year);
                            cmdAddSet.Parameters.AddWithValue(@"theme_id", set.Theme_Id);
                            cmdAddSet.Parameters.AddWithValue(@"num_parts", set.Num_Parts);
                            cmdAddSet.Parameters.AddWithValue(@"set_img_url", set.Set_Img_Url is null ? DBNull.Value : set.Set_Img_Url);
                            cmdAddSet.Parameters.AddWithValue(@"last_modified_dt", set.Last_Modified_Dt);

                            newSetId = Convert.ToInt32(cmdAddSet.ExecuteScalar());

                            //TODO check instructions url is valid - it's not working properly, always gives me a 200 status code
                            // even for gobbledegook urls
                            string setNumTruncated = SetNumTruncater(set.Set_Num);
                            SqlCommand cmdAddInstructions = new SqlCommand(sqlAddInstructions, conn);
                            cmdAddInstructions.Parameters.AddWithValue(@"set_id", set.Set_Num);
                            cmdAddInstructions.Parameters.AddWithValue(@"url", LegoInstructionsUrl + setNumTruncated);
                            newInstructionsId = Convert.ToInt32(cmdAddInstructions.ExecuteScalar());

                            newSet = GetSetInUsingBlock(sqlGetBySetId, conn, "@sets_id", newSetId.ToString());

                            output.Added_Updated[newSet.Set_Num] = "Added";
                        }
                        else if (CheckObjectMatch(set, currentSet))
                        {
                            // assert fail
                            // update database
                            // update updated column
                            // add ot list of updated set
                            Set updatedSet = new Set();

                            SqlCommand cmdUpdatedPart = new SqlCommand(sqlUpdateDatabase, conn);
                            cmdUpdatedPart.Parameters.AddWithValue(@"name", set.Name);
                            cmdUpdatedPart.Parameters.AddWithValue(@"year", set.Year);
                            cmdUpdatedPart.Parameters.AddWithValue(@"theme_id", set.Theme_Id);
                            cmdUpdatedPart.Parameters.AddWithValue(@"num_parts", set.Num_Parts);
                            cmdUpdatedPart.Parameters.AddWithValue(@"set_img_url", set.Set_Img_Url is null ? DBNull.Value : set.Set_Img_Url);
                            cmdUpdatedPart.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);
                            cmdUpdatedPart.Parameters.AddWithValue(@"id", set.Set_Num);

                            updatedRows = cmdUpdatedPart.ExecuteNonQuery();

                            if (updatedRows != 1)
                            {
                                throw new DaoException("Something went wrong and the wrong number of rows were updated");
                            }

                            updatedSet = GetSetInUsingBlock(sqlGetById, conn, "@id", set.Set_Num);

                            output.Added_Updated[updatedSet.Set_Num.ToString()] = "Updated";
                        }

                        // for debug 
                        counter++;
                        if (counter % 1000 == 0)
                        {
                            Debug.WriteLine($"Processed {counter} rows");
                        }
                    }
                }
            }
            catch (SqlException sqe)
            {
                CreationUpdateLog.WriteLog("Sql Error", "Sets", $"{output.Added_Updated.Count} records were actioned before failure. {sqe.Message}");
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
                CreationUpdateLog.WriteLog("Created & Updated", "Sets", $"Zero records were actioned");
            }
            else
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Sets", $"Success! {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            if (output.Count != allSets.Results.Count)
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Sets", $"Partial Success! Some records may not be in the database - the program is weird. {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            return output;
        }
        public bool UpdateInstructions()
        {
            // get all the sets
            List<Set> allSets = GetAllSets();
            // then for each set, update the instructions
            string sql = "UPDATE instructions " +
                "SET url = @url " +
                "WHERE set_id = @id;";
            int counter = 1;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    foreach(Set set in allSets)
                    {
                        string setNumTruncated = SetNumTruncater(set.Set_Num);
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue(@"id", set.Set_Num);
                        cmd.Parameters.AddWithValue(@"url", LegoInstructionsUrl + setNumTruncated);

                        int updatedRows = cmd.ExecuteNonQuery();

                        if (updatedRows != 1)
                        {
                            throw new DaoException("Something went wrong and the wrong number of rows were updated");
                        }
                        // for debug 
                        counter++;
                        if (counter % 1000 == 0)
                        {
                            Debug.WriteLine($"Processed {counter} rows");
                        }
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

            return true;
        }

        private Set MapRowToSet(SqlDataReader reader, bool getInstructions = false)
        {
            Set output = new Set();
            output.Set_Id = Convert.ToInt32(reader["sets_id"]);
            output.Set_Num = Convert.ToString(reader["set_num"]);
            output.Name = Convert.ToString(reader["name"]);
            output.Year = Convert.ToInt32(reader["year"]);
            output.Theme_Id = Convert.ToInt32(reader["theme_id"]);
            // can't have theme_name here or it'll fail on the add or update
            //output.Theme_Name = Convert.ToString(reader["theme_name"]);
            output.Num_Parts = Convert.ToInt32(reader["num_parts"]);
            output.Set_Img_Url = Convert.ToString(reader["set_img_url"]);
            if (reader["set_img_url"] is DBNull)
            {
                output.Set_Img_Url = null;
            }
            else
            {
                output.Set_Img_Url = Convert.ToString(reader["set_img_url"]);
            }
            output.Last_Modified_Dt = Convert.ToDateTime(reader["last_modified_dt"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);
            if (getInstructions)
            {
                if (reader["url"] is DBNull)
                {
                    output.Instructions_Url = null;
                }
                else
                {
                    output.Instructions_Url = Convert.ToString(reader["url"]);
                }
            }
            

            return output;
        }

        private bool CheckObjectMatch(Set toCheck, Set inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Name != inDatabase.Name ||
                toCheck.Year != inDatabase.Year ||
                toCheck.Theme_Id != inDatabase.Theme_Id ||
                toCheck.Num_Parts != inDatabase.Num_Parts ||
                toCheck.Set_Img_Url != inDatabase.Set_Img_Url ||
                toCheck.Last_Modified_Dt.Year != inDatabase.Last_Modified_Dt.Year ||
                toCheck.Last_Modified_Dt.Month != inDatabase.Last_Modified_Dt.Month ||
                toCheck.Last_Modified_Dt.Day != inDatabase.Last_Modified_Dt.Day ||
                toCheck.Last_Modified_Dt.Hour != inDatabase.Last_Modified_Dt.Hour ||
                toCheck.Last_Modified_Dt.Minute != inDatabase.Last_Modified_Dt.Minute ||
                toCheck.Last_Modified_Dt.Second != inDatabase.Last_Modified_Dt.Second)
            {
                return true;
            }
            return false;
        }

        private Set GetSetInUsingBlock(string query, SqlConnection conn, string parameter, string id, bool getInstructions = false)
        {
            Set output = null;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToSet(reader, getInstructions);
            }
            reader.Close();
            return output;
        }
    }
}
