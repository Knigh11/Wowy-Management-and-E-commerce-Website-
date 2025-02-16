using SV21T1020096.DataLayers;
using SV21T1020096.DataLayers.SQLServer;
using SV21T1020096.DomainModels;

namespace SV21T1020096.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountDAL(Configuration.ConnectionString);
            customerAccountDB = new CustomerAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
        public static int Register(RegisterCustomer data)
        {
            return customerAccountDB.Register(data);
        }
        public static bool ChangePassword(UserTypes userType, string UserID, string oldPassword, string newPassword)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.ChangePassword(UserID, oldPassword, newPassword);
            else
                return customerAccountDB.ChangePassword(UserID, oldPassword, newPassword);
        }

    }
    /// <summary>
    /// Loại tài khoản
    /// </summary>
    public enum UserTypes
    {
        Employee,
        Customer
    }
}