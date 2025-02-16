using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Web.AppCodes;
using SV21T1020096.Web.Models;
using System.Globalization;

namespace SV21T1020096.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";

        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"

                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                                                   condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details

            };

            return View(model);
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái đã được duyệt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
                TempData["Message"] = "Không thể duyệt đơn này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái kết thúc
        /// </summary>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                TempData["Message"] = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái bị hủy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác hủy đối với đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// chuyển đơn hàng sang trạng thái bị từ chối
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác từ chối đối với đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này";
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            ViewBag.Title = "Cập nhật thông tin chi tiết đơn hàng ";
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id });
        }
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng bán không hợp lệ" });
            if (salePrice < 0)
                return Json(new { success = false, message = "Giá bán không hợp lệ" });
            bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            if (!result)
                return Json(new { success = false, message = "Không được phép thay đổi thông tin của đơn hàng này" });
            return RedirectToAction("Details", new { id = orderID });
        }

        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json(new { success = false, message = "Vui lòng chọn người giao hàng" });

            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json(new { success = false, message = "Đơn hàng không cho phép chuyển cho người giao hàng" });

            return Json(new { success = true, redirectUrl = Url.Action("Details", "Order", new { id = id }) });
        }
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""

                };
            }
            return View(condition);
        }
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice <= 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)// nếu mặt hàng chưa có trong giỏ hàng thì bổ sung vào 
            {
                shoppingCart.Add(item);
            }
            else//nếu mặt hàng đã có trong giỏ thì tăng số lượng và thay đổi giá bán
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng chọn một mặt hàng cần bán");
            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            int employeeID = Convert.ToInt32(User.GetUserData()?.UserID);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
    }
}
