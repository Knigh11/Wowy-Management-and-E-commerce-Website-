using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;

namespace SV21T1020096.Web.Controllers
{
    [Authorize(Roles = "admin, employee")]
    public class ProductController : Controller
    {
        private readonly IS3Service _s3Service;
        private const int PAGE_SIZE = 30;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public ProductController(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                };

            }
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng mới";
            ViewBag.IsOld = false;
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = true,
            };

            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsOld = true;
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Product data, IFormFile? uploadPhoto)
        {
            string oldPhoto = "";
            if (uploadPhoto != null)
            {
                oldPhoto = data.ProductID == 0 ? "" : ProductDataService.GetProduct(data.ProductID).Photo;
                if (oldPhoto != "") await _s3Service.DeleteFileAsync(oldPhoto);
                String fileName = await _s3Service.UploadFileAsync(uploadPhoto, "products");
                data.Photo = _s3Service.GetFileUrl(fileName);
            }
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Xin hãy nhập mô tả mặt hàng");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Xin hãy chọn loại hàng");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Xin hãy chọn nhà cung cấp");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (data.Price == 0)
                ModelState.AddModelError(nameof(data.Price), "Giá mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Photo))
                ModelState.AddModelError(nameof(data.Photo), "Hãy tải lên ảnh của sản phẩm");
            if (ModelState.IsValid == false)
            {
                ViewBag.IsOld = false;
                return View("Edit", data);
            }
            try
            {
                if (data.ProductID == 0)
                {
                    // Thêm mới mặt hàng
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ViewBag.IsOld = false;
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng bị trùng");
                        return View("Edit", data);
                    }
                    else
                    {
                        ViewBag.IsOld = true;
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Cập nhật mặt hàng hiện có
                    bool result = ProductDataService.UpdateProduct(data);
                    if (result == false)
                    {
                        ViewBag.IsOld = true;
                        ModelState.AddModelError(nameof(data.ProductName), "Nhà cung cấp bạn chọn đã có loại hàng này");
                        return View("Edit", data);
                    }
                }
                // Chuyển hướng về trang danh sách sau khi lưu
                ViewBag.IsOld = true;
                return RedirectToAction("Edit", data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Edit");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            string oldPhoto = "";
            if (uploadPhoto != null)
            {
                oldPhoto = data.PhotoId == 0 ? "" : ProductDataService.GetPhoto(data.PhotoId).Photo;
                if (oldPhoto != "") await _s3Service.DeleteFileAsync(oldPhoto);
                String fileName = await _s3Service.UploadFileAsync(uploadPhoto, "productPhotos");
                data.Photo = _s3Service.GetFileUrl(fileName);
            }
            if (string.IsNullOrWhiteSpace(data.Photo))
                ModelState.AddModelError(nameof(data.Photo), "Hãy tải ảnh lên cho mặt hàng");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Hãy nhập mô tả cho ảnh");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Hãy nhập thứ tự hiển thị cho ảnh");
            if (ModelState.IsValid == false)
            {
                return View("Photo", data);
            }
            try
            {
                if (data.PhotoId == 0)
                {
                    long id = ProductDataService.AddPhoto(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Bị trùng thứ tự hiển thị");
                        return View("Photo", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Photo), "Bị trùng ảnh hoặc thứ tự hiển thị");
                        return View("Photo", data);
                    }

                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                //return View("Photo");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Thêm thông báo lỗi vào ModelState để hiển thị trên giao diện người dùng
                ModelState.AddModelError("Error", $"Đã xảy ra lỗi: {ex.Message}");

                // Trả về view với thông báo lỗi
                return View("Photo");
            }
        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Hãy nhập thứ tự hiển thị cho thuộc tính");
            if (ModelState.IsValid == false)
            {
                return View("Attribute", data);
            }
            try
            {
                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                    if (id == -1)
                    {
                        ModelState.AddModelError(nameof(data.AttributeName), "Có thể bị trùng tên thuộc tính");
                        return View("Attribute", data);
                    }
                    else
                    {
                        if (id == -2)
                        {
                            ModelState.AddModelError(nameof(data.AttributeValue), "Có thể bị trùng giá trị thuộc tính");
                            return View("Attribute", data);
                        }
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.DisplayOrder), "Có thể bị trùng thứ tự hiển thị");
                        ModelState.AddModelError(nameof(data.AttributeName), "Có thể bị trùng Tên thuộc tính");
                        ModelState.AddModelError(nameof(data.AttributeValue), "Có thể bị trùng giá trị thuộc tính");
                        return View("Attribute", data);
                    }
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Attribute");
            }
        }
        public IActionResult Photo(int id = 0, string method = "", long photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var data = new ProductPhoto
                    {
                        ProductID = id,
                        PhotoId = photoId// Khởi tạo ProductID nếu có
                    };
                    return View(data);

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    var data_edit = ProductDataService.GetPhoto(photoId);
                    return View(data_edit);

                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Attribute(int id = 0, string method = "", long attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var data = new ProductAttribute
                    {
                        AttributeID = attributeId,
                        ProductID = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    data = ProductDataService.GetAttribute(attributeId);
                    return View(data);
                case "delete":
                    ProductDataService.DeletableAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}


