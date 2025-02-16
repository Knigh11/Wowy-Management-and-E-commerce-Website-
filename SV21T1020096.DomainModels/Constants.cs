using System.Reflection;

namespace SV21T1020096.DomainModels
{
    public static class Constants
    {
        public const int ORDER_INIT = 1;
        public const int ORDER_ACCEPTED = 2;
        public const int ORDER_SHIPPING = 3;
        public const int ORDER_FINISHED = 4;
        public const int ORDER_CANCEL = -1;
        public const int ORDER_REJECTED = -2;

        public static List<int> GetConstantsValues()
        {
            // Lấy tất cả giá trị của các hằng số trong class này
            return typeof(Constants)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly) // Lọc ra các hằng số
                .Select(fi => (int)fi.GetRawConstantValue()) // Lấy giá trị của từng hằng số
                .ToList();
        }
    }
}
