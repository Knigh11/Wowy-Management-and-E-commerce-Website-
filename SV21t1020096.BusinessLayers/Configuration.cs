namespace SV21T1020096.BusinessLayers
{
    public static class Configuration
    {
        private static string connectionString = "";
        /// <summary>
        /// Khởi tạo cấu hình cho BusinessLayer
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }
        public static string ConnectionString
        {
            get { return connectionString; }
        }
    }
}
