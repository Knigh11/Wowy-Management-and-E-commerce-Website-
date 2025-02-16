using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;

namespace SV21T1020096.Web.Controllers
{
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfSuppliers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);
            return View(model);
        }


        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetSupplier(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var data = CommonDataService.GetSupplier(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";

            var data = new Supplier()
            {
                SupplierID = 0
            };

            return View("Edit", data);
        }
        public IActionResult Save(Supplier data)
        {
            //if (data.SupplierID == 0) CommonDataService.AddSupplier(data);
            //else CommonDataService.UpdateSupplier(data);
            //return RedirectToAction("Index");
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp mới" : "Cập nhật thông tin nhà cung cấp";
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho nhà cung cấp");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            //TODO: Kiểm tra dữ liệu có đúng hay không
            try
            {
                if (data.SupplierID == 0)
                {
                    // Thêm mới nhà cung cấp
                    int id = CommonDataService.AddSupplier(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật nhà cung cấp hiện có
                    bool result = CommonDataService.UpdateSupplier(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                // Chuyển hướng về trang danh sách sau khi lưu
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Edit");
            }
        }
    }
}