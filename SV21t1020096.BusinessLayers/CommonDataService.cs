using SV21T1020096.DataLayers;
using SV21T1020096.DomainModels;

namespace SV21T1020096.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ISimpleSelectDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        /// <summary>
        /// Khởi tạo liên kết đến Server
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);


        }
        /// <summary>
        /// lấy thông tin của 1 khách hàng dựa trên mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// lấy thông tin của một nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một khách hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Bổ sung một nhà cung cấp mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// cập nhật thông tin của khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// cập nhật thông tin một nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xoá một khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id)) return false;
            else
                return customerDB.Delete(id);
        }
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.InUsed(id)) return false;
            else
                return employeeDB.Delete(id);
        }
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.InUsed(id)) return false;
            else
                return categoryDB.Delete(id);
        }
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.InUsed(id))
            {
                return false;
            }
            else
                return shipperDB.Delete(id);
        }
        /// <summary>
        /// Xoá một nhà cung cấp
        /// </summary>
        /// <param name="id">id nhà cung cấp</param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.InUsed(id)) return false;
            else
                return supplierDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem một khách hàng đang có đơn hàng hay không
        /// </summary>
        /// <param name="id">id một khách hàng</param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        /// <summary>
        /// Kiểm tra một nhà cung cấp có đang cung cấp mặt hàng nào hay không
        /// </summary>
        /// <param name="id">id nhà cung cấp</param>
        /// <returns></returns>
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        /// <summary>
        /// tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// tìm kiếm và lấy danh sách nhà cung cấp
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// tìm kiếm và láy danh sách loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// tìm kiếm và láy danh sách nhân viên
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// tìm kiếm và láy danh sách người giao hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// lấy danh sách các thành phố
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
        public static List<Category> ListCategories()
        {
            return categoryDB.List();
        }
        public static List<Supplier> ListSuppliers()
        {
            return supplierDB.List();
        }
        public static List<Customer> ListCustomers()
        {
            return customerDB.List();
        }
        public static List<Shipper> ListShippers()
        {
            return shipperDB.List();
        }
    }
}

