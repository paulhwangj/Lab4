﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Collections.ObjectModel;
using Npgsql;

// https://www.dotnetperls.com/serialize-list
// https://www.daveoncsharp.com/2009/07/xml-serialization-of-collections/


namespace Lab4
{
    public class RelationalDatabase : IDatabase
    {
        String connectionString;
        ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
        JsonSerializerOptions options;


        /// <summary>
        /// Creates connection string to be used to connect to bit.io db
        /// </summary>
        public RelationalDatabase()
        {
            connectionString = InitializeConnectionString();
        }

        /// <summary>
        /// Creates the connection string to be utilized throughout the program
        /// </summary>
        public String InitializeConnectionString()
        {
            var bitHost = "db.bit.io";
            var bitApiKey = "v2_3uf2D_m8ksxvxbCX4iXqDbU9vL9Di";

            var bitUser = "paulhwangj";
            var bitDbName = "paulhwangj/lab3";

            return connectionString = $"Host={bitHost};Username={bitUser};Password={bitApiKey};Database={bitDbName}";
        }


        /// <summary>
        /// Adds an entry to the database
        /// </summary>
        /// <param name="entry">the entry to add</param>
        public void AddEntry(Entry entry)
        {
            try
            {
                using var con = new NpgsqlConnection(connectionString);
                con.Open();
                var sql = "INSERT INTO entries (clue, answer, difficulty, date, id) VALUES(@clue, @answer, @difficulty, @date, @id)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("clue", entry.Clue);
                cmd.Parameters.AddWithValue("answer", entry.Answer);
                cmd.Parameters.AddWithValue("difficulty", entry.Difficulty);
                cmd.Parameters.AddWithValue("date", entry.Date);
                cmd.Parameters.AddWithValue("id", entry.Id);
                int numRowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"The # of rows inserted was {numRowsAffected}");
                con.Close();

                entries.Add(entry); // database successfully added the entry, now let's add it to entries
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Error while adding entry: {0}", ioe);
            }
        }


        /// <summary>
        /// Finds a specific entry
        /// </summary>
        /// <param name="id">id of entry to find</param>
        /// <returns>the Entry (if available, null otherwise)</returns>
        public Entry FindEntry(int id)
        {
            foreach (Entry entry in entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="entry">An entry, which is verified to exist</param>
        /// <returns>a bool that says if deletion was successful or not</returns>
        public bool DeleteEntry(Entry entry)
        {
            try
            {
                var result = entries.Remove(entry);

                using var con = new NpgsqlConnection(connectionString);
                con.Open();
                var sql = "DELETE FROM entries WHERE id = @id"; // don't hardcode,  
                                                                // and don't use unsanitized user input, instead ... 
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("id", entry.Id);
                int numRowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"The # of rows deleted was {numRowsAffected}");
                con.Close(); // Write the SQL to DELETE entry from bit.io. You have its id, that should be all that you need

                return true;
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Error while deleting entry: {0}", ioe);
            }
            return false;
        }

        /// <summary>
        /// Edits an entry
        /// </summary>
        /// <param name="modifiedEntry">Entry containing updated information but same id</param>
        /// <returns>true if editing was successful, false otherwise</returns>
        public bool EditEntry(Entry modifiedEntry)
        {
            foreach (Entry entry in entries) // iterate through entries until we find the Entry in question
            {
                if (entry.Id == modifiedEntry.Id) // found it
                {
                    try
                    {
                        using var con = new NpgsqlConnection(connectionString);
                        con.Open();
                        var sql = "UPDATE entries SET clue = @clue, answer = @answer, difficulty = @difficulty, date = @date WHERE id = @id";
                        using var cmd = new NpgsqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("clue", modifiedEntry.Clue);
                        cmd.Parameters.AddWithValue("answer", modifiedEntry.Answer);
                        cmd.Parameters.AddWithValue("difficulty", modifiedEntry.Difficulty);
                        cmd.Parameters.AddWithValue("date", modifiedEntry.Date);
                        cmd.Parameters.AddWithValue("id", modifiedEntry.Id);
                        int numRowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"The # of rows inserted was {numRowsAffected}");
                        con.Close();

                        // modify entry in entries after it's been added to db
                        entry.Clue = modifiedEntry.Clue;
                        entry.Answer = modifiedEntry.Answer;
                        entry.Difficulty = modifiedEntry.Difficulty;
                        entry.Date = modifiedEntry.Date;
                        
                        return true;
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("Error while replacing entry: {0}", ioe);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Populates entries with entries in the db
        /// </summary>
        /// <returns>ObservableCollection of <Entry> that will display on application</returns>
        public ObservableCollection<Entry> GetEntries()
        {
            PopulateEntries("none");
            return entries;
        }

        /// <summary>
        /// Sorts the entries by the clue (ascending alphabetical)
        /// </summary>
        public void SortByClue()
        {
            PopulateEntries("clue");
        }

        /// <summary>
        /// Sorts the entries by the answer (ascending alphabetical)
        /// </summary>
        public void SortByAnswer()
        {
            PopulateEntries("answer");
        }

        /// <summary>
        /// Populates entries in the proper ordering that it needs to be in
        /// </summary>
        ///<param name="ordering">string representing how entries should be ordered</param>
        public void PopulateEntries(string ordering)
        {
            string sql;
            // sql statement depends on which method called it
            if (ordering == "clue") // SortByClue()
            {
                sql = "SELECT * FROM entries ORDER BY clue";
            }
            else if (ordering == "answer") // SortByAnswer()
            {
                sql = "SELECT * FROM entries ORDER BY answer";
            }
            else // GetEntries(), order of entries doesn't matter
            {
                sql = "SELECT * FROM entries";
            }

            // clear entries, ordering of entries is going to change
            entries.Clear();
            
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // populates accordingly
                entries.Add(new Entry(reader[0] as String, reader[1] as String, (int)reader[2], reader[3] as String, (int)reader[4]));
            }
            con.Close();
        }

        /// <summary>
        /// Ran only once at program start up, it retrieves the next available Id by
        /// finding the max id within the database and returns it
        /// </summary>
        /// <returns>the max id (int)</returns>
        public int GetNextId() {
            int id; 
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT MAX(id) FROM entries;"; // returns the largest id in the table
            using var cmd = new NpgsqlCommand(sql, con);
            var check = cmd.ExecuteScalar();    // assigns the largest id in the table to id
            con.Close();

            // check will be of type System.DBNull if the table is empty (meaning no MAX(id) exists)
            if(check.GetType() != typeof(System.DBNull))
            {
                id = (Int32)check;
            }
            else
            {
                id = 0;
            }
            return id;
        }

        /// <summary>
        /// Clears the database by deleting all rows
        /// </summary>
        public void ClearDatabase()
        {
            try
            {
                using var con = new NpgsqlConnection(connectionString);
                con.Open();
                var sql = "DELETE FROM entries;"; // returns the largest id in the table
                using var cmd = new NpgsqlCommand(sql, con);
                int numRowsAffected = cmd.ExecuteNonQuery();    // assigns the largest id in the table to id
                con.Close();
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Error while Clearing database: {0}", ioe);
            }
        }
    }
}