namespace CommerceBuilder.Data
{
    using System;
    using System.IO;

    internal class SqlLogger
    {
        private static volatile SqlLogger instance;
        private static object syncRoot = new Object();

        public static SqlLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SqlLogger();
                    }
                }
                return instance;
            }
        }

        string _LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\sql.log");

        private SqlLogger() { }

        public void Append(string sql)
        {
            try
            {
                if (!File.Exists(_LogFile)) File.WriteAllText(_LogFile, string.Empty);
                using (StreamWriter sw = File.AppendText(_LogFile))
                {
                    sw.WriteLine(sql);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch { }
        }
    }
}