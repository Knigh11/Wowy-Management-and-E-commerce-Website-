using Dapper;
using SV21T1020096.DomainModels;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID, Email as UserName, FullName as DisplayName, Photo, RoleNames 
                           from Employees where Email=@Email AND Password=@Password";
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

        public bool ChangePassword(string UserID, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Employees
                            SET Password = @newPassword
                            WHERE EmployeeID = @UserID AND Password = @oldPassword";
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

        public int Register(RegisterCustomer data)
        {
            throw new NotImplementedException();
        }
    }
}