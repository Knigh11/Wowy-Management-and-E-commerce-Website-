using SV21T1020096.DomainModels;

namespace SV21T1020096.Shop.Models
{
    public abstract class PaginationSearchResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                {
                    return 1;
                }
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                {
                    c += 1;
                }
                return c;
            }
        }
    }
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
    public class ProductSearchResult : PaginationSearchResult
    {
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public required List<Product> Data { get; set; }

        public ProductSearchResult()
        {

        }
    }
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}
