using SV21T1020096.DataLayers;
using SV21T1020096.DataLayers.SQLServer;
using SV21T1020096.DomainModels;

namespace SV21T1020096.BusinessLayers
{
    public static class ProductDataService
    {
        public static readonly IProductDAL productDB;
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (Không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(1, 0, searchValue, 0, 0, 0);
        }
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
        }
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        public static bool UpdateProduct(Product product)
        {
            return productDB.Update(product);
        }
        public static int AddProduct(Product product) { return productDB.Add(product); }
        public static bool DeleteProduct(int id)
        {
            if (productDB.InUsed(id)) return false;
            else
                return productDB.Delete(id);

        }
        public static bool InUsedProduct(int id) { return productDB.InUsed(id); }
        public static IList<ProductPhoto> ListPhotos(int productID) { return productDB.ListPhotos(productID); }
        public static ProductPhoto? GetPhoto(long attributeID) { return productDB.GetPhotos(attributeID); }
        public static long AddPhoto(ProductPhoto data) { return productDB.AddPhoto(data); }
        public static bool DeletePhoto(long photoID) { return productDB.DeletePhoto(photoID); }
        public static bool UpdatePhoto(ProductPhoto data) { return productDB.UpdatePhoto(data); }
        public static IList<ProductAttribute> ListAttributes(int productID) { return productDB.ListAttributes(productID); }
        public static ProductAttribute? GetAttribute(long attributeID) { return productDB.GetAttributes(attributeID); }
        public static long AddAttribute(ProductAttribute data) { return productDB.AddAttribute(data); }
        public static bool DeletableAttribute(long attributeID) { return productDB.DeleteAttribute(attributeID); }
        public static bool UpdateAttribute(ProductAttribute data) { return productDB.UpdateAttribute(data); }
    }
}

