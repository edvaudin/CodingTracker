using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CodingTracker
{
    internal class DAL : IDisposable
    {
        private string? connectionString;
        protected SqliteConnection conn = null;
        private bool disposed;

        public DAL() { }

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
        }

        private void OpenConnection()
        {
            conn = new SqliteConnection(connectionString);
            conn.Open();
        }

        private void CloseConnection()
        {
            if (conn?.State != System.Data.ConnectionState.Closed)
            {
                conn?.Close();
            }
        }

        private void CreateMainTableIfMissing()
        {
            OpenConnection();
            string sql = "CREATE TABLE IF NOT EXISTS tracker (" +
                         "id INTEGER NOT NULL UNIQUE, " +
                         "start_time TEXT NOT NULL, " +
                         "end_time TEXT NOT NULL, " +
                         "duration TEXT NOT NULL, " +
                         "PRIMARY KEY(id AUTOINCREMENT));";
            SqliteCommand cmd = new(sql, conn);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void AddEntry(string startTime, string endTime, string duration) 
        {
            OpenConnection();
            string sql = "INSERT INTO tracker (start_time, end_time, duration) VALUES (@start_time, @end_time, @duration);";
            SqliteCommand cmd = new(sql, conn);
            AddParameter("@start_time", startTime, cmd);
            AddParameter("@end_time", endTime, cmd);
            AddParameter("@duration", duration, cmd);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public List<CodingSession> GetCodingSessions()
        {
            OpenConnection();
            string sql = "SELECT * FROM tracker;";
            SqliteCommand cmd = new(sql, conn);
            List<CodingSession> sessions = GetQueriedList(cmd, reader => new CodingSession(reader));
            CloseConnection();
            return sessions;
        }

        protected static List<T> GetQueriedList<T>(SqliteCommand cmd, Func<SqliteDataReader, T> creator)
        {
            List<T> results = new List<T>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(creator(reader));
                }
            }
            return results;
        }

        protected static void AddParameter<T>(string name, T value, SqliteCommand cmd)
        {
            SqliteParameter param = new(name, SqliteTypeConverter.GetDbType(value.GetType()));
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    conn.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
