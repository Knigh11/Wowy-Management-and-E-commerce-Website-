using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Shop.Models;

namespace SV21T1020096.Shop.Controllers
{
    [Authorize(Roles = "customer")]
    public class Account_InfController : Controller
    {

        public IActionResult Index()
        {
            var id = User?.GetUserData()?.UserID;
            var data = CommonDataService.GetCustomer(Convert.ToInt32(id));
            return View(data);
        }
        [HttpGet]
        [HttpPost]
        public IActionResult ChangePassword(string UserID = "", string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (string.IsNullOrWhiteSpace(oldPassword)) { ModelState.AddModelError("Error", "Mật khẩu cũ không được để trống"); return View(); }
            if (string.IsNullOrWhiteSpace(newPassword)) { ModelState.AddModelError("Error", "Mật khẩu mới không được để trống"); return View(); }
            if (string.IsNullOrWhiteSpace(confirmPassword)) { ModelState.AddModelError("Error", "Mật khẩu xác nhận lại không được để trống"); return View(); }
            if (!string.Equals(newPassword, confirmPassword)) { ModelState.AddModelError("Error", "Mật khẩu mới và xác nhận mật khẩu không khớp nhau"); return View(); }
            bool result = UserAccountService.ChangePassword(UserTypes.Customer, UserID, oldPassword, newPassword);
            if (result == false)
            {
                ModelState.AddModelError("Error", "Nhập sai mật khẩu cũ");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Save(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành");
            if (ModelState.IsValid == false)
            {
                return View("Index", data);
            }
            //TODO: Kiểm tra dữ liệu có đúng hay không
            try
            {

                // Cập nhật khách hàng hiện có
                bool result = CommonDataService.UpdateCustomer(data);
                if (result == false)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Index", data);
                }

                // Chuyển hướng về trang danh sách sau khi lưu
                return View("Index", data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Index");
            }

        }
        [HttpGet]
        public IActionResult Order_Infor(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(order.OrderID);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details

            };
            return View(model);
        }
    }
}
