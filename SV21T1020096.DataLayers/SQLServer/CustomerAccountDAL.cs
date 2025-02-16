using Dapper;
using SV21T1020096.DomainModels;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserID, Email as UserName, CustomerName as DisplayName,'' as Photo, RoleNames 
                           from Customers where Email=@Email AND Password=@Password";
                var parameters = new
                {
                    Email = userName,
                    Password = password,
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }
        public int Register(RegisterCustomer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @" IF EXISTS (SELECT 1 FROM Customers WHERE Email = @Email)
                                BEGIN
                                    SELECT -1 AS StatusCode; -- Trả về -1 nếu Email đã tồn tại
                                END
                                ELSE IF EXISTS (SELECT 1 FROM Customers WHERE Phone = @Phone)
                                BEGIN
                                    SELECT -2 AS StatusCode; -- Trả về -2 nếu Số điện thoại đã tồn tại
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO Customers (CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked, RoleNames)
                                    VALUES (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @PassWord, @IsLocked, 'customer');

                                    SELECT SCOPE_IDENTITY() AS NewCustomerID; -- Trả về ID của bản ghi vừa thêm
                                END
                                ";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    PassWord = data.PassWord ?? "",
                    IsLocked = data.IsLocked,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }
        public bool ChangePassword(string UserID, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers
                            SET Password = @newPassword
                            WHERE CustomerID = @UserID AND Password = @oldPassword";
                var parameters = new
                {
                    UserID = UserID,
                    newPassword = newPassword,
                    oldPassword = oldPassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
