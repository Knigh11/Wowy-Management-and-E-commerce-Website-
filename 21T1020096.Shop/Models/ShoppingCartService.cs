using SV21T1020096.Shop.AppCodes;
using SV21T1020096.Shop.Models;

public class ShoppingCartService : IShoppingCartService
{
    private const string SHOPPING_CART = "ShoppingCart";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CartItem> GetShoppingCart()
    {
        var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
        if (shoppingCart == null)
        {
            shoppingCart = new List<CartItem>();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
        }
        return shoppingCart;
    }

    public void RemoveFromCart(int id)
    {
        var shoppingCart = GetShoppingCart();
        int index = shoppingCart.FindIndex(m => m.ProductID == id);
        if (index >= 0)
            shoppingCart.RemoveAt(index);
        ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
    }

    public void AddToCart(CartItem item)
    {
        var shoppingCart = GetShoppingCart();
        var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
        if (existsProduct == null)
        {
            shoppingCart.Add(item);
        }
        else
        {
            existsProduct.Quantity += item.Quantity;
            existsProduct.SalePrice = item.SalePrice;
        }
        ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
    }

    public void ClearCart()
    {
        var shoppingCart = GetShoppingCart();
        shoppingCart.Clear();
        ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
    }
}