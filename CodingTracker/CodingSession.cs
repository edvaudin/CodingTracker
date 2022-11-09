using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker
{
    public class CodingSession
    {
        public int id = 0;
        public string startTime = string.Empty;
        public string endTime = string.Empty;
        public string duration = string.Empty;

        public CodingSession() { }

        public CodingSession(SqliteDataReader reader)
        {
            this.id = reader.GetInt32(reader.GetOrdinal("id"));
            this.startTime = reader.GetString(reader.GetOrdinal("start_time"));
            this.endTime = reader.GetString(reader.GetOrdinal("end_time"));
            this.duration = reader.GetString(reader.GetOrdinal("duration"));
        }

        public string GetPretifiedTime(string time)
        {
            if (DateTime.TryParseExact(time, "dd-MM-yy HH-mm-ss", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime parsedDate))
            {
                return $"{parsedDate.ToLongTimeString()} - {parsedDate.ToLongDateString()}";
            }
            else
            {
                return $"{time}";
            }
            
        }

        public override string ToString()
        {
            return $"[#{id}] Coded for {duration} ({startTime} to {endTime});";
        }
    }
}
