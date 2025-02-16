namespace SV21T1020096.DataLayers
{
    public interface ISimpleSelectDAL<T> where T : class
    {
        /// <summary>
        /// Select toàn bộ dữ liệu của bảng
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
