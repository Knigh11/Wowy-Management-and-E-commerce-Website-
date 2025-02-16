using SV21T1020096.DomainModels;

namespace SV21T1020096.DataLayers
{
    public interface IProductDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần tìm kiếm</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm(0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm(0 nếu không tìm theo nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng cân tìm(0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng(chuỗi rỗng nếu không tìm)</param>
        /// <param name="categoryID">Mã loại hàng(0 nếu không tìm theo loại)</param>
        /// <param name="supplierID">Mã nhà cung cấp(0 nếu không tìm thoe nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng cần tùm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng cần tìm(0 nếu không hạn chế)</param>
        /// <returns></returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// lấy thông tin mặt hàng theo mã
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Product? Get(int productID);
        /// <summary>
        /// Bổ sung mặt hàng mới (hàm trả về mã mặt hàng được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(Product data);
        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Product data);
        /// <summary>
        /// Xoá mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có đơn hàng hay không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool InUsed(int productID);
        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<ProductPhoto> ListPhotos(int productID);
        /// <summary>
        /// Lấy thông tin ảnh dựa vào ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        ProductPhoto? GetPhotos(long photoID);
        /// <summary>
        /// Bổ sung 1 ảnh cho mặt hàng (hàm trả về mã của ảnh được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddPhoto(ProductPhoto data);
        /// <summary>
        /// Cập nhât ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdatePhoto(ProductPhoto data);
        /// <summary>
        /// Xoá ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        bool DeletePhoto(long photoID);
        /// <summary>
        /// Lấy danh sách các thuộc tính của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<ProductAttribute> ListAttributes(int productID);
        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttributes(long attributeID);
        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddAttribute(ProductAttribute data);
        /// <summary>
        /// Cập nhât thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateAttribute(ProductAttribute data);
        /// <summary>
        /// Xoá thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        bool DeleteAttribute(long attributeID);
    }

}
