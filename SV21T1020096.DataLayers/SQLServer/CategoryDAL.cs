using Dapper;
using SV21T1020096.DomainModels;
using System.Data;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Category>, ISimpleSelectDAL<Category>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category data)
        {

            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS (SELECT 1 FROM Categories WHERE CategoryName = @CategoryName)
                                BEGIN
                                    SELECT -1; 
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO Categories (CategoryName, Description)
                                    VALUES (@CategoryName, @Description);

                                    SELECT SCOPE_IDENTITY();
                                END
                                ";
                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? ""
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
                        from Categories
                        where (CategoryName like @searchValue) or (Description like @searchValue)";
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
                var sql = @"DELETE FROM Categories WHERE CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Category? Get(int id)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Categories WHERE CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryId = id
                };
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Products WHERE CategoryID  = @CategoryID)
                                    SELECT 1
                               ELSE
                                    SELECT 0";
                var parameters = new
                {
                    CategoryId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                                    from (
                                            select *, ROW_NUMBER() over (order by CategoryName) as RowNumber
                                            from Categories
                                             where (CategoryName like @searchValue) or (Description like @searchValue)
                                              ) as t
                                    where (@pageSize =0) or (t.RowNumber between (@page -1) * @pageSize + 1 and @page * @pageSize)
                                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();

            }
            return data;
        }

        public List<Category> List()
        {
            List<Category> data = new List<Category>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Categories";
                data = connection.Query<Category>(sql: sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS (SELECT 1 FROM Categories WHERE CategoryName = @CategoryName AND CategoryID <> @CategoryID)
                                BEGIN
                                    SELECT -1; 
                                END
                                ELSE
                                BEGIN
                                    UPDATE Categories 
                                    SET CategoryName = @CategoryName,
                                        Description = @Description
                                    WHERE CategoryID = @CategoryID;
                                END
                                ";
                var parameters = new
                {
                    Description = data.Description ?? "",
                    CategoryName = data.CategoryName ?? "",
                    CategoryID = data.CategoryID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
