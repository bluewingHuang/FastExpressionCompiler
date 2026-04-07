namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ColumnData<T> : ColumnDataBase
    {

        /// <summary>
        /// 
        /// </summary>
        static Func<object, T> ConvertFunction;

        /// <summary>
        /// 
        /// </summary>
        static ColumnData()
        {
            ColumnData<byte?>.ConvertFunction = (m) => m != null ? Convert.ToByte(m) : null;
            ColumnData<int?>.ConvertFunction = (m) => m != null ? Convert.ToInt32(m) : null;
            ColumnData<double?>.ConvertFunction = (m) => m != null ? Convert.ToDouble(m) : null;
            ColumnData<float?>.ConvertFunction = (m) => m != null ? Convert.ToSingle(m) : null;
            ColumnData<string>.ConvertFunction = (m) => m?.ToString();
            ColumnData<long?>.ConvertFunction = (m) => m != null ? Convert.ToInt64(m) : null;
            ColumnData<DateTime?>.ConvertFunction = (m) => m != null ? Convert.ToDateTime(m) : null;
            ColumnData<bool?>.ConvertFunction = (m) => m != null ? Convert.ToBoolean(m) : null;
            ColumnData<decimal?>.ConvertFunction = (m) => m != null ? Convert.ToDecimal(m) : null;
            ColumnData<short?>.ConvertFunction = (m) => m != null ? Convert.ToInt16(m) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        public ValueList<T> Datas;

        private int _capacity;
        /// <summary>
        /// 
        /// </summary>
        public override int Capacity => _capacity;

        /// <summary>
        /// 
        /// </summary>
        public ColumnData(EnumFieldDataType dataType, int capacity, int initCount = 0)
        {
            DataType = dataType;
            Datas = new ValueList<T>(capacity, initCount);
            _capacity = capacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override void Set(int listIndex, int arrayIndex, object value)
        {
            if (value is T t)
                Datas.SetValue(listIndex, arrayIndex, t);
            else
                Datas.SetValue(listIndex, arrayIndex, ConvertFunction(value));
        }

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listIndex"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public override (int[], int[]) Sort(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params)
        {
            return Datas.Sort(listIndex, arrayIndex, aggtype, Params);
        }
        #endregion

    }
}
