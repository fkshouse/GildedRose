using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

namespace GildedRoseKata
{
    public class Database
    {
        const string _dbconnection = @"Data Source=C:\database\guildedrosedb.db";
        IList<Item> Items = new List<Item>
        {
            new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
            new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
            new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 49},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49},
            new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
        };
        IList<QualityUpdate> QAItems = new List<QualityUpdate>
        {
            new QualityUpdate {ItemId = 1, ChangeRate = "P|MINUS1|N|MINUS1"},
            new QualityUpdate {ItemId = 2, ChangeRate = "P|PLUS1|N|PLUS1"},
            //new QualityUpdate {ItemId = 3, ChangeRate = "P|MINUS1|N|MINUS1"},
            //new QualityUpdate {ItemId = 4, ChangeRate = "P|NO|N|NO"},
            //new QualityUpdate {ItemId = 5, ChangeRate = "P|NO|N|NO"},
            //new QualityUpdate {ItemId = 6, ChangeRate = "P|PLUS1|N|ZERO|T1"},
        };

        public Database()
        {
            if (!DoesTableExistAndHasRows())
                InstantiateDB();
        }

        public void Update(int id, Item item)
        {
            var sql = @"UPDATE item SET sellin = @sellin, quality = @quality WHERE id = @id";

            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();
                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@sellin", item.SellIn);
                command.Parameters.AddWithValue("@quality", item.Quality);
                command.Parameters.AddWithValue("@id", id);
                var rowInserted = command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string GetQualityChangeRate(int id)
        {
            var sql = @"SELECT changerate FROM qualitychange WHERE itemid = @itemid";
            string changerate = "";
            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@itemid", id);

                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        changerate = reader["changerate"].ToString();
                        
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return changerate;
        }

        public Dictionary<int, Item> GetAllItems()
        {
            var query = @"SELECT * FROM item";
            DataTable dt = new DataTable();
            Dictionary<int, Item> items = new Dictionary<int, Item>();
            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    SqliteDataReader queryCommandReader = command.ExecuteReader();
                    dt.Load(queryCommandReader);
                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            items.Add(Convert.ToInt32(row["id"]), new Item { Name = row["name"].ToString(), Quality = Convert.ToInt32(row["quality"]), SellIn = Convert.ToInt32(row["sellin"]) });
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return items;
        }

        public Item GetItemById(int id)
        {
            var query = @"SELECT * FROM item WHERE id = @id";
            DataTable dt = new DataTable();
            Item item = new Item();
            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@id", id);
                    SqliteDataReader queryCommandReader = command.ExecuteReader();
                    dt.Load(queryCommandReader);
                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                       item = new Item { Name = dt.Rows[0]["name"].ToString(), Quality = Convert.ToInt32(dt.Rows[0]["quality"]), SellIn = Convert.ToInt32(dt.Rows[0]["sellin"]) };
                        
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return item;
        }

        public void ResetAllOriginalValues()
        {
            DeleteAllRows();
            InstantiateDB();
        }

        private void DeleteAllRows()
        {
            var deleteitems = @"DELETE FROM item";
            var deleteqc = @"DELETE FROM qualitychange";
            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();
                using var command = new SqliteCommand(deleteitems, connection);
                command.ExecuteNonQuery();
                command.CommandText = deleteqc;
                command.ExecuteNonQuery();

            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool DoesTableExistAndHasRows()
        {
            try
            {
                var checkitems = @"SELECT COUNT(*) FROM item";
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();
                using var command = new SqliteCommand(checkitems, connection);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (SqliteException ex)
            {
                return false;
            }

        }

        private void InstantiateDB()
        {
            var createItemTable = @"CREATE TABLE IF NOT EXISTS item(id INTEGER PRIMARY KEY, name TEXT NOT NULL, sellin INT NOT NULL, quality INT NOT NULL)";            
            var createQualityChangeTable = @"CREATE TABLE IF NOT EXISTS qualitychange(id INTEGER PRIMARY KEY, itemid INT NOT NULL, changerate TEXT NOT NULL)";            

            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = createItemTable;
                    command.ExecuteNonQuery();                    

                    command.CommandText = createQualityChangeTable;
                    command.ExecuteNonQuery();                    

                    transaction.Commit();
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (var item in Items)
            {
                AddItem(item);
            }

            foreach (var qc in QAItems)
            {
                AddQualityChange(qc);
            }
        }

        private void AddItem(Item item)
        {
            var addItems = @"INSERT INTO item (name, sellin, quality) VALUES (@name, @sellin, @quality)";

            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = addItems;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@sellin", item.SellIn);
                    command.Parameters.AddWithValue("@quality", item.Quality);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddQualityChange (QualityUpdate qc)
        {
            var addQualityChange = @"INSERT INTO qualitychange (itemid, changerate) VALUES (@itemid, @changerate)";

            try
            {
                using var connection = new SqliteConnection(_dbconnection);
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = addQualityChange;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@itemid", qc.ItemId);
                    command.Parameters.AddWithValue("@changerate", qc.ChangeRate);
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
