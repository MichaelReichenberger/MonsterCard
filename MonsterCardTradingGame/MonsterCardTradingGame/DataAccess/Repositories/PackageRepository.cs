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
using MonsterCardTradingGame.BusinessLogic;
using NpgsqlTypes;

namespace MonsterCardTradingGame.DataBase.Repositories
{
    public class PackageRepository : IRepository
    {
        private DBAccess _dbAccess { get; set; }
        private Parser _parser;
        private UserRepository _userRepository;
        public PackageRepository(string connectionString)
        {
            _dbAccess = new DBAccess(connectionString);
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _parser = new Parser();
        }

        public int GetFirstId()
        {
            throw new NotImplementedException();
        }

        public int GetNextId()
        {
            return _dbAccess.ExecuteQuery<int>(conn =>
            {
                using (var cmd = new NpgsqlCommand("SELECT MAX(package_id) FROM packages", conn))
                {
                    object result = cmd.ExecuteScalar();
                    // Überprüfen, ob das Ergebnis NULL ist (d.h. keine Einträge in der Tabelle)
                    if (result == DBNull.Value)
                    {
                        return 1; // Keine Einträge vorhanden, also 1 zurückgeben
                    }
                    else
                    {
                        return Convert.ToInt32(result) + 1; // MAX(package_id) + 1 zurückgeben
                    }
                }
            });
        }


        //Insert package to DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void DeserializeAndInsertPackageToDB(string packageContent)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cards = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(packageContent, options);
            int nextId = GetNextId();

            _dbAccess.ExecuteTransaction((conn, trans) => // Beginn der Transaktion
            {
                foreach (var card in cards)
                {
                    (string Element, string Name) CardTuple = _parser.ParseCards(card["Name"].GetString());
                    double damage = card["Damage"].GetDouble();
                    try
                    {
                        using (var cmd = new NpgsqlCommand(
                                   "INSERT INTO packages (package_id, unique_id, name, element, damage) VALUES (@package_id, @unique_id, @name, @element, @damage)",
                                   conn))
                        {
                            cmd.Transaction = trans; // Zuweisen der Transaktion

                            // Füge die Parameter hinzu
                            cmd.Parameters.AddWithValue("@package_id", nextId);
                            cmd.Parameters.AddWithValue("@unique_id", card["Id"].ValueKind != JsonValueKind.Null ? card["Id"].GetString() : DBNull.Value);
                            cmd.Parameters.AddWithValue("@name", CardTuple.Name);
                            cmd.Parameters.AddWithValue("@element", CardTuple.Element);
                            cmd.Parameters.AddWithValue("@damage", damage);

                            cmd.ExecuteNonQuery(); // Führen Sie den Befehl aus
                        }
                    } 
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        trans.Rollback();
                        throw new Exception("Error inserting Package"); // Rollback im Fehlerfall
                    }
                }
                trans.Commit(); // Commit am Ende aller Einfügungen
            });
        }



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
                throw new Exception("Error while getting package count");
            }
        }

        //Remove first package from DB
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 public void RemoveFirstPackage()
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM packages WHERE package_id IN (SELECT MIN(package_id) FROM packages)", conn))
                    {
                        cmd.Transaction = trans;  // Assign the transaction
                        cmd.ExecuteNonQuery();  // Execute the command
                    }
                    trans.Commit();  // Commit the transaction after all operations
                }
                catch (Exception e)
                {
                    trans.Rollback();  // Rollback the transaction in case of an error
                    throw new Exception("Error while removing first package", e);
                }
            });
        }



        //Move cards from packages Table in card_stacks table after acquirement
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TransferPackageToStack(int userId)
        {
            _dbAccess.ExecuteTransaction((conn, trans) =>
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                               "INSERT INTO card_stacks (user_id, unique_id, name, element, damage) SELECT @user_id, unique_id, name, element, damage FROM packages WHERE package_id IN (SELECT MIN(package_id) FROM packages)",
                               conn))
                    {
                        cmd.Transaction = trans;  // Assign the transaction

                        // Add the parameter
                        cmd.Parameters.AddWithValue("@user_id", userId);

                        cmd.ExecuteNonQuery();  // Execute the command
                    }
                    trans.Commit();  // Commit the transaction after all operations
                }
                catch (Exception e)
                {
                    trans.Rollback();  // Rollback the transaction in case of an error
                    throw new Exception("Error while transferring package to stack", e);
                }
            });
        }

    }
}