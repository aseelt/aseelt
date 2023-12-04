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
using System.Diagnostics;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LegoBuilder.SqlDaos
{
    public class UserSetsSqlDao : BaseSqlDao, IUserSetsSqlDao
    {
        private const int DefaultStartingQuantity = 1;
        private const bool Favourite = true;
        private const bool UnFavourite = false;
        public UserSetsSqlDao(string dbConnectionString) : base(dbConnectionString)
        {
        }

        // won't ever need to get just the columns out of user_sets
        // but can get the full user's set information in one object, why not
        // TODO pagination
        public UserFullInfo GetFullInfoByUsername(string username)
        {
            CheckString(username);

            UserFullInfo output = new UserFullInfo();

            string sql = "SELECT username, users.is_active AS user_active, " +
                "sets_parts.set_num, is_favourite, sets_parts.is_active AS set_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts * users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity * users_sets.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE users.username = @username " +
                "ORDER BY sets_parts.set_num, sets_parts.part_num; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    bool startToggle = true;
                    List<string> includedSets = new List<string>();

                    while (reader.Read())
                    {
                        if (startToggle)
                        {
                            output = MapRowToFullSetInfo(reader);
                            includedSets.Add(output.Sets[0].Set_Num);
                            startToggle = false;
                        }
                        else
                        {
                            UserFullInfo newSet = MapRowToFullSetInfo(reader);

                            // if the set number is in the output, add the part
                            if (includedSets.Contains(newSet.Sets[0].Set_Num))
                            {
                                int indexOfSet = includedSets.IndexOf(newSet.Sets[0].Set_Num);
                                output.Sets[indexOfSet].Parts.Add(newSet.Sets[0].Parts[0]);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Parts[0].Part_Name} to {output.Sets[indexOfSet].Set_Num}");
                            }
                            else
                            {
                                // if not, add the set
                                output.Sets.Add(newSet.Sets[0]);
                                // update the includedsets list
                                includedSets.Add(newSet.Sets[0].Set_Num);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Set_Num} to the list");
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
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }
        public UserFullInfo WildcardSearchAllFieldsUsersSets(string username, string searchId)
        {
            CheckString(username);
            searchId = SearchStringWildcardAdder(searchId);
            UserFullInfo output = new UserFullInfo();
            
            string sql = "SELECT username, users.is_active AS user_active, " +
                "sets_parts.set_num, is_favourite, sets_parts.is_active AS set_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts * users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity * users_sets.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE users.username = @username AND (sets_parts.set_num LIKE @searchId OR sets.name LIKE @searchId OR themes.name LIKE @searchId OR sets_parts.part_num LIKE @searchId OR parts.name LIKE @searchId " +
                "OR part_categories.name LIKE @searchId OR colours.name LIKE @searchId) " +
                "ORDER BY sets_parts.set_num, sets_parts.part_num;";


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"searchId", searchId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    bool startToggle = true;
                    List<string> includedSets = new List<string>();

                    while (reader.Read())
                    {
                        if (startToggle)
                        {
                            output = MapRowToFullSetInfo(reader);
                            includedSets.Add(output.Sets[0].Set_Num);
                            startToggle = false;
                        }
                        else
                        {
                            UserFullInfo newSet = MapRowToFullSetInfo(reader);

                            // if the set number is in the output, add the part
                            if (includedSets.Contains(newSet.Sets[0].Set_Num))
                            {
                                int indexOfSet = includedSets.IndexOf(newSet.Sets[0].Set_Num);
                                output.Sets[indexOfSet].Parts.Add(newSet.Sets[0].Parts[0]);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Parts[0].Part_Name} to {output.Sets[indexOfSet].Set_Num}");
                            }
                            else
                            {
                                // if not, add the set
                                output.Sets.Add(newSet.Sets[0]);
                                // update the includedsets list
                                includedSets.Add(newSet.Sets[0].Set_Num);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Set_Num} to the list");
                            }
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

        public UserFullInfo WildcardSearchByFieldUsersSets(string username, string setSearch = "", string partSearch = "", string colourSearch = "")
        {
            CheckString(username);
            setSearch = SearchStringWildcardAdder(setSearch);
            partSearch = SearchStringWildcardAdder(partSearch);
            colourSearch = SearchStringWildcardAdder(colourSearch);
            UserFullInfo output = new UserFullInfo();

            string sql = "SELECT username, users.is_active AS user_active, " +
                "sets_parts.set_num, is_favourite, sets_parts.is_active AS set_active, sets_parts.lb_creation_date, sets_parts.lb_update_date, " +
                "sets.name AS set_name, users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts * users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme, " +
                "sets_parts.part_num, element_id, sets_parts.quantity * users_sets.quantity AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN sets ON sets_parts.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE users.username = @username AND (sets_parts.set_num LIKE @setSearch OR sets.name LIKE @setSearch OR themes.name LIKE @setSearch OR sets_parts.part_num LIKE @partSearch OR parts.name LIKE @partSearch " +
                "OR part_categories.name LIKE @partSearch OR colours.name LIKE @colourSearch) " +
                "ORDER BY sets_parts.set_num, sets_parts.part_num;";


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"setSearch", setSearch);
                    cmd.Parameters.AddWithValue(@"partSearch", partSearch);
                    cmd.Parameters.AddWithValue(@"colourSearch", colourSearch);

                    SqlDataReader reader = cmd.ExecuteReader();

                    bool startToggle = true;
                    List<string> includedSets = new List<string>();

                    while (reader.Read())
                    {
                        if (startToggle)
                        {
                            output = MapRowToFullSetInfo(reader);
                            includedSets.Add(output.Sets[0].Set_Num);
                            startToggle = false;
                        }
                        else
                        {
                            UserFullInfo newSet = MapRowToFullSetInfo(reader);

                            // if the set number is in the output, add the part
                            if (includedSets.Contains(newSet.Sets[0].Set_Num))
                            {
                                int indexOfSet = includedSets.IndexOf(newSet.Sets[0].Set_Num);
                                output.Sets[indexOfSet].Parts.Add(newSet.Sets[0].Parts[0]);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Parts[0].Part_Name} to {output.Sets[indexOfSet].Set_Num}");
                            }
                            else
                            {
                                // if not, add the set
                                output.Sets.Add(newSet.Sets[0]);
                                // update the includedsets list
                                includedSets.Add(newSet.Sets[0].Set_Num);
                                //Debug.WriteLine($"Added {newSet.Sets[0].Set_Num} to the list");
                            }
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

        public List<UserSetInfo> GetSetOnlyInfoByUsername(string username)
        {
            CheckString(username);

            List<UserSetInfo> output = new List<UserSetInfo>();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "sets.set_num, is_favourite, sets.is_active AS set_active, sets.lb_creation_date, sets.lb_update_date, sets.name AS set_name, " +
                "users_sets.quantity AS set_quantity, sets.year AS year_released, sets.num_parts* users_sets.quantity AS total_num_parts, sets.set_img_url, last_modified_dt AS set_rb_last_modified, " +
                "url AS instructions_url, " +
                "themes.name AS set_theme " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "JOIN sets ON users_sets.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "WHERE users.username = @username " +
                "ORDER BY sets.set_num; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        UserSetInfo setInfo = MapSetInfo(reader);
                        setInfo.Username = user.Username;
                        setInfo.User_Active = user.User_Active;
                        UserThemeInfo theme = MapUserThemeInfo(reader);
                        setInfo.Set_Theme = theme.Set_Theme;
                        setInfo.Set_Quantity = theme.Set_Quantity;
                        setInfo.Total_Num_Parts = theme.Total_Num_Parts;
                        output.Add(setInfo);
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this set's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public List<UserPartColourInfo> GetPartAndColourInfoByUsername(string username)
        {
            CheckString(username);

            List<UserPartColourInfo> output = new List<UserPartColourInfo>();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "sets_parts.part_num, element_id, SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active, sets_parts.part_num, element_id, parts.name, part_categories.name, part_url, part_img_url,colours.name, rgb, is_trans " +
                "ORDER BY element_id; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UserPartColourInfo partColour = MapPartColourInfo(reader);
                        UserBase user = MapUserBaseInfo(reader);
                        partColour.Username = user.Username;
                        partColour.User_Active = user.User_Active;

                        partColour.Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;
                        partColour.Part_Cat_Name = MapPartCatInfo(reader).Part_Cat_Name;

                        UserPartInfo part = MapPartInfo(reader);
                        partColour.Part_Num = part.Part_Num;
                        partColour.Part_Name = part.Part_Name;
                        partColour.Part_Url = part.Part_Url;
                        partColour.Part_Img_Url = part.Part_Img_Url;

                        UserColourInfo colour = MapColourInfo(reader);
                        partColour.Colour = colour.Colour;
                        partColour.RGB = colour.RGB;
                        partColour.Is_Trans = colour.Is_Trans;

                        output.Add(partColour);
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this part and colour information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public List<UserPartInfo> GetPartOnlyInfoByUsername(string username)
        {
            CheckString(username);

            List<UserPartInfo> output = new List<UserPartInfo>();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "sets_parts.part_num, SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, parts.name AS part_name, part_categories.name AS part_cat_name, part_url, part_img_url " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active, sets_parts.part_num, parts.name, part_categories.name, part_url, part_img_url " +
                "ORDER BY sets_parts.part_num; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        UserPartInfo part = MapPartInfo(reader);
                        part.Username = user.Username;
                        part.User_Active = user.User_Active;

                        part.Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;
                        part.Part_Cat_Name = MapPartCatInfo(reader).Part_Cat_Name;
                        output.Add(part);
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this part's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public List<UserColourInfo> GetColourOnlyInfoByUsername(string username)
        {
            CheckString(username);

            List<UserColourInfo> output = new List<UserColourInfo>();

            string sql = "SELECT username, users.is_active AS user_active, " +
                "SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, " +
                "colours.name AS colour, rgb, is_trans " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN colours ON sets_parts.colour_id = colours.id " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active, colours.name, rgb, is_trans " +
                "ORDER BY colours.name; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        UserColourInfo colour = MapColourInfo(reader);
                        colour.Username = user.Username;
                        colour.User_Active = user.User_Active;
                        colour.Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;

                        output.Add(colour);
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this colour's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public List<UserPartCatInfo> GetPartCategoriesInfoByUsername(string username)
        {
            CheckString(username);

            List<UserPartCatInfo> output = new List<UserPartCatInfo>();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity, part_categories.name AS part_cat_name " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "JOIN part_categories ON parts.part_cat_id = part_categories.id " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active, part_categories.name " +
                "ORDER BY part_categories.name; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        UserPartCatInfo partCat = MapPartCatInfo(reader);
                        partCat.Username = user.Username;
                        partCat.User_Active = user.User_Active;
                        partCat.Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;
                        output.Add(partCat);
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this part category's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public List<UserThemeInfo> GetThemesInfoByUsername(string username)
        {
            CheckString(username);

            List<UserThemeInfo> output = new List<UserThemeInfo>();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "SUM(users_sets.quantity) AS set_quantity, SUM(sets.num_parts * users_sets.quantity) AS total_num_parts, " +
                "themes.name AS set_theme " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "JOIN sets ON users_sets.set_num = sets.set_num " +
                "LEFT JOIN instructions ON sets.set_num = instructions.set_id " +
                "JOIN themes ON sets.theme_id = themes.id " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active, themes.name " +
                "ORDER BY themes.name; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        UserThemeInfo theme = MapUserThemeInfo(reader);
                        theme.Username = user.Username;
                        theme.User_Active = user.User_Active;
                        output.Add(theme);
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this theme's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        public UserTotalPartInfo GetTotalPartInfoByUsername(string username)
        {
            CheckString(username);

            UserTotalPartInfo output = new UserTotalPartInfo();

            string sql = "SELECT DISTINCT username, users.is_active AS user_active, " +
                "SUM(sets_parts.quantity * users_sets.quantity) AS part_quantity " +
                "FROM users " +
                "LEFT JOIN users_sets ON users.user_id = users_sets.user_id " +
                "LEFT JOIN sets_parts ON users_sets.set_num = sets_parts.set_num " +
                "JOIN parts ON sets_parts.part_num = parts.part_num " +
                "WHERE users.username = @username " +
                "GROUP BY username, users.is_active " +
                "ORDER BY username; ";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        UserBase user = MapUserBaseInfo(reader);
                        output.Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;
                        output.Username = user.Username;
                        output.User_Active = user.User_Active;
                    }
                }

            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("I'm not sure what went wrong");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }

        // assume this is not being used outside of this class
        private UsersSets GetUsersSetsById(int id)
        {
            UsersSets output = null;

            string sql = "SELECT users_sets_id, user_id, set_num, quantity, is_favourite, is_active, lb_creation_date, lb_update_date " +
                "FROM users_sets " +
                "WHERE users_sets_id = @id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output = MapUsersSetsRow(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this user's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }
        // want the lighter one as private, the full set info should be public
        private List<UsersSets> GetUsersSetsByUsername(string username)
        {
            CheckString(username);
            List<UsersSets> output = new List<UsersSets>();

            string sql = "SELECT users_sets_id, user_id, set_num, quantity, is_favourite, is_active, lb_creation_date, lb_update_date " +
                "FROM users_sets " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username);";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UsersSets usersSet = MapUsersSetsRow(reader);
                        output.Add(usersSet);
                    }
                }
            }
            catch (SqlException)
            {
                throw new DaoException("Sql Exception occurred");
            }
            catch (DaoException)
            {
                throw new DaoException("There is a mismatch in this user's information");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return output;
        }
        public UsersSets AddNewSet(string username, string setNumber, int quantity = DefaultStartingQuantity)
        {
            CheckString(username);
            CheckString(setNumber);
            if(quantity < 1)
            {
                throw new IncorrectEntryException("You must add at least one of this set");
            }

            // check if the user has zero of the set already
            // pull the sets
            List<UsersSets> userSets = GetUsersSetsByUsername(username);

            // find the specific set
            UsersSets foundSet = userSets.Find(i => i.Set_Num == setNumber);

            // if quantity 0, it exists, so reactivate, adjust quantity, and return
            if(foundSet.Quantity == 0)
            {
                ReactivateUsersSet(username, setNumber);
                ChangeSetQuantity(username, setNumber, quantity);
                return GetUsersSetsById(foundSet.Users_Sets_Id);
            }
            // otherwise create for user
            else
            {
                UsersSets output = null;

                int newUsersSetsId = 0;

                string sql = "INSERT INTO users_sets (user_id, set_num, quantity) " +
                    "OUTPUT inserted.users_sets_id " +
                    "VALUES ( (SELECT user_id FROM users WHERE username = @username), @set_num, @quantity);";

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue(@"username", username);
                        cmd.Parameters.AddWithValue(@"set_num", setNumber);
                        cmd.Parameters.AddWithValue(@"quantity", quantity);

                        newUsersSetsId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    output = GetUsersSetsById(newUsersSetsId);
                }
                catch (SqlException)
                {
                    throw new IncorrectEntryException("Search for a valid set number to add to a user's collection");
                }
                catch (Exception e)
                {
                    throw new DaoException("There was an error" + e.Message);
                }
                return output;
            }
        }
        public string ChangeSetQuantity(string username, string setNumber, int quantity)
        {
            CheckString(username);
            CheckString(setNumber); 

            // get the user's sets
            List<UsersSets> userSets = GetUsersSetsByUsername(username);

            // find the set
            UsersSets foundSet = userSets.Find(i => i.Set_Num == setNumber);
            if (foundSet == null)
            {
                throw new IncorrectEntryException("Please enter a valid set number");
            }
            else if (foundSet.Quantity + quantity < 0)
            {
                throw new IncorrectEntryException("Your set count must be zero or greater");
            }

            // if the deduction is going to reduce this to zero, deactivate the set
            if(foundSet.Quantity == Math.Abs(quantity))
            {
                InactivateUsersSet(username, setNumber);
            }

            int rowsAffected = 0;

            string sql = "UPDATE users_sets " +
                "SET quantity = quantity + @quantity, lb_update_date = @lb_update_date " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username) AND set_num = @set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"set_num", setNumber);
                    cmd.Parameters.AddWithValue(@"quantity", quantity);
                    cmd.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                    rowsAffected = cmd.ExecuteNonQuery();
                    if(rowsAffected != 1)
                    {
                        throw new DaoException("The wrong number of rows were affected");
                    }
                }
            }
            catch (SqlException)
            {
                throw new IncorrectEntryException("Search for a valid set number to add to a user's collection");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return $"{setNumber}, belonging to {username}, had its quantity changed by {quantity}";
        }
        private string InactivateUsersSet(string username, string setNumber)
        {
            CheckString(username);
            CheckString(setNumber);
             
            int rowsAffected = 0;

            string sql = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @lb_update_date " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username) AND set_num = @set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"set_num", setNumber);
                    cmd.Parameters.AddWithValue(@"is_active", Inactive);
                    cmd.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                    rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new DaoException("The wrong number of rows were affected");
                    }
                }
            }
            catch (SqlException)
            {
                throw new IncorrectEntryException("Search for a valid set number to inactivate");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return $"{setNumber}, belonging to {username}, has been inactivated";
        }
        private string ReactivateUsersSet(string username, string setNumber)
        {
            CheckString(username);
            CheckString(setNumber);

            int rowsAffected = 0;

            string sql = "UPDATE users_sets " +
                "SET is_active = @is_active, lb_update_date = @lb_update_date " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username) AND set_num = @set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"set_num", setNumber);
                    cmd.Parameters.AddWithValue(@"is_active", Active);
                    cmd.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                    rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new DaoException("The wrong number of rows were affected");
                    }
                }
            }
            catch (SqlException)
            {
                throw new IncorrectEntryException("Search for a valid set number to activate");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return $"{setNumber}, belonging to {username}, has been activated";
        }
        public string FavouriteUsersSet(string username, string setNumber)
        {
            CheckString(username);
            CheckString(setNumber);

            int rowsAffected = 0;

            string sql = "UPDATE users_sets " +
                "SET is_favourite = @is_favourite, lb_update_date = @lb_update_date " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username) AND set_num = @set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"set_num", setNumber);
                    cmd.Parameters.AddWithValue(@"is_favourite", Favourite);
                    cmd.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                    rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new DaoException("The wrong number of rows were affected");
                    }
                }
            }
            catch (SqlException)
            {
                throw new IncorrectEntryException("Search for a valid set number to favourite");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return $"{setNumber}, belonging to {username}, has been favourited";
        }
        public string UnFavouriteUsersSet(string username, string setNumber)
        {
            CheckString(username);
            CheckString(setNumber);

            int rowsAffected = 0;

            string sql = "UPDATE users_sets " +
                "SET is_favourite = @is_favourite, lb_update_date = @lb_update_date " +
                "WHERE user_id = (SELECT user_id FROM users WHERE username = @username) AND set_num = @set_num;";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"username", username);
                    cmd.Parameters.AddWithValue(@"set_num", setNumber);
                    cmd.Parameters.AddWithValue(@"is_favourite", UnFavourite);
                    cmd.Parameters.AddWithValue(@"lb_update_date", DateTime.UtcNow);

                    rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new DaoException("The wrong number of rows were affected");
                    }
                }
            }
            catch (SqlException)
            {
                throw new IncorrectEntryException("Search for a valid set number to unfavourite");
            }
            catch (Exception e)
            {
                throw new DaoException("There was an error" + e.Message);
            }
            return $"{setNumber}, belonging to {username}, has been unfavourited";
        }
        private UserFullInfo MapRowToFullSetInfo(SqlDataReader reader)
        {
            UserFullInfo output = new UserFullInfo();
            FullSetInfo set = new FullSetInfo();
            output.Sets.Add(set);
            FullPartInfo part = new FullPartInfo();
            output.Sets[0].Parts.Add(part);

            UserBase userBase = MapUserBaseInfo(reader);
            output.Username = userBase.Username;
            output.User_Active = userBase.User_Active;

            UserThemeInfo theme = MapUserThemeInfo(reader);
            output.Sets[0].Set_Theme = theme.Set_Theme;
            output.Sets[0].Set_Quantity = theme.Set_Quantity;
            output.Sets[0].Total_Num_Parts = theme.Total_Num_Parts;

            UserSetInfo setInfo = MapSetInfo(reader);
            output.Sets[0].Set_Num = setInfo.Set_Num;
            output.Sets[0].Is_Favourite = setInfo.Is_Favourite;
            output.Sets[0].Is_Active = setInfo.Is_Active;
            output.Sets[0].LB_Creation_Date = setInfo.LB_Creation_Date;
            output.Sets[0].LB_Update_Date = setInfo.LB_Update_Date;

            output.Sets[0].Set_Name = setInfo.Set_Name;
            output.Sets[0].Year_Released = setInfo.Year_Released;
            output.Sets[0].Set_Img_Url = setInfo.Set_Img_Url;
            output.Sets[0].Set_RB_Last_Modified = setInfo.Set_RB_Last_Modified;

            output.Sets[0].Instructions_Url = setInfo.Instructions_Url;

            output.Sets[0].Parts[0].Part_Quantity = MapUserTotalPartInfo(reader).Part_Quantity;
            output.Sets[0].Parts[0].Part_Cat_Name = MapPartCatInfo(reader).Part_Cat_Name;

            UserColourInfo colour = MapColourInfo(reader);
            output.Sets[0].Parts[0].Colour = colour.Colour;
            output.Sets[0].Parts[0].RGB = colour.RGB;
            output.Sets[0].Parts[0].Is_Trans = colour.Is_Trans;

            UserPartInfo partInfo = MapPartInfo(reader);
            output.Sets[0].Parts[0].Part_Num = partInfo.Part_Num;
            output.Sets[0].Parts[0].Part_Name = partInfo.Part_Name;
            output.Sets[0].Parts[0].Part_Url = partInfo.Part_Url;
            output.Sets[0].Parts[0].Part_Img_Url = partInfo.Part_Img_Url;

            output.Sets[0].Parts[0].Element_Id = MapPartColourInfo(reader).Element_Id;

            return output;
        }
        private UserBase MapUserBaseInfo(SqlDataReader reader)
        {
            UserBase output = new UserBase();

            output.Username = Convert.ToString(reader["username"]);
            output.User_Active = Convert.ToBoolean(reader["user_active"]);

            return output;
        }
        private UserThemeInfo MapUserThemeInfo(SqlDataReader reader)
        {
            UserThemeInfo output = new UserThemeInfo();

            output.Set_Theme = Convert.ToString(reader["set_theme"]);
            output.Set_Quantity = Convert.ToInt32(reader["set_quantity"]);
            output.Total_Num_Parts = Convert.ToInt32(reader["total_num_parts"]);

            return output;
        }
        private UserSetInfo MapSetInfo(SqlDataReader reader)
        {
            UserSetInfo output = new UserSetInfo();

            output.Set_Num = Convert.ToString(reader["set_num"]);
            output.Is_Favourite = Convert.ToBoolean(reader["is_favourite"]);
            output.Is_Active = Convert.ToBoolean(reader["set_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            output.Set_Name = Convert.ToString(reader["set_name"]);
            output.Year_Released = Convert.ToInt32(reader["year_released"]);
            output.Set_Img_Url = Convert.ToString(reader["set_img_url"]);
            output.Set_RB_Last_Modified = Convert.ToDateTime(reader["set_rb_last_modified"]);

            output.Instructions_Url = Convert.ToString(reader["instructions_url"]);

            return output;
        }
        private UserTotalPartInfo MapUserTotalPartInfo(SqlDataReader reader)
        {
            UserTotalPartInfo output = new UserTotalPartInfo();

            output.Part_Quantity = Convert.ToInt32(reader["part_quantity"]);

            return output;
        }
        private UserPartCatInfo MapPartCatInfo(SqlDataReader reader)
        {
            UserPartCatInfo output = new UserPartCatInfo();

            output.Part_Cat_Name = Convert.ToString(reader["part_cat_name"]);

            return output;
        }
        private UserColourInfo MapColourInfo(SqlDataReader reader)
        {
            UserColourInfo output = new UserColourInfo();

            output.Colour = Convert.ToString(reader["colour"]);
            output.RGB = Convert.ToString(reader["rgb"]);
            output.Is_Trans = Convert.ToBoolean(reader["is_trans"]);

            return output;
        }
        private UserPartInfo MapPartInfo(SqlDataReader reader)
        {
            UserPartInfo output = new UserPartInfo();

            output.Part_Num = Convert.ToString(reader["part_num"]);
            output.Part_Name = Convert.ToString(reader["part_name"]);
            output.Part_Url = Convert.ToString(reader["part_url"]);
            output.Part_Img_Url = Convert.ToString(reader["part_img_url"]);

            return output;
        }
        private UserPartColourInfo MapPartColourInfo(SqlDataReader reader)
        {
            UserPartColourInfo output = new UserPartColourInfo();

            output.Element_Id = Convert.ToString(reader["element_id"]);

            return output;
        }
        private UsersSets MapUsersSetsRow(SqlDataReader reader)
        {
            UsersSets output = new UsersSets();

            output.Users_Sets_Id = Convert.ToInt32(reader["users_sets_id"]);
            output.User_Id = Convert.ToInt32(reader["user_id"]);
            output.Set_Num = Convert.ToString(reader["set_num"]);
            output.Quantity = Convert.ToInt32(reader["quantity"]);
            output.Is_Favourite = Convert.ToBoolean(reader["is_favourite"]);
            output.Is_Active = Convert.ToBoolean(reader["is_active"]);
            output.LB_Creation_Date = Convert.ToDateTime(reader["lb_creation_date"]);
            output.LB_Update_Date = Convert.ToDateTime(reader["lb_update_date"]);

            return output;
        }
        

    }
}
