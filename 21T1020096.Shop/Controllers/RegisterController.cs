using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;

namespace SV21T1020096.Shop.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            var userRegister = new RegisterCustomer()
            {
                CustomerID = 0,
                IsLocked = false,
            };
            return View(userRegister);
        }
        [HttpPost]
        public IActionResult Register(RegisterCustomer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName)) ModelState.AddModelError(nameof(data.CustomerName), "Tên tài khoản của bạn không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName)) ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch của bạn không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone)) ModelState.AddModelError(nameof(data.Phone), "Số điện thoại của bạn không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email)) ModelState.AddModelError(nameof(data.Email), "Email của bạn không được để trống");
            if (string.IsNullOrWhiteSpace(data.Province)) ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh thành bạn sống");
            if (string.IsNullOrWhiteSpace(data.Address)) ModelState.AddModelError(nameof(data.Address), "Hãy nhập địa chỉ của bạn");
            if (string.IsNullOrWhiteSpace(data.PassWord)) ModelState.AddModelError(nameof(data.PassWord), "Mật khẩu không được để trống");
            if (string.IsNullOrWhiteSpace(data.ConfirmPass)) ModelState.AddModelError(nameof(data.ConfirmPass), "Xác nhận mật khẩu không được để trống");
            if (!ModelState.IsValid)
            {
                return View("Index", data);
            }
            if (!string.Equals(data.PassWord, data.ConfirmPass))
            {
                ModelState.AddModelError(nameof(data.ConfirmPass), "Mật khẩu và xác nhận mật khẩu không khớp nhau");
                return View("Index", data);
            }
            try
            {
                int id = UserAccountService.Register(data);
                if (id == -1)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Index", data);
                }
                else
                {
                    if (id == -2)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Index", data);
                    }
                }
                ModelState.AddModelError("Success", "Đăng kí thành công, xin hãy đăng nhập vào tài bằng khoản vừa đăng kí");
                return View("Index", new RegisterCustomer() { CustomerID = 0 });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Index", new RegisterCustomer() { CustomerID = 0 });
            }
        }

    }
}
