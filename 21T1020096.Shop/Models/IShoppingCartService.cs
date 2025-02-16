namespace SV21T1020096.Shop.Models
{
    public interface IShoppingCartService
    {
        List<CartItem> GetShoppingCart();
        void RemoveFromCart(int id);
        void ClearCart();
        void AddToCart(CartItem item);
    }
}
