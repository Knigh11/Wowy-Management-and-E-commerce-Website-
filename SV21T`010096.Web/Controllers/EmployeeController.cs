using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;

namespace SV21T1020096.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IS3Service _s3Service;
        public EmployeeController(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        private const int PAGE_SIZE = 30;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";

            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            };

            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Employee data, String _BirthDate, IFormFile? _Photo)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
            //TODO: Kiểm tra dữ liệu có đúng hay không
            //xử lý cho ngày sinh
            string oldPhoto = "";
            DateTime? d = _BirthDate.ToDateTime();
            if (d.HasValue)
            {
                data.BirthDate = d.Value;
            }

            //xử lý ảnh
            //if (_Photo != null)
            //{
            //    String fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
            //    String filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
            //    using (var strean = new FileStream(filePath, FileMode.Create))
            //    {
            //        _Photo.CopyTo(strean);
            //    };
            //    data.Photo = fileName;
            //}
            if (_Photo != null)
            {
                oldPhoto = data.EmployeeID == 0 ? "" : CommonDataService.GetEmployee(data.EmployeeID).Photo;
                if (oldPhoto != "") await _s3Service.DeleteFileAsync(oldPhoto);
                String fileName = await _s3Service.UploadFileAsync(_Photo, "employees");
                data.Photo = _s3Service.GetFileUrl(fileName);
            }
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(_BirthDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh của nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ của nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại của nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email của nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Photo))
                ModelState.AddModelError(nameof(data.Photo), "Hãy tải lên ảnh của nhân viên");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            try
            {
                if (data.EmployeeID == 0)
                {
                    // Thêm mới nhân viên
                    int id = CommonDataService.AddEmployee(data);
                    if (id == -1)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                    if (id == -2)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật nhân viên hiện có

                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
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
        public async Task<IActionResult> Delete(int id = 0)
        {
            var data = CommonDataService.GetEmployee(id);
            if (Request.Method == "POST")
            {
                await _s3Service.DeleteFileAsync(data.Photo);
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
