using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Npgsql;
using System.Collections.Generic;
using Npgsql;
using System.Collections.Generic;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    internal class PackageRepository
    {
        private DBAccess _dbAccess { get; set; }
        public PackageRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
        }
        private UserRepository userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");


        //Get count of packages in DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal int GetPackageCount()
        {
            try
            {
                 return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM packages", conn))
                {
                    // Safely convert the result to int
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }


        //Insert JSON content in packages table
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void InsertJsonPackagesIntoDatabase(string jsonData)
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO packages (package_content) VALUES (@json::JSONB)", conn))
                    {
                        cmd.Parameters.AddWithValue("@json", jsonData);
                        return cmd.ExecuteNonQuery(); // Rückgabewert ist int
                    }
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        //Remove first package from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveFirstPackage()
        {
            try
            {
                _dbAccess.ExecuteQuery<int>(conn =>
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM packages WHERE package_id IN (SELECT package_id FROM packages ORDER BY package_id ASC LIMIT 1)", conn))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        //Get first package from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public (string PackageId, string PackageContent) GetFirstPackage()
        {
            try
            {
                return _dbAccess.ExecuteQuery(conn =>
                {
                    using (var cmd = new NpgsqlCommand(
                               "SELECT package_id, package_content FROM packages ORDER BY package_id ASC LIMIT 1",
                               conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string packageId = reader.GetString(reader.GetOrdinal("package_id"));
                                string packageContent = reader.GetString(reader.GetOrdinal("package_content"));
                                return (PackageId: packageId, PackageContent: packageContent);
                            }
                            else
                            {
                                return (PackageId: null,
                                    PackageContent: null); // or throw an exception if a row must exist
                            }
                        }
                    }
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return (PackageId: null, PackageContent: null);
            }
 
        }




        //Move cards from packages Table in card_stacks table after acquirement
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DeserializeAndInsertCards(string packageContent, int userId)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cards = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(packageContent, options);

            foreach (var card in cards)
            {
                try
                {
                    // Insert each card into the cards_table
                    _dbAccess.ExecuteQuery<int>(conn =>
                    {
                        using (var cmd = new NpgsqlCommand(
                                   "INSERT INTO card_stacks (stack_id,unique_id, name, damage) VALUES (@stackId,@id, @name, @damage)",
                                   conn))
                        {
                            cmd.Parameters.AddWithValue("@stackId", userRepository.getStackIsByUserId(userId));
                            cmd.Parameters.AddWithValue("@id", card["Id"].GetString());
                            cmd.Parameters.AddWithValue("@name", card["Name"].GetString());
                            cmd.Parameters.AddWithValue("@damage",
                                card["Damage"].GetDouble()); // Use GetDouble for JsonElement
                            return cmd.ExecuteNonQuery();
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
