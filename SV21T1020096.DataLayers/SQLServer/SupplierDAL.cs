using Dapper;
using SV21T1020096.DomainModels;
using System.Data;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>, ISimpleSelectDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                                        Values(@SupplierName,@ContactName,@Province,@Address,@Phone,@Email);
                            Select SCOPE_IDENTITY();";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select COUNT (*)
                        from Suppliers
                        where (SupplierName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Suppliers WHERE SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var conn = OpenConnection())
            {
                var sql = "SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
                var parameters = new { SupplierID = id };
                data = conn.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connections = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Products WHERE SupplierID  = @SupplierID)
                                    SELECT 1
                               ELSE
                                    SELECT 0";
                var parameters = new { SupplierID = id };
                result = connections.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connections.Close();
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                                    from (
                                            select *, ROW_NUMBER() over (order by SupplierName) as RowNumber
                                            from Suppliers
                                             where (SupplierName like @searchValue) or (ContactName like @searchValue)
                                              ) as t
                                    where (@pageSize =0) or (t.RowNumber between (@page -1) * @pageSize + 1 and @page * @pageSize)
                                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue

                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }



            return data;
        }

        public List<Supplier> List()
        {
            List<Supplier> data = new List<Supplier>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Suppliers";
                data = connection.Query<Supplier>(sql: sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Suppliers SET 
                    SupplierName=@SupplierName,
                    ContactName =@ContactName,
                    Province =@Province,
                    Address =@Address,
                    Phone =@Phone,
                    Email=@Email
                    WHERE SupplierID=@SupplierID";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    SupplierID = data.SupplierID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
