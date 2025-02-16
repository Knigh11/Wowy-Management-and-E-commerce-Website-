using Dapper;
using SV21T1020096.DomainModels;
using System.Data;

namespace SV21T1020096.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        IF EXISTS (SELECT * FROM Products WHERE ProductName = @ProductName)
                            SELECT -1;
                        ELSE
                            BEGIN
                                INSERT INTO Products 
                                    (ProductName, 
                                     ProductDescription, 
                                     SupplierID, 
                                     CategoryID, 
                                     Unit, 
                                     Price, 
                                     Photo, 
                                     IsSelling)
                                VALUES 
                                    (@ProductName, 
                                     @ProductDescription, 
                                     @SupplierID, 
                                     @CategoryID, 
                                     @Unit, 
                                     @Price, 
                                     @Photo, 
                                     @IsSelling);
         
                                SELECT SCOPE_IDENTITY();
                            END
                        ";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            IF EXISTS (SELECT 1 FROM ProductAttributes WHERE ProductID = @ProductID AND AttributeName = @AttributeName)
                            BEGIN
                                SELECT -1; -- Trả về -1 nếu AttributeName đã tồn tại
                            END
                            ELSE IF EXISTS (SELECT 1 FROM ProductAttributes WHERE ProductID = @ProductID AND AttributeValue = @AttributeValue)
                            BEGIN
                                SELECT -2; -- Trả về -2 nếu AttributeValue đã tồn tại
                            END
                            ELSE
                            BEGIN
                                INSERT INTO ProductAttributes (ProductID, AttributeName, AttributeValue, DisplayOrder)
                                VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);

                                SELECT SCOPE_IDENTITY(); -- Trả về ID của bản ghi vừa thêm
                            END
                            ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                                IF EXISTS (SELECT * FROM ProductPhotos WHERE Photo = @Photo OR DisplayOrder = @DisplayOrder)
                                    SELECT -1;
                             ELSE 
                                BEGIN
                                INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden) 
                                        VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                SELECT SCOPE_IDENTITY();
                                END
                            ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT (*)
                        FROM Products
                        WHERE (@searchValue = '' OR ProductName LIKE @searchValue)  
                        AND (@categoryID = 0 OR CategoryID = @categoryID) 
                        AND (@supplierID = 0 OR SupplierId = @supplierID) 
                        AND (Price >= @minPrice) 
                        AND (@maxPrice <= 0 OR Price <= @maxPrice) ";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"    DELETE FROM ProductPhotos WHERE ProductID = @ProductID
                                DELETE FROM ProductAttributes WHERE ProductID = @ProductID
                                DELETE FROM Products WHERE ProductID = @ProductID;";
                var parameters = new
                {
                    ProductID = id,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductAttributes WHERE AttributeID = @attributeID";
                var parameters = new
                {
                    attributeID = attributeID,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductPhotos WHERE PhotoID = @photoID";
                var parameters = new
                {
                    photoID = photoID,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttributes(long attributeID)
        {

            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @attributeID";
                var parameters = new
                {
                    attributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhotos(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE PhotoID = @photoID";
                var parameters = new
                {
                    photoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM OrderDetails WHERE ProductID  = @ProductID)
                                    SELECT 1
                               ELSE
                                    SELECT 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * 
                            FROM (
                                 SELECT *, 
                                 ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber  FROM Products  
                                 WHERE (@searchValue = N'' OR ProductName LIKE @searchValue)  AND (@categoryID = 0 OR CategoryID = @categoryID) 
                                 AND (@supplierID = 0 OR SupplierId = @supplierID) 
                                 AND (Price >= @minPrice) 
                                 AND (@maxPrice <= 0 OR Price <= @maxPrice) 
                                 ) AS t 
                            WHERE (@PageSize = 0) OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * FROM ProductAttributes WHERE ProductID = @productID
                            ";
                var parameters = new
                {
                    productID = productID,
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * FROM ProductPhotos WHERE ProductID = @productID
                            ";
                var parameters = new
                {
                    productID = productID,
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if not exists (select * from Products where ProductID <> @ProductID and ProductName = @ProductName and SupplierID = @SupplierID and CategoryID = @CategoryID)
                            begin
                                UPDATE Products SET 
                                    ProductName = @ProductName,
                                    ProductDescription = @ProductDescription,
                                    SupplierID = @SupplierID,
                                    CategoryID = @CategoryID,
                                    Unit = @Unit,
                                    Price = @Price,
                                    Photo = @Photo,
                                    IsSelling = @IsSelling
                                WHERE ProductID = @ProductID
                            end
                            ";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                    ProductID = data.ProductID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                                if not exists (select * from ProductAttributes where ProductID = @productID and (AttributeName = @attributeName OR AttributeValue = @attributeValue OR DisplayOrder = @displayOrder))
                                begin
                                    UPDATE ProductAttributes SET 
                                        AttributeName = @attributeName,
                                        AttributeValue = @attributeValue,
                                        DisplayOrder = @displayOrder
                                    WHERE AttributeID = @attributeID
                                end
                            ";
                var parameters = new
                {
                    attributeName = data.AttributeName ?? "",
                    attributeValue = data.AttributeValue ?? "",
                    displayOrder = data.DisplayOrder,
                    attributeID = data.AttributeID,
                    productID = data.ProductID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            
                                    UPDATE ProductPhotos SET 
                                        ProductID = @productID,
                                        Photo = @photo,
                                        Description = @description,
                                        DisplayOrder = @displayOrder,
                                        IsHidden = @ishidden
                                    WHERE PhotoID = @photoID
                            ";
                var parameters = new
                {
                    productID = data.ProductID,
                    photo = data.Photo ?? "",
                    description = data.Description ?? "",
                    displayOrder = data.DisplayOrder,
                    photoID = data.PhotoId,
                    ishidden = data.IsHidden,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
