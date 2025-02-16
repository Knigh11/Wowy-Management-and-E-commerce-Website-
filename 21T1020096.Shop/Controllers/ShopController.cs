using _SV21T1020096.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.DomainModels;
using SV21T1020096.Shop.Models;
namespace SV21T1020096.Shop.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.CUSTOMER}")]
    public class ShopController : Controller
    {
        private readonly IShoppingCartService _cartService;
        public ShopController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }
        public IActionResult ProductView(int id = 0)
        {
            ViewBag.CountProduct = _cartService.GetShoppingCart().Count();
            var product = ProductDataService.GetProduct(id);
            return View(product);
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = _cartService.GetShoppingCart();
            return shoppingCart;
        }
        [HttpPost]
        public IActionResult AddToCart(CartItem item)
        {
            try
            {
                _cartService.AddToCart(item);
                return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            _cartService.RemoveFromCart(id);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(string deliveryProvince, string deliveryAddress)
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng chọn một mặt hàng muốn mua");
            if (string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin của bạn và nơi giao hàng");
            int employeeID = 2;
            int customerID = Convert.ToInt32(User.GetUserData()?.UserID);
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
        public IActionResult Checkout()
        {
            return View(GetShoppingCart());
        }
        public IActionResult MiniCart()
        {
            return PartialView(_cartService.GetShoppingCart());
        }
        public IActionResult GetCartCount()
        {
            return Json(_cartService.GetShoppingCart().Count());
        }
    }

}
