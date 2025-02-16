using Dapper;
using SV21T1020096.DomainModels;
using System.Data;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, ISimpleSelectDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }
        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Provinces";
                data = connection.Query<Province>(sql: sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
