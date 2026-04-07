namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ColumnDataBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EnumFieldDataType DataType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Capacity => 100;

        public virtual void Set(int listIndex, int arrayIndex, object value)
        {

        }

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listIndex"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public virtual (int[], int[]) Sort(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params)
        {
            return (new int[0], new int[0]);
        }
        #endregion

    }
}
