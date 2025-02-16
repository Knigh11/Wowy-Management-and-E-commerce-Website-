using SV21T1020096.Shop.AppCodes;

namespace SV21T1020096.Shop.Models
{
    /// <summary>
    /// Lưu giữ các thông ti đầu cào sử dụng cho chức ănng tìm kiếm và hiển thị dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// Số dòng hiểi thị trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Chuỗi giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
    }
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
    }
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary> 
        /// Trạng thái của đơn hàng cần tìm 
        /// </summary> 
        public int Status { get; set; } = 0;
        /// <summary> 
        /// Khoảng thời gian cần tìm (chuỗi 2 giá trị ngày có dạng dd/MM/yyyy - dd/MM/yyyy)  /// </summary> 
        public string TimeRange { get; set; } = "";
        /// <summary> 
        /// Lấy thời điểm bắt đầu dựa vào DateRange 
        /// </summary> 
        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[0].Trim().ToDateTime();
                    return value;
                }
                return null;
            }
        }
        /// <summary> 
        /// Lấy thời điểm kết thúc dựa vào DateRange 
        /// (thời điểm kết thúc phải là cuối ngày) 
        /// </summary> 
        public DateTime? ToTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[1].Trim().ToDateTime();
                    if (value.HasValue)
                        value = value.Value.AddMilliseconds(86399998); //86399999
                    return value;
                }
                return null;
            }
        }
    }
}
