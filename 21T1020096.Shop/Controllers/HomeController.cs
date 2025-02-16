using Microsoft.AspNetCore.Mvc;
using SV21T1020096.BusinessLayers;
using SV21T1020096.Shop.AppCodes;
using SV21T1020096.Shop.Models;
using System.Diagnostics;

namespace SV21T1020096.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IShoppingCartService _cartService;
        private const int PAGE_SIZE = 32;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public HomeController(ILogger<HomeController> logger, IShoppingCartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }


        public IActionResult Index()
        {
            ViewBag.CountProduct = _cartService.GetShoppingCart().Count();
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
            //if (TempData.ContainsKey("CategoryID"))
            //{
            //    ViewBag.CategoryID = condition.CategoryID = (int)TempData["CategoryID"];
            //    TempData.Remove("CategoryID");
            //}
            //if (TempData.TryGetValue("MinPrice", out var minPriceString) &&
            //    decimal.TryParse(minPriceString as string, NumberStyles.Number, CultureInfo.InvariantCulture, out var minPrice))
            //{
            //    condition.MinPrice = minPrice;
            //    ViewBag.MinPrice = minPrice;
            //}
            //if (TempData.TryGetValue("MaxPrice", out var maxPriceString) &&
            //    decimal.TryParse(minPriceString as string, NumberStyles.Number, CultureInfo.InvariantCulture, out var maxPrice))
            //{
            //    condition.MaxPrice = maxPrice;
            //    ViewBag.MaxPrice = maxPrice;
            //}
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            //if (TempData["ProductSearchInput"] != null)
            //{
            //    condition = JsonConvert.DeserializeObject<ProductSearchInput>((string)TempData["ProductSearchInput"]);
            //    TempData.Remove("ProductSearchInput");
            //    // Sử dụng condition
            //}
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
        public IActionResult Search1(int page = 0, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            ProductSearchInput condition = new ProductSearchInput()
            {
                Page = page,
                PageSize = pageSize,
                SearchValue = searchValue,
                CategoryID = categoryID,
                SupplierID = supplierID,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            //TempData["ProductSearchInput"] = JsonConvert.SerializeObject(conditoin);
            //TempData["CategoryID"] = categoryID;
            //TempData["MixPrice"] = minPrice.ToString(CultureInfo.InvariantCulture);
            //TempData["MaxPrice"] = maxPrice.ToString(CultureInfo.InvariantCulture);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return RedirectToAction("Index", "Home");

        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }
        public List<CartItem> GetShoppingCart()
        {
            var shoppingCart = _cartService.GetShoppingCart();
            return shoppingCart;
        }

        public IActionResult MiniCart()
        {
            return PartialView(GetShoppingCart());
        }
        [HttpGet]
        public IActionResult GetCartCount()
        {
            return Json(GetShoppingCart()?.Count() ?? 0);
        }
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            _cartService.RemoveFromCart(id);
            return Json("");
        }
        public IActionResult MobileNav(ProductSearchInput condition)
        {
            return PartialView(condition);
        }
    }
}
