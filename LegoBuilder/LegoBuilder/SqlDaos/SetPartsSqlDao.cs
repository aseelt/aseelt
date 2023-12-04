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
using System.Drawing;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace LegoBuilder.SqlDaos
{
    public class SetPartsSqlDao : BaseSqlDao, ISetPartsSqlDao
    {
        public SetPartsSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }

        // gets the info from the setparts table
        // never going to ask for all sets out of the database, right?
        public SetParts GetSetPartsBySetNum(string setNum)
        {
            CheckString(setNum);

            SetParts output = null;

            string sql = "SELECT sets_parts_id, set_num, id, inv_part_id, part_num, colour_id, quantity, is_spare, element_id, is_active, lb_creation_date, lb_update_date " +
                "FROM sets_parts " +
                "WHERE set_num = @setNum;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    output = GetSetPartsInUsingBlock(sql, conn, "@setNum", setNum);
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

        public string GetRandomSetNumber()
        {
            string output = null;

            // get the total count of unique sets_parts

            string setsCount = "SELECT COUNT(DISTINCT set_num) as sets " +
                "FROM sets_parts; ";

            int count = 0;

            // give me a random set number from one of those rows
            string setNumberSql = "SELECT * FROM ( " +
                "SELECT " +
                "ROW_NUMBER() OVER(ORDER BY set_num ASC) AS rownumber, " +
                "set_num " +
                "FROM sets_parts " +
                "GROUP BY set_num) AS foo " +
                "WHERE rownumber = @rowNumber";


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand countCmd = new SqlCommand(setsCount, conn);
                    SqlDataReader readerCount = countCmd.ExecuteReader();

                    if (readerCount.Read())
                    {
                        count = Convert.ToInt32(readerCount["sets"]);
                    }
                    readerCount.Close();

                    Random rnd = new Random();
                    int rowNumber = rnd.Next(1, count);

                    SqlCommand setNumberCmd = new SqlCommand(setNumberSql, conn);
                    setNumberCmd.Parameters.AddWithValue(@"rowNumber", rowNumber);

                    SqlDataReader setNumberReader = setNumberCmd.ExecuteReader();

                    if (setNumberReader.Read())
                    {
                        output = Convert.ToString(setNumberReader["set_num"]);
                    }
                    setNumberReader.Close();

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
        public List<FullSetInfo> WildcardSearchAllFieldsSetParts(string searchId)
        {
            searchId = SearchStringWildcardAdder(searchId);
            List<FullSetInfo> output = new List<FullSetInfo>();

            // finds sets and their parts based on their number or name
            // finds sets and their parts based on their theme
            // finds all sets that contain the part number or part name (e.g. multiple colours)
            // finds all sets and their parts based on the categories
            // finds all sets based on the colour
            // think I should have a more specific version of this search that is broken up to search by related fields
            string sql = "SELECT sets_parts_id, sets_parts.set_num, sets_parts.is_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, sets.year AS year_released, sets.num_parts AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM sets_parts " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE sets_parts.set_num LIKE @searchId OR sets.name LIKE @searchId OR themes.name LIKE @searchId OR sets_parts.part_num LIKE @searchId OR parts.name LIKE @searchId " +
                "OR part_categories.name LIKE @searchId OR colours.name LIKE @searchId " +
                "ORDER BY sets_parts_id;";


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"searchId", searchId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<string> includedSets = new List<string>();

                    while (reader.Read())
                    {
                        // get the row mapped
                        FullSetInfo newSet = MapRowToFullSetInfo(reader);

                        // if the set number is in the output, add the part only
                        if (includedSets.Contains(newSet.Set_Num))
                        {
                            int indexOfSet = includedSets.IndexOf(newSet.Set_Num);
                            output[indexOfSet].Parts.Add(newSet.Parts[0]);
                            //Debug.WriteLine($"Added {newSet.Parts[0].Part_Name} to {output[indexOfSet].Set_Num}");
                        }
                        else
                        {
                            // if not, add the set and part
                            output.Add(newSet);
                            // update the includedsets list
                            includedSets.Add(newSet.Set_Num);
                            //Debug.WriteLine($"Added {newSet.Set_Num} to the list");
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
            return output;
        }
        public List<FullSetInfo> WildcardSearchByFieldSetParts(string setSearch = "", string partSearch = "", string colourSearch = "")
        {
            // this functions as an or search, do we want to make an and search? how about the option? default is or
            // would mean i need to update the sql query to be "AND this, AND that"
            // tried it, couldn't get it to work, oh well
            setSearch = SearchStringWildcardAdder(setSearch);
            partSearch = SearchStringWildcardAdder(partSearch);
            colourSearch = SearchStringWildcardAdder(colourSearch);


            List<FullSetInfo> output = new List<FullSetInfo>();

            string sql = "SELECT sets_parts_id, sets_parts.set_num, sets_parts.is_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, sets.year AS year_released, sets.num_parts AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM sets_parts " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE sets_parts.set_num LIKE @setSearch OR sets.name LIKE @setSearch OR themes.name LIKE @setSearch OR sets_parts.part_num LIKE @partSearch OR parts.name LIKE @partSearch " +
                "OR part_categories.name LIKE @partSearch OR colours.name LIKE @colourSearch " +
                "ORDER BY sets_parts_id;";



            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"setSearch", setSearch);
                    cmd.Parameters.AddWithValue(@"partSearch", partSearch);
                    cmd.Parameters.AddWithValue(@"colourSearch", colourSearch);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<string> includedSets = new List<string>();

                    while (reader.Read())
                    {
                        // get the row mapped
                        FullSetInfo newSet = MapRowToFullSetInfo(reader);

                        // if the set number is in the output, add the part only
                        if (includedSets.Contains(newSet.Set_Num))
                        {
                            int indexOfSet = includedSets.IndexOf(newSet.Set_Num);
                            output[indexOfSet].Parts.Add(newSet.Parts[0]);
                            //Debug.WriteLine($"Added {newSet.Parts[0].Part_Name} to {output[indexOfSet].Set_Num}");
                        }
                        else
                        {
                            // if not, add the set and part
                            output.Add(newSet);
                            // update the includedsets list
                            includedSets.Add(newSet.Set_Num);
                            //Debug.WriteLine($"Added {newSet.Set_Num} to the list");
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
            return output;
        }
        // inactivation/reactivation of setparts is handled in the setsqldao, it should happen at the same time

        public FullSetInfo GetFullSetInfo(string inputSetNum)
        {

            CheckString(inputSetNum);

            string setNum = SetNumChecker(inputSetNum);

            FullSetInfo output = new FullSetInfo();

            string sql = "SELECT sets_parts_id, sets_parts.set_num, sets_parts.is_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, sets.year AS year_released, sets.num_parts AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM sets_parts " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE sets_parts.set_num = @setNum " +
                "ORDER BY sets_parts_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"setNum", setNum);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // counter - do the common properties once
                    // then update the list the first time and all the other times
                    // first time, capture all the properties
                    // subsequent times, check if it matches
                    // if it doesn't throw exception
                    bool startToggle = true; ;

                    while (reader.Read())
                    {
                        if (startToggle)
                        {
                            output = MapRowToFullSetInfo(reader);
                            startToggle = false;
                        }
                        else
                        {
                            FullSetInfo newPart = MapRowToFullSetInfo(reader);

                            // not sure this will ever be the case but why not put this in
                            if (newPart.Set_Num != output.Set_Num ||
                                newPart.Is_Active != output.Is_Active ||
                                newPart.Set_Name != output.Set_Name ||
                                newPart.Year_Released != output.Year_Released ||
                                newPart.Total_Num_Parts != output.Total_Num_Parts ||
                                newPart.Set_Img_Url != output.Set_Img_Url ||
                                newPart.Set_RB_Last_Modified != output.Set_RB_Last_Modified ||
                                newPart.Set_Theme != output.Set_Theme)
                            {
                                throw new DaoException("There is a mismatch in this set's part information");
                            }
                            else
                            {
                                output.Parts.Add(newPart.Parts[0]);
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this set's part information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
            return output;
        }

        public SetParts UpdateDatabase(SetParts setPart)
        {
            if (setPart == null)
            {
                throw new IncorrectEntryException("Please provide a valid set part");
            }

            SetParts output = new SetParts();
            SetParts addedUpdatedNoChange = new SetParts();

            SetParts currentSetPart = new SetParts();

            int newSetPartId = 0;
            int updatedRows = 0;

            string sqlGetBySetNum = "SELECT sets_parts_id, set_num, id, inv_part_id, part_num, colour_id, quantity, is_spare, element_id, is_active, lb_creation_date, lb_update_date " +
                "FROM sets_parts " +
                "WHERE set_num = @setNum;";

            string sqlGetByResultsSetPartsId = "SELECT sets_parts_id, set_num, id, inv_part_id, part_num, colour_id, quantity, is_spare, element_id, is_active, lb_creation_date, lb_update_date " +
                "FROM sets_parts " +
                "WHERE id = @id;";

            string sqlAddToDatabse = "INSERT INTO sets_parts (set_num, id, inv_part_id, part_num, colour_id, quantity, is_spare, element_id) " +
                "OUTPUT inserted.sets_parts_id " +
                "VALUES (@set_num, @id, @inv_part_id, @part_num, @colour_id, @quantity, @is_spare, @element_id);";

            string sqlUpdateDatabase = "UPDATE sets_parts " +
                "SET inv_part_id = @inv_part_id, part_num = @part_num, colour_id = @colour_id, quantity = @quantity, is_spare = @is_spare, element_id = @element_id, lb_update_date = @lb_update_date " +
                "WHERE id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // doing it one by one, so no need for a foreach loop
                    currentSetPart = GetSetPartsInUsingBlock(sqlGetBySetNum, conn, "@setNum", setPart.Set_Num);

                    if (currentSetPart.Results.Count == 0)
                    {
                        SetParts newSetParts = new SetParts();

                        // but need to do a foreach for each of the parts in the results list
                        foreach (ResultsSetParts item in setPart.Results)
                        {
                            try
                            {
                            SqlCommand cmdAddSetPart = AddParameters(sqlAddToDatabse, conn, item);
                            cmdAddSetPart.Parameters.AddWithValue(@"set_num", item.Set_Num);

                            Convert.ToInt32(cmdAddSetPart.ExecuteScalar());
                            addedUpdatedNoChange.Added_Updated.Add(item.Id.ToString(), "Added");
                            }
                            catch (Exception)
                            {
                                AllSetPartsLoadingLog.PartLoadError(item);
                            }

                        }

                        // singular output of the whole thing created
                        CreationUpdateLog.WriteLog("Created & Updated", "Set Parts", $"Success! {setPart.Set_Num} and all of its parts were added");
                        output.Added_Updated[setPart.Set_Num] = "The full set and all its parts have been added";
                        return output;
                    }
                    else
                    {
                        // if the set is in the set parts table, then need to check if the individual result is matching
                        // so for each result in the setpart, pull it out, find it in the database
                        // if not found, add
                        // if found check if it's the same - if not update
                        foreach (ResultsSetParts item in setPart.Results)
                        {
                            ResultsSetParts checkedResultsSetPart = GetResultsSetPartsInUsingBlock(sqlGetByResultsSetPartsId, conn, "@id", item.Id);

                            if (checkedResultsSetPart == null)
                            {
                                try
                                {
                                    SqlCommand cmdAddSetPart = AddParameters(sqlAddToDatabse, conn, item);
                                    cmdAddSetPart.Parameters.AddWithValue(@"set_num", item.Set_Num);

                                    Convert.ToInt32(cmdAddSetPart.ExecuteScalar());
                                    addedUpdatedNoChange.Added_Updated[item.Id.ToString()] = "Added";
                                }
                                catch (Exception)
                                {
                                    AllSetPartsLoadingLog.PartLoadError(item);
                                }
                                
                                CreationUpdateLog.WriteLog("Created & Updated", "Set Parts", $"Success! Set Part Element {item.Element_Id}, " +
                                    $"belonging to {item.Set_Num} was missing in the SetParts table and was added");
                            }
                            else if (CheckObjectMatch(item, checkedResultsSetPart))
                            {
                                try
                                {
                                    SqlCommand cmdUpdateSetPart = AddParameters(sqlUpdateDatabase, conn, item);
                                    cmdUpdateSetPart.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                                    updatedRows = cmdUpdateSetPart.ExecuteNonQuery();
                                    if (updatedRows != 1)
                                    {
                                        throw new DaoException("Something went wrong and the wrong number of rows were updated");
                                    }
                                    addedUpdatedNoChange.Added_Updated[item.Id.ToString()] = "Updated";
                                }
                                catch (Exception)
                                {
                                    AllSetPartsLoadingLog.PartLoadError(item);
                                }
                                
                                CreationUpdateLog.WriteLog("Created & Updated", "Set Parts", $"Success! Set Part Element {item.Element_Id}, " +
                                    $"belonging to {item.Set_Num} was updated in the SetParts table");
                            }
                            else
                            {
                                addedUpdatedNoChange.Added_Updated[item.Id.ToString()] = "No Change";
                            }
                        }
                        output = GetSetPartsInUsingBlock(sqlGetBySetNum, conn, "@setNum", setPart.Set_Num);
                        output.Added_Updated = addedUpdatedNoChange.Added_Updated;
                        return output;
                    }

                }
            }
            catch (SqlException sqe)
            {
                CreationUpdateLog.WriteLog("Sql Error", "Set Parts", $"Something went wrong adding/updating {setPart.Set_Num} in the Set Parts table. {sqe.Message}");
                throw new DaoException("Sql Exception occurred");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error");
            }
        }

        // need two - one that returns just the setparts table info
        // use this primarily for add/delete - user shouldn't get this
        // another that returns the set parts combined with other stuff
        private ResultsSetParts MapRowToSetParts(SqlDataReader reader)
        {
            ResultsSetParts output = new ResultsSetParts();
            output.Sets_Parts_Id = Convert.ToInt32(reader["sets_parts_id"]);
            output.Set_Num = Convert.ToString(reader["set_num"]);
            output.Id = Convert.ToInt32(reader["id"]);
            output.Inv_Part_Id = Convert.ToInt32(reader["inv_part_id"]);
            output.Part.Part_Num = Convert.ToString(reader["part_num"]);
            output.Color.Id = Convert.ToString(reader["colour_id"]);
            output.Quantity = Convert.ToInt32(reader["quantity"]);
            output.Is_Spare = Convert.ToBoolean(reader["is_spare"]);
            output.Element_Id = Convert.ToString(reader["element_id"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }

        private FullSetInfo MapRowToFullSetInfo(SqlDataReader reader)
        {
            FullSetInfo output = new FullSetInfo();
            FullPartInfo part = new FullPartInfo();
            output.Parts.Add(part);

            output.Sets_Parts_Id = Convert.ToInt32(reader["sets_parts_id"]);
            output.Set_Num = Convert.ToString(reader["set_num"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            output.Set_Name = Convert.ToString(reader["set_name"]);
            output.Year_Released = Convert.ToInt32(reader["year_released"]);
            output.Total_Num_Parts = Convert.ToInt32(reader["total_num_parts"]);
            output.Set_Img_Url = Convert.ToString(reader["set_img_url"]);
            output.Set_RB_Last_Modified = Convert.ToDateTime(reader["set_rb_last_modified"]);

            output.Instructions_Url = Convert.ToString(reader["instructions_url"]);
            output.Set_Theme = Convert.ToString(reader["set_theme"]);

            output.Parts[0].Part_Num = Convert.ToString(reader["part_num"]);
            output.Parts[0].Part_Name = Convert.ToString(reader["part_name"]);
            output.Parts[0].Element_Id = Convert.ToString(reader["element_id"]);
            output.Parts[0].Part_Quantity = Convert.ToInt32(reader["part_quantity"]);
            output.Parts[0].Part_Name = Convert.ToString(reader["part_name"]);
            output.Parts[0].Part_Cat_Name = Convert.ToString(reader["part_cat_name"]);
            output.Parts[0].Part_Url = Convert.ToString(reader["part_url"]);
            output.Parts[0].Part_Img_Url = Convert.ToString(reader["part_img_url"]);

            output.Parts[0].Colour = Convert.ToString(reader["colour"]);
            output.Parts[0].RGB = Convert.ToString(reader["rgb"]);
            output.Parts[0].Is_Trans = Convert.ToBoolean(reader["is_trans"]);

            return output;
        }

        private bool CheckObjectMatch(ResultsSetParts toCheck, ResultsSetParts inDatabase)
        {
            // if all the same, then true, if one different, then false and go into update code block
            // assumes Id is not changing in the database
            if (toCheck.Set_Num != inDatabase.Set_Num ||
                toCheck.Id != inDatabase.Id ||
                toCheck.Inv_Part_Id != inDatabase.Inv_Part_Id ||
                toCheck.Part.Part_Num != inDatabase.Part.Part_Num ||
                toCheck.Color.Id != inDatabase.Color.Id ||
                toCheck.Quantity != inDatabase.Quantity ||
                toCheck.Is_Spare != inDatabase.Is_Spare ||
                toCheck.Element_Id != inDatabase.Element_Id)
            {
                return true;
            }
            return false;
        }

        private SetParts GetSetPartsInUsingBlock(string query, SqlConnection conn, string parameter, string id)
        {
            SetParts output = new SetParts();
            SqlCommand cmd = new SqlCommand(query, conn);
            // handles the creation of all set parts - sometimes there isn't an id supplied
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ResultsSetParts newPart = MapRowToSetParts(reader);
                output.Results.Add(newPart);
            }
            reader.Close();
            return output;
        }

        private ResultsSetParts GetResultsSetPartsInUsingBlock(string query, SqlConnection conn, string parameter, int id)
        {
            ResultsSetParts output = null;
            SqlCommand cmd = new SqlCommand(query, conn);

            // handles the creation of all set parts - sometimes an id is not supplied
            cmd.Parameters.AddWithValue(parameter, id);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                output = MapRowToSetParts(reader);
            }
            reader.Close();
            return output;
        }

        private SqlCommand AddParameters(string sql, SqlConnection conn, ResultsSetParts item)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue(@"id", item.Id);
            cmd.Parameters.AddWithValue(@"inv_part_id", item.Inv_Part_Id);
            cmd.Parameters.AddWithValue(@"part_num", item.Part.Part_Num);
            cmd.Parameters.AddWithValue(@"colour_id", item.Color.Id);
            cmd.Parameters.AddWithValue(@"quantity", item.Quantity);
            cmd.Parameters.AddWithValue(@"is_spare", item.Is_Spare);
            cmd.Parameters.AddWithValue(@"element_id", item.Element_Id is null ? "" : item.Element_Id);
            return cmd;
        }


    }
}
