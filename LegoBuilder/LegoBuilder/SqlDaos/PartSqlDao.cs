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
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace LegoBuilder.SqlDaos
{
    public class PartSqlDao : BaseSqlDao, IPartSqlDao
    {
        public PartSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }


        public Part GetPartById(string id)
        {
            CheckString(id);

            Part output;

            string sql = "SELECT parts_id, part_num, parts.name, parts.part_cat_id, part_categories.name AS category_name, year_from, year_to, part_url, part_img_url, parts.is_active, parts.lb_creation_date, parts.lb_update_date " +
                "FROM parts " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE part_num = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    output = GetPartInUsingBlock(sql, conn, "@id", id);
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
        public List<Part> WildcardSearchParts(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<Part> output = new List<Part>();

            // lets do a join to the part category too, so you can search that as well when looking for parts
            string sql = "SELECT parts_id, part_num, parts.name, parts.part_cat_id, part_categories.name, year_from, year_to, part_url, part_img_url, parts.is_active, parts.lb_creation_date, parts.lb_update_date " +
                "FROM parts " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE part_num LIKE @searchId OR parts.name LIKE @searchId OR part_categories.name LIKE @searchId " +
                "ORDER BY part_num";


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
                        Part newRow = MapRowToPart(reader);
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

        public List<Part> GetAllParts()
        {
            List<Part> output = new List<Part>();

            string sql = "SELECT parts_id, part_num, parts.name, parts.part_cat_id, part_categories.name AS category_name, year_from, year_to, part_url, part_img_url, parts.is_active, parts.lb_creation_date, parts.lb_update_date " +
                "FROM parts " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "ORDER BY parts_id;"; 

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Part newRow = MapRowToPart(reader);
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

        public string DeletePart(string id)
        {
            CheckString(id);

            string sql = "UPDATE parts " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE part_num = @id;";

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

            return $"Part {id} has been inactivated";
        }

        public string UnDeletePart(string id)
        {
            CheckString(id);

            string sql = "UPDATE parts " +
                "SET is_active = @is_active, lb_update_date = @now " +
                "WHERE part_num = @id;";

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

            return $"Part {id} has been inactivated";
        }

        public AllParts UpdateDatabase(AllParts allParts)
        {
            CheckCount(allParts.Results.Count);

            AllParts output = new AllParts();

            Part currentPart = new Part();

            int newPartId = 0;
            int updatedRows = 0;

            string sqlGetById = "SELECT parts_id, part_num, parts.name, parts.part_cat_id, part_categories.name AS category_name, year_from, year_to, part_url, part_img_url, parts.is_active, parts.lb_creation_date, parts.lb_update_date " +
                "FROM parts " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE part_num = @id;";

            string sqlGetByPartId = "SELECT parts_id, part_num, parts.name, parts.part_cat_id, part_categories.name AS category_name, year_from, year_to, part_url, part_img_url, parts.is_active, parts.lb_creation_date, parts.lb_update_date " +
                "FROM parts " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE parts_id = @parts_id;";

            string sqlAddToDatabse = "INSERT INTO parts (part_num, name, part_cat_id, year_from, year_to, part_url, part_img_url) " +
                "OUTPUT inserted.parts_id " +
                "VALUES (@part_num, @name, @part_cat_id, @year_from, @year_to, @part_url, @part_img_url);";

            string sqlUpdateDatabase = "UPDATE parts " +
                "SET name = @name, part_cat_id = @part_cat_id, year_from = @year_from, year_to = @year_to, part_url = @part_url, part_img_url = @part_img_url, lb_update_date = @lb_update_date " +
                "WHERE part_num = @id;";

            // for debug
            int counter = 1;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // checks the database for the colour
                    foreach (Part part in allParts.Results)
                    {
                        currentPart = GetPartInUsingBlock(sqlGetById, conn, "@id", part.Part_Num);

                        if (currentPart == null)
                        {
                            // add to the database
                            // add to list of added parts
                            Part newPart = new Part();

                            SqlCommand cmdAddPart = new SqlCommand(sqlAddToDatabse, conn);
                            cmdAddPart.Parameters.AddWithValue(@"part_num", part.Part_Num);
                            cmdAddPart.Parameters.AddWithValue(@"name", part.Name);
                            cmdAddPart.Parameters.AddWithValue(@"part_cat_id", part.Part_Cat_Id);
                            cmdAddPart.Parameters.AddWithValue(@"year_from", part.Year_From is null ? DBNull.Value : part.Year_From);
                            cmdAddPart.Parameters.AddWithValue(@"year_to", part.Year_To is null ? DBNull.Value : part.Year_From);
                            cmdAddPart.Parameters.AddWithValue(@"part_url", part.Part_Url is null ? DBNull.Value : part.Part_Url);
                            cmdAddPart.Parameters.AddWithValue(@"part_img_url", part.Part_Img_Url is null ? DBNull.Value : part.Part_Img_Url);

                            newPartId = Convert.ToInt32(cmdAddPart.ExecuteScalar());

                            newPart = GetPartInUsingBlock(sqlGetByPartId, conn, "@parts_id", newPartId.ToString());

                            output.Added_Updated[newPart.Part_Num] = "Added";
                        }
                        else if (CheckObjectMatch(part, currentPart))
                        {
                            // assert fail
                            // update database
                            // update updated column
                            // add ot list of updated part
                            Part updatedPart = new Part();

                            SqlCommand cmdUpdatedPart = new SqlCommand(sqlUpdateDatabase, conn);
                            cmdUpdatedPart.Parameters.AddWithValue(@"name", part.Name);
                            cmdUpdatedPart.Parameters.AddWithValue(@"part_cat_id", part.Part_Cat_Id);
                            cmdUpdatedPart.Parameters.AddWithValue(@"year_from", part.Year_From is null ? DBNull.Value : part.Year_From);
                            cmdUpdatedPart.Parameters.AddWithValue(@"year_to", part.Year_To is null ? DBNull.Value : part.Year_From);
                            cmdUpdatedPart.Parameters.AddWithValue(@"part_url", part.Part_Url is null ? DBNull.Value : part.Part_Url);
                            cmdUpdatedPart.Parameters.AddWithValue(@"part_img_url", part.Part_Img_Url is null ? DBNull.Value : part.Part_Img_Url);
                            cmdUpdatedPart.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);
                            cmdUpdatedPart.Parameters.AddWithValue(@"id", part.Part_Num);

                            updatedRows = cmdUpdatedPart.ExecuteNonQuery();

                            if (updatedRows != 1)
                            {
                                throw new DaoException("Something went wrong and the wrong number of rows were updated");
                            }

                            updatedPart = GetPartInUsingBlock(sqlGetById, conn, "@id", part.Part_Num);

                            output.Added_Updated[updatedPart.Part_Num.ToString()] = "Updated";
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
                CreationUpdateLog.WriteLog("Sql Error", "Parts", $"{output.Added_Updated.Count} records were actioned before failure. {sqe.Message}");
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
                CreationUpdateLog.WriteLog("Created & Updated", "Parts", $"Zero records were actioned");
            }
            else
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Parts", $"Success! {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            if (output.Count != allParts.Results.Count)
            {
                CreationUpdateLog.WriteLog("Created & Updated", "Parts", $"Partial Success! Some records may not be in the database - the program is weird. {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            
            return output;
        }

        private Part MapRowToPart(SqlDataReader reader)
        {
            Part output = new Part();
            output.Parts_Id = Convert.ToInt32(reader["parts_id"]);
            output.Part_Num = Convert.ToString(reader["part_num"]);
            output.Name = Convert.ToString(reader["name"]);
            output.Part_Cat_Id = Convert.ToInt32(reader["part_cat_id"]);
            output.Category_Name = Convert.ToString(reader["category_name"]);
            if (reader["year_from"] is DBNull)
            {
                output.Year_From = null;
            }
            else
            {
                output.Year_From = Convert.ToInt32(reader["year_from"]);
            }
            if (reader["year_to"] is DBNull)
            {
                output.Year_To = null;
            }
            else
            {
                output.Year_To = Convert.ToInt32(reader["year_to"]);
            }
            if (reader["part_url"] is DBNull)
            {
                output.Part_Url = null;
            }
            else
            {
                output.Part_Url = Convert.ToString(reader["part_url"]);
            }
            if (reader["part_img_url"] is DBNull)
            {
                output.Part_Img_Url = null;
            }
            else
            {
                output.Part_Img_Url = Convert.ToString(reader["part_img_url"]);
            }
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }

        private bool CheckObjectMatch(Part toCheck, Part inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Name != inDatabase.Name ||
                toCheck.Part_Cat_Id != inDatabase.Part_Cat_Id ||
                toCheck.Year_From != inDatabase.Year_From ||
                toCheck.Year_To != inDatabase.Year_To ||
                toCheck.Part_Url != inDatabase.Part_Url ||
                toCheck.Part_Img_Url != inDatabase.Part_Img_Url)
            {
                return true;
            }
            return false;
        }

        private Part GetPartInUsingBlock(string query, SqlConnection conn, string parameter, string id)
        {
            Part output = null;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToPart(reader);
            }
            reader.Close();
            return output;
        }
    }
}
