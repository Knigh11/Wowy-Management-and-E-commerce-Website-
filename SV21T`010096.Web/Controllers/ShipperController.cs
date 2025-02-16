using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;
namespace SV21T1020096.Web.Controllers
{
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 30;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0,
            };

            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng mới" : "Cập nhật thông tin người giao hàng";
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trông");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            try
            {
                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    if (id == -1)
                    {
                        ModelState.AddModelError(nameof(data.ShipperName), "Tên nhà giao hàng này đã có");
                        return View("Edit", data);
                    }
                    if (id == -2)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại này đã có");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.ShipperName), "Trùng tên nhà giao hàng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Edit");
            }

        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }
    }
}
