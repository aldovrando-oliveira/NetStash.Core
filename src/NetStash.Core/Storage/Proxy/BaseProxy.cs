using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace NetStash.Core.Storage.Proxy
{
    public class BaseProxy
    {
        static string dbFilePath = "./NetStashCore.db";
        static bool initialized = false;
        static object _lock = new object();

        public BaseProxy()
        {
            lock (_lock)
            {
                if (initialized) return;

                if (!File.Exists(dbFilePath))
                {
                    using (var cnn = GetConnection())
                    {
                        cnn.Open();
                        var cmd = new SqliteCommand("CREATE TABLE \"Log\" ([IdLog] integer, [Message] nvarchar, PRIMARY KEY(IdLog));", cnn);
                        cmd.ExecuteNonQuery();
                    }
                }

                initialized = true;
            }
        }

        internal SqliteConnection GetConnection()
        {
            return new SqliteConnection(string.Format("Data Source={0};", dbFilePath));
        }
        private void SaveToDisk(string file, string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file))
            using (FileStream fileStream = new FileStream(name, FileMode.CreateNew))
                for (int i = 0; i < stream.Length; i++)
                    fileStream.WriteByte((byte)stream.ReadByte());
        }
    }
}
