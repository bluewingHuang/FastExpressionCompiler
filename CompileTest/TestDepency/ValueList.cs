using System.Buffers;

namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ValueList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        private int capacity = 100;

        /// <summary>
        /// 
        /// </summary>
        public List<T[]> DataArrayList;

        /// <summary>
        /// 
        /// </summary>
        static Func<int[], int[], EnumAggType, object[], List<T[]>, (int[], int[])> SortFunction;

        /// <summary>
        /// 
        /// </summary>
        static ValueList()
        {

            #region
            ValueList<int?>.SortFunction = (l, a, t, p, list) => SortInt(l, a, t, p, list);
            ValueList<double?>.SortFunction = (l, a, t, p, list) => SortDouble(l, a, t, p, list);
            ValueList<float?>.SortFunction = (l, a, t, p, list) => SortFloat(l, a, t, p, list);
            ValueList<string>.SortFunction = (l, a, t, p, list) => SortString(l, a, t, p, list);
            ValueList<long?>.SortFunction = (l, a, t, p, list) => SortLong(l, a, t, p, list);
            ValueList<DateTime?>.SortFunction = (l, a, t, p, list) => SortDateTime(l, a, t, p, list);
            ValueList<decimal?>.SortFunction = (l, a, t, p, list) => SortDecimal(l, a, t, p, list);
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public ValueList()
        {
            DataArrayList = new List<T[]>() { newArray() };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="rowCount"></param>
        public ValueList(int capacity, int rowCount = 0)
        {
            this.capacity = capacity;
            if (rowCount > 0)
            {
                int len = rowCount / capacity;
                if (rowCount % capacity > 0) len++;
                this.DataArrayList = new List<T[]>(len);
                for (int k = 0; k < len; ++k)
                    DataArrayList.Add(newArray());
            }
            else
                this.DataArrayList = new List<T[]>() { newArray() };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Return();
            DataArrayList = new List<T[]>() { newArray() };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Return()
        {
            DataArrayList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listIndex"></param>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        public void SetValue(int listIndex, int arrayIndex, T value)
        {
            DataArrayList[listIndex][arrayIndex] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private T[] newArray()
        {
            return new T[capacity];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Expansion()
        {
            DataArrayList.Add(newArray());
        }

        #region
        public (int[], int[]) Sort(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params)
        {
            return SortFunction(listIndex, arrayIndex, aggtype, Params, DataArrayList);
        }
        public static (int[], int[]) SortInt(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<int?[]> dataList)
        {
            int order = -1;
            int isNullMax = 1;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();

            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortInt(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortInt(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortDouble(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<double?[]> dataList)
        {
            int order = -1;
            int isNullMax = 1;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();

            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortDouble(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortDouble(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortFloat(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<float?[]> dataList)
        {
            int order = -1;
            int isNullMax = 0;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();
            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortFloat(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortFloat(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortString(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<string[]> dataList)
        {
            int order = -1;
            int isNullMax = 0;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();

            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortString(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortString(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortLong(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<long?[]> dataList)
        {
            int order = -1;
            int isNullMax = 0;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();
            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortLong(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortLong(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortDateTime(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<DateTime?[]> dataList)
        {
            int order = -1;
            int isNullMax = 0;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();
            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortDateTime(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortDateTime(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        public static (int[], int[]) SortDecimal(int[] listIndex, int[] arrayIndex, EnumAggType aggtype, object[] Params, List<decimal?[]> dataList)
        {
            int order = -1;
            int isNullMax = 0;
            if (aggtype == EnumAggType.listagg)
            {
                order = Params.Length > 3 ? (int)Params[3] : order;
                isNullMax = Params.Length > 5 ? (int)Params[5] : isNullMax;
            }
            else if (aggtype == EnumAggType.min || aggtype == EnumAggType.atop)
            {
                order = 0;
                isNullMax = aggtype == EnumAggType.min && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.atop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }
            else if (aggtype == EnumAggType.max || aggtype == EnumAggType.dtop)
            {
                order = 1;
                isNullMax = aggtype == EnumAggType.max && Params.Length > 1 ? int.Parse(Params[1].ToString()) : (aggtype == EnumAggType.dtop && Params.Length > 2 ? int.Parse(Params[2].ToString()) : 1);
            }

            int nullCount = 0;
            if (order == 0 || order == 1)
            {
                for (int i = listIndex.Length - 1; i >= 0; i--)
                {
                    if (dataList[listIndex[i]][arrayIndex[i]] == null)
                    {
                        nullCount++;
                        int j = listIndex.Length - nullCount;
                        var temp = (listIndex[i], arrayIndex[i]);
                        (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                        (listIndex[j], arrayIndex[j]) = temp;
                    }
                }
            }

            int[] notNullListIndex = listIndex.Take(listIndex.Length - nullCount).ToArray();
            int[] notNullArrayIndex = arrayIndex.Take(listIndex.Length - nullCount).ToArray();
            if (notNullListIndex.Length <= 0)
                return (listIndex, arrayIndex);
            if (order == 0)
            {
                QuickSortDecimal(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, false);
                if (isNullMax == 1)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            else if (order == 1)
            {
                QuickSortDecimal(0, notNullListIndex.Length - 1, dataList, notNullListIndex, notNullArrayIndex, true);
                if (isNullMax == 2)
                {
                    listIndex = listIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullListIndex).ToArray();
                    arrayIndex = arrayIndex.Skip(listIndex.Length - nullCount).ToArray().Concat(notNullArrayIndex).ToArray();
                }
                else
                {
                    listIndex = notNullListIndex.Concat(listIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                    arrayIndex = notNullArrayIndex.Concat(arrayIndex.Skip(listIndex.Length - nullCount).ToArray()).ToArray();
                }
            }
            return (listIndex, arrayIndex);
        }

        #region
        public static void QuickSortInt(int low, int high, List<int?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortInt(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortInt(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortDouble(int low, int high, List<double?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortDouble(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortDouble(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortFloat(int low, int high, List<float?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortFloat(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortFloat(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortString(int low, int high, List<string[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? string.Compare(dataList[listIndex[i]][arrayIndex[i]], pivot) > 0 : string.Compare(dataList[listIndex[i]][arrayIndex[i]], pivot) < 0) i++;
                while (desc ? string.Compare(dataList[listIndex[j]][arrayIndex[j]], pivot) < 0 : string.Compare(dataList[listIndex[j]][arrayIndex[j]], pivot) > 0) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortString(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortString(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortLong(int low, int high, List<long?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortLong(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortLong(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortDateTime(int low, int high, List<DateTime?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortDateTime(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortDateTime(i, high, dataList, listIndex, arrayIndex, desc);
        }
        public static void QuickSortDecimal(int low, int high, List<decimal?[]> dataList, int[] listIndex, int[] arrayIndex, bool desc)
        {
            int i = low, j = high;
            var pivotListIndex = listIndex[(i + j) / 2];
            var pivotArrayIndex = arrayIndex[(i + j) / 2];
            var pivot = dataList[pivotListIndex][pivotArrayIndex];

            while (i <= j)
            {
                while (desc ? dataList[listIndex[i]][arrayIndex[i]] > pivot : dataList[listIndex[i]][arrayIndex[i]] < pivot) i++;
                while (desc ? dataList[listIndex[j]][arrayIndex[j]] < pivot : dataList[listIndex[j]][arrayIndex[j]] > pivot) j--;

                if (i <= j)
                {
                    var temp = (listIndex[i], arrayIndex[i]);
                    (listIndex[i], arrayIndex[i]) = (listIndex[j], arrayIndex[j]);
                    (listIndex[j], arrayIndex[j]) = temp;
                    i++;
                    j--;
                }

            }

            if (low < j)
                QuickSortDecimal(low, j, dataList, listIndex, arrayIndex, desc);

            if (i < high)
                QuickSortDecimal(i, high, dataList, listIndex, arrayIndex, desc);
        }
        #endregion
        #endregion
    }
}
