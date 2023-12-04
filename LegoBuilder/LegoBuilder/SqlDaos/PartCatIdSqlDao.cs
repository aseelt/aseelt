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
    public class PartCatIdSqlDao : BaseSqlDao, IPartCatIdSqlDao
    {
        public PartCatIdSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }


        public PartCatId GetPartCatIdById(string id)
        {
            CheckString(id);

            PartCatId output;

            string sql = "SELECT part_categories_id, id, name, part_count, is_active, lb_creation_date, lb_update_date " +
                "FROM part_categories " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    output = GetPartCatIdInUsingBlock(sql, conn, "@id", id);
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
        public List<PartCatId> WildcardSearchPartCats(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<PartCatId> output = new List<PartCatId>();

            string sql = "SELECT part_categories_id, id, name, part_count, is_active, lb_creation_date, lb_update_date, " +
                "FROM part_categories " +
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
                        PartCatId newRow = MapRowToPartCatId(reader);
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
        public List<PartCatId> GetAllPartCatIds()
        {
            List<PartCatId> output = new List<PartCatId>();

            string sql = "SELECT part_categories_id, id, name, part_count, is_active, lb_creation_date, lb_update_date " +
                "FROM part_categories " +
                "ORDER BY part_categories_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PartCatId newRow = MapRowToPartCatId(reader);
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

        public string DeletePartCatIds(string id)
        {
            CheckString(id);

            string sql = "UPDATE part_categories " +
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

            return $"Part Category Id {id} has been inactivated";
        }

        public string UnDeletePartCatIds(string id)
        {
            CheckString(id);

            string sql = "UPDATE part_categories " +
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

            return $"Part Category Id {id} has been inactivated";
        }

        public AllPartCats UpdateDatabase(AllPartCats allPartCats)
        {
            CheckCount(allPartCats.Results.Count);

            AllPartCats output = new AllPartCats();

            PartCatId currentPartCatId = new PartCatId();

            int newPartCatIdId = 0;
            int updatedRows = 0;

            string sqlGetById = "SELECT part_categories_id, id, name, part_count, is_active, lb_creation_date, lb_update_date " +
                "FROM part_categories " +
                "WHERE id = @id;";

            string sqlGetByPartCatIdId = "SELECT part_categories_id, id, name, part_count, is_active, lb_creation_date, lb_update_date " +
                "FROM part_categories " +
                "WHERE part_categories_id = @part_categories_id;";

            string sqlAddToDatabse = "INSERT INTO part_categories (id, name, part_count) " +
                "OUTPUT inserted.part_categories_id " +
                "VALUES (@id, @name, @part_count);";

            string sqlUpdateDatabase = "UPDATE part_categories " +
                "SET name = @name, part_count = @part_count, lb_update_date = @lb_update_date " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // checks the database for the colour
                    foreach (PartCatId partCatId in allPartCats.Results)
                    {
                        currentPartCatId = GetPartCatIdInUsingBlock(sqlGetById, conn, "@id", partCatId.Id.ToString());

                        if (currentPartCatId == null)
                        {
                            // add to the database
                            // add to list of added colours
                            PartCatId newPartCatId = new PartCatId();

                            SqlCommand cmdAddPartCatId = new SqlCommand(sqlAddToDatabse, conn);
                            cmdAddPartCatId.Parameters.AddWithValue(@"id", partCatId.Id);
                            cmdAddPartCatId.Parameters.AddWithValue(@"name", partCatId.Name);
                            cmdAddPartCatId.Parameters.AddWithValue(@"part_count", partCatId.Part_Count);

                            newPartCatIdId = Convert.ToInt32(cmdAddPartCatId.ExecuteScalar());

                            newPartCatId = GetPartCatIdInUsingBlock(sqlGetByPartCatIdId, conn, "@part_categories_id", newPartCatIdId.ToString());

                            output.Added_Updated[newPartCatId.Id.ToString()] = "Added";
                        }
                        else if (CheckObjectMatch(partCatId, currentPartCatId))
                        {
                            // assert fail
                            // update database
                            // update updated column
                            // add ot list of updated colours
                            PartCatId updatedPartCatId = new PartCatId();

                            SqlCommand cmdUpdatedPartCatId = new SqlCommand(sqlUpdateDatabase, conn);
                            cmdUpdatedPartCatId.Parameters.AddWithValue(@"name", partCatId.Name);
                            cmdUpdatedPartCatId.Parameters.AddWithValue(@"part_count", partCatId.Part_Count);
                            cmdUpdatedPartCatId.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);
                            cmdUpdatedPartCatId.Parameters.AddWithValue(@"id", partCatId.Id);

                            updatedRows = cmdUpdatedPartCatId.ExecuteNonQuery();

                            if (updatedRows != 1)
                            {
                                throw new DaoException("Something went wrong and the wrong number of rows were updated");
                            }

                            updatedPartCatId = GetPartCatIdInUsingBlock(sqlGetById, conn, "@id", partCatId.Id.ToString());

                            output.Added_Updated[updatedPartCatId.Id.ToString()] = "Updated";
                        }
                    }

                }
            }
            catch (SqlException sqe)
            {
                CreationUpdateLog.WriteLog("Sql Error", "PartCatIds", $"{output.Added_Updated.Count} records were actioned before failure. {sqe.Message}");
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            output.Count = output.Added_Updated.Count;
            if (output.Count == 0)
            {
                CreationUpdateLog.WriteLog("Created & Updated", "PartCatIds", $"Zero records were actioned");
            }
            else
            {
                CreationUpdateLog.WriteLog("Created & Updated", "PartCatIds", $"Success! {output.Added_Updated.Count} records were added/updated", output.Added_Updated);
            }
            return output;
        }

        private PartCatId MapRowToPartCatId(SqlDataReader reader)
        {
            PartCatId output = new PartCatId();
            output.Part_Categories_Id = Convert.ToInt32(reader["part_categories_id"]);
            output.Id = Convert.ToInt32(reader["id"]);
            output.Name = Convert.ToString(reader["name"]);
            output.Part_Count = Convert.ToInt32(reader["part_count"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }

        private bool CheckObjectMatch(PartCatId toCheck, PartCatId inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Name != inDatabase.Name ||
                toCheck.Part_Count != inDatabase.Part_Count)
            {
                return true;
            }
            return false;
        }

        private PartCatId GetPartCatIdInUsingBlock(string query, SqlConnection conn, string parameter, string id)
        {
            PartCatId output = null;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToPartCatId(reader);
            }
            reader.Close();
            return output;
        }
    }
}
