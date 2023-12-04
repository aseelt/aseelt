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
using LegoBuilder.Utilities;
using System.Runtime.Intrinsics.X86;

namespace LegoBuilder.SqlDaos
{
    public class ColourSqlDao : BaseSqlDao, IColourSqlDao
    {
        public ColourSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }


        public Colour GetColourById(string id)
        {
            CheckString(id);

            Colour output;

            string sql = "SELECT colours_id, id, name, rgb, is_trans, is_active, lb_creation_date, lb_update_date " +
                "FROM colours " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    output = GetColourInUsingBlock(sql, conn, "@id", id);
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
        public List<Colour> WildcardSearchColours(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<Colour> output = new List<Colour>();

            string sql = "SELECT colours_id, id, name, rgb, is_trans, is_active, lb_creation_date, lb_update_date, " +
                "FROM colours " +
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
                        Colour newRow = MapRowToColour(reader);
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
        public List<Colour> GetAllColours()
        {
            List<Colour> output = new List<Colour>();

            string sql = "SELECT colours_id, id, name, rgb, is_trans, is_active, lb_creation_date, lb_update_date " +
                "FROM colours " +
                "ORDER BY colours_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Colour newRow = MapRowToColour(reader);
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

        public string DeleteColour(string id)
        {
            CheckString(id);

            string sql = "UPDATE colours " +
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

            return $"Colour {id} has been inactivated";
        }

        public string UnDeleteColour(string id)
        {
            CheckString(id);

            string sql = "UPDATE colours " +
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

            return $"Colour {id} has been inactivated";
        }

        public AllColours UpdateDatabase(AllColours allColours)
        {
            // open the connection
            // for each item in allcolours
            // pull the id
            // check if it's in the database
            // if found and all rows match
            //      do nothing
            // if found and all rows don't match
            //      update
            //      add it to the list of all colours of what's been updated, with update string
            // if null,
            //      add it to the database 
            //      add it to the list of all colours of what's been updated, with added string
            CheckCount(allColours.Results.Count);

            AllColours output = new AllColours();

            Colour currentColour = new Colour();

            int newColourId = 0;
            int updatedRows = 0;

            string sqlGetById = "SELECT colours_id, id, name, rgb, is_trans, is_active, lb_creation_date, lb_update_date " +
                "FROM colours " +
                "WHERE id = @id;";

            string sqlGetByColourId = "SELECT colours_id, id, name, rgb, is_trans, is_active, lb_creation_date, lb_update_date " +
                "FROM colours " +
                "WHERE colours_id = @colours_id;";

            string sqlAddToDatabse = "INSERT INTO colours (id, name, rgb, is_trans) " +
                "OUTPUT inserted.colours_id " +
                "VALUES (@id, @name, @rgb, @is_trans);";

            string sqlUpdateDatabase = "UPDATE colours " +
                "SET name = @name, rgb = @rgb, is_trans = @is_trans, lb_update_date = @lb_update_date " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // checks the database for the colour
                    foreach (Colour colour in allColours.Results)
                    {
                        currentColour = GetColourInUsingBlock(sqlGetById, conn, "@id", colour.Id);

                        if (currentColour == null)
                        {
                            // add to the database
                            // add to list of added colours
                            Colour newColour = new Colour();

                            SqlCommand cmdAddColour = new SqlCommand(sqlAddToDatabse, conn);
                            cmdAddColour.Parameters.AddWithValue(@"id", colour.Id);
                            cmdAddColour.Parameters.AddWithValue(@"name", colour.Name);
                            cmdAddColour.Parameters.AddWithValue(@"rgb", colour.RGB);
                            cmdAddColour.Parameters.AddWithValue(@"is_trans", colour.Is_Trans);

                            newColourId = Convert.ToInt32(cmdAddColour.ExecuteScalar());

                            newColour = GetColourInUsingBlock(sqlGetByColourId, conn, "@colours_id", newColourId.ToString());

                            output.Added_Updated[newColour.Id] = "Added";
                        }
                        else if (CheckObjectMatch(colour, currentColour))
                        {
                            // assert fail
                            // update database
                            // update updated column
                            // add ot list of updated colours
                            Colour updatedColour = new Colour();

                            SqlCommand cmdUpdatedColour = new SqlCommand(sqlUpdateDatabase, conn);
                            cmdUpdatedColour.Parameters.AddWithValue(@"name", colour.Name);
                            cmdUpdatedColour.Parameters.AddWithValue(@"rgb", colour.RGB);
                            cmdUpdatedColour.Parameters.AddWithValue(@"is_trans", colour.Is_Trans);
                            cmdUpdatedColour.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);
                            cmdUpdatedColour.Parameters.AddWithValue(@"id", colour.Id);

                            updatedRows = cmdUpdatedColour.ExecuteNonQuery();

                            if (updatedRows != 1)
                            {
                                throw new DaoException("Something went wrong and the wrong number of rows were updated");
                            }

                            updatedColour = GetColourInUsingBlock(sqlGetById, conn, "@id", colour.Id);

                            output.Added_Updated[updatedColour.Id] = "Updated";
                        }
                    }

                }
            }
            catch (SqlException sqe)
            {
                CreationUpdateLog.WriteLog("Sql Error", "Colours", $"{output.Added_Updated.Count} records were actioned before failure. {sqe.Message}");
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            output.Count = output.Added_Updated.Count;
            if (output.Count == 0)
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Colours", $"Zero records were actioned");
            }
            else
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Colours", $"Success! {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            return output;
        }

        private Colour MapRowToColour(SqlDataReader reader)
        {
            Colour output = new Colour();
            output.Colours_Id = Convert.ToInt32(reader["colours_id"]);
            output.Id = Convert.ToString(reader["id"]);
            output.Name = Convert.ToString(reader["name"]);
            output.RGB = Convert.ToString(reader["rgb"]);
            output.Is_Trans = Convert.ToBoolean(reader["is_trans"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }

        private bool CheckObjectMatch(Colour toCheck, Colour inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Name != inDatabase.Name ||
                toCheck.RGB != inDatabase.RGB ||
                toCheck.Is_Trans != inDatabase.Is_Trans)
            {
                return true;
            }
            return false;
        }

        private Colour GetColourInUsingBlock(string query, SqlConnection conn, string parameter, string id)
        {
            Colour output = null;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToColour(reader);
            }
            reader.Close();
            return output;
        }
    }
}
