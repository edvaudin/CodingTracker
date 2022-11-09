using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Data;

namespace CodingTracker
{
    class DAL
    {
        private string connectionString;
        protected SqliteConnection conn = null;

        public DAL()
        {
            SetConnectionToAppSettingsReference();
        }

        public void InitializeDatabase()
        {
            SetConnectionToAppSettingsReference();
            try
            {
                CreateMainTableIfMissing();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetConnectionToAppSettingsReference()
        {
            connectionString = ConfigurationManager.AppSettings.Get("dbConnectionString");
            if (connectionString == null)
            {
                throw new NullReferenceException("Could not retrieve connection string from app settings.");
            }
        }

        private void CreateMainTableIfMissing()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "CREATE TABLE IF NOT EXISTS tracker (" +
                             "id INTEGER NOT NULL UNIQUE, " +
                             "start_time TEXT NOT NULL, " +
                             "end_time TEXT NOT NULL, " +
                             "duration TEXT NOT NULL, " +
                             "PRIMARY KEY(id AUTOINCREMENT));";
                SqliteCommand cmd = new(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        public List<CodingSession> GetCodingSessions()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM TRACKER;";
                SqliteCommand cmd = new(sql, conn);
                List<CodingSession> sessions = GetQueriedList(cmd, reader => new CodingSession(reader));
                return sessions;
            }
        }

        protected static List<T> GetQueriedList<T>(SqliteCommand cmd, Func<SqliteDataReader, T> creator)
        {

            List<T> results = new();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(creator(reader));
                }
            }
            return results;
        }

        public void AddEntry(string startTime, string endTime, string duration)
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO tracker (start_time, end_time, duration) VALUES (@start_time, @end_time, @duration);";
                SqliteCommand cmd = new(sql, conn);
                AddParameter("@start_time", startTime, cmd);
                AddParameter("@end_time", endTime, cmd);
                AddParameter("@duration", duration, cmd);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteEntry(int id)
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM tracker WHERE id = @id;";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                AddParameter("@id", id, cmd);
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateEntry(int id, string startTime, string endTime, string duration)
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE tracker SET start_time = @start_time, end_time = @end_time, duration = @duration WHERE id = @id;";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                AddParameter("@id", id, cmd);
                AddParameter("@start_time", startTime, cmd);
                AddParameter("@end_time", endTime, cmd);
                AddParameter("@duration", duration, cmd);
                cmd.ExecuteNonQuery();
            }
        }

        protected static void AddParameter<T>(string name, T value, SqliteCommand cmd)
        {
            SqliteParameter param = new(name, SqliteTypeConverter.GetDbType(value.GetType()));
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
