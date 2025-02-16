using Dapper;
using SV21T1020096.DomainModels;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email)
                            BEGIN
                                SELECT -1; -- Trả về -1 nếu Email đã tồn tại
                            END
                            ELSE IF EXISTS (SELECT 1 FROM Employees WHERE Phone = @Phone)
                            BEGIN
                                SELECT -2; -- Trả về -2 nếu Phone đã tồn tại
                            END
                            ELSE
                            BEGIN
                                INSERT INTO Employees (FullName, BirthDate, Address, Phone, Email, Photo, Password, IsWorking)
                                VALUES (@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @Password, @IsWorking);

                                SELECT @@IDENTITY; -- Trả về ID của bản ghi vừa thêm
                            END
                            ";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    Password = data.Password ?? "123456",
                    IsWorking = data.IsWorking,
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
            using (var connectiom = OpenConnection())
            {
                var sql = @"select COUNT (*)
                        from Employees
                        where (FullName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connectiom.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connectiom.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Orders WHERE EmployeeID  = @EmployeeID)
                                    SELECT 1
                               ELSE
                                    SELECT 0";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                                    from (
                                            select *, ROW_NUMBER() over (order by FullName) as RowNumber
                                            from Employees
                                             where (FullName like @searchValue)
                                              ) as t
                                    where (@pageSize =0) or (t.RowNumber between (@page -1) * @pageSize + 1 and @page * @pageSize)
                                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();

            }
            return data;
        }

        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    if not exists (select * from Employees where EmployeeID <> @EmployeeID and Email =@Email)
                    begin
                        UPDATE Employees SET 
                        FullName=@FullName,
                        BirthDate =@BirthDate,
                        Address =@Address,
                        Photo =@Photo,
                        Phone =@Phone,
                        Email=@Email,
                        IsWorking=@IsWorking
                        WHERE EmployeeID=@EmployeeID
                    end";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Photo = data.Photo ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsWorking = data.IsWorking,
                    EmployeeID = data.EmployeeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
