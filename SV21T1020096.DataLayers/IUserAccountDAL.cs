using SV21T1020096.DomainModels;

namespace SV21T1020096.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu liên quan đến tài khoản người dùng
    /// </summary>
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Xác thực tài khoản đăng nhập của người dùng.
        /// Hàm trả về thông tin tài khoản nếu xác thực thành công,
        /// ngược lại hàm trả về null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string userName, string password);
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        int Register(RegisterCustomer data);
    }
}
