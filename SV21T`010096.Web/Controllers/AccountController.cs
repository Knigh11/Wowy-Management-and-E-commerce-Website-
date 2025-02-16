using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.Web;

namespace SV21T10200096.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.UserName = username;
            //Kiểm tra thông tin đầu vào 
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên đăng nhập và mật khẩu!");
                return View();
            }
            //// TODO: Kiểm tra xem username  và password (của Employee) có đúng không?
            //if (username!="admin")
            //{
            //    ModelState.AddModelError("Error", "Đăng nhập thất bại");
            //    return View();
            //}
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            //Đăng nhập thành công
            //1. Tạo thông tin người dùng 
            var userData = new WebUserData()
            {
                UserID = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',')
                             .Select(role => role.Trim())
                             .ToList()

            };

            // 2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        [HttpPost]
        public IActionResult ChangePassword(string UserID = "", string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (string.IsNullOrWhiteSpace(oldPassword)) { ModelState.AddModelError("Error", "Mật khẩu cũ không được để trống"); return View(); }
            if (string.IsNullOrWhiteSpace(newPassword)) { ModelState.AddModelError("Error", "Mật khẩu mới không được để trống"); return View(); }
            if (string.IsNullOrWhiteSpace(confirmPassword)) { ModelState.AddModelError("Error", "Mật khẩu xác nhận lại không được để trống"); return View(); }
            if (!string.Equals(newPassword, confirmPassword)) { ModelState.AddModelError("Error", "Mật khẩu mới và xác nhận mật khẩu không khớp nhau"); return View(); }
            bool result = UserAccountService.ChangePassword(UserTypes.Employee, UserID, oldPassword, newPassword);
            if (result == false)
            {
                ModelState.AddModelError("Error", "Nhập sai mật khẩu cũ");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}