namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumFieldDataType
    {
        INT = 0,
        STRING = 1,
        DATETIME = 2,
        FLOAT = 3,
        DOUBLE = 4,
        LONG = 5,
        DECIMAL = 6,
        BOOLEAN = 7,
        BYTE = 8,
        DATE = 9,
        SHORT = 10,
        NONE = 11
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EnumAggType
    {
        /// <summary>
        /// 
        /// </summary>
        none = -1,
        /// <summary>
        /// 
        /// </summary>
        count = 0,
        /// <summary>
        /// 
        /// </summary>
        max = 1,
        /// <summary>
        /// 
        /// </summary>
        min = 2,
        /// <summary>
        /// 
        /// </summary>
        sum = 3,
        /// <summary>
        /// 
        /// </summary>
        avg = 4,
        /// <summary>
        /// 
        /// </summary>
        distinctcount = 5,
        /// <summary>
        /// 
        /// </summary>
        median = 6,
        /// <summary>
        /// 
        /// </summary>
        std = 7,
        /// <summary>
        /// 
        /// </summary>
        atop = 8,
        /// <summary>
        /// 
        /// </summary>
        dtop = 9,
        /// <summary>
        /// 
        /// </summary>
        listagg = 10,
        /// <summary>
        /// 
        /// </summary>
        product = 11,
        /// <summary>
        /// 
        /// </summary>
        countnonull = 12,
        /// <summary>
        /// 
        /// </summary>
        recentRank = 13,
        /// <summary>
        /// 
        /// </summary>
        stds = 14

    }

    public enum EnumOrderMode
    {
        ASC = 0,
        DESC = 1,
        ASCCH = 2,
        DESCCH = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public enum EnumNilMode
    {
        /// <summary>
        /// 
        /// </summary>
        FIRST = 0,
        /// <summary>
        /// 
        /// </summary>
        LAST = 1
    }
}
