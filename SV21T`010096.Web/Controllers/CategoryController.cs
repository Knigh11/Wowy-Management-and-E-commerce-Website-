using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;

namespace SV21T1020096.Web.Controllers
{
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông loại hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng mới" : "Cập nhật thông tin loại hàng";
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Hãy nhập mô tả");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            //TODO: Kiểm tra dữ liệu có đúng hay không
            try
            {
                if (data.CategoryID == 0)
                {
                    // Thêm mới khách hàng

                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật khách hàng hiện có
                    bool result = CommonDataService.UpdateCategory(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
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
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var data = new Category()
            {
                CategoryID = 0
            };

            return View("Edit", data);
        }
    }
}
