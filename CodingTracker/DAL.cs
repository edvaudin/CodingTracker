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
                Console.WriteLine("Successfully connected to database and created the tracker table.");
            }
        }
        public List<CodingSession> GetCodingSessions()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM TRACKER;";
                SqliteCommand cmd = new(sql, conn);
                Console.WriteLine("DAL using this connection: " + conn.ConnectionString);
                Console.WriteLine("Command using this connection: " + cmd.Connection.ConnectionString);
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

        protected static void AddParameter<T>(string name, T value, SqliteCommand cmd)
        {
            SqliteParameter param = new(name, SqliteTypeConverter.GetDbType(value.GetType()));
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
