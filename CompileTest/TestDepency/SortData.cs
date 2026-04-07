using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    public class SortData
    {
        /// <summary>
        /// 
        /// </summary>
        public ColumnDataBase[] Columns;
        /// <summary>
        /// 
        /// </summary>
        public int[] RowNumbers;
        /// <summary>
        /// 
        /// </summary>
        public int[,] NilValues;

        /// <summary>
        /// 
        /// </summary>
        public bool[] isAsc;

        /// <summary>
        /// 
        /// </summary>
        public Action<SortData, int, int, int> SortFun;

        /// <summary>
        /// 
        /// </summary>
        public Func<SortData, int, int, int> Comparer;

        /// <summary>
        /// 
        /// </summary>
        public OracleStringComparer StringComparer;

        /// <summary>
        /// 
        /// </summary>
        public OracleStringComparer ChineseStringComparer;

        /// <summary>
        /// 
        /// </summary>
        public int Begin;

        /// <summary>
        /// 
        /// </summary>
        public int End;

        /// <summary>
        /// 
        /// </summary>
        public int DepthLimit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool InRange(int begin, int end)
        {
            if (End < begin || end < Begin)
                return false;

            return true;
        }

        #region
        /// <summary>
        /// 
        /// </summary>
        static Random random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int PivotRandom(int begin, int end)
        {
            return random.Next(begin, end);
        }
        #endregion


        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void HeapSort(SortData sortData, Span<int> rowNumbers, Func<SortData, int, int, int> comparer)
        {
            System.Diagnostics.Debug.WriteLine("HeapSort");
            int n = rowNumbers.Length;
            for (int i = n >> 1; i >= 1; i--)
            {
                DownHeap(sortData, rowNumbers, i, n, comparer);
            }

            for (int i = n; i > 1; i--)
            {
                Swap(rowNumbers, 0, i - 1);
                DownHeap(sortData, rowNumbers, 1, i - 1, comparer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void Swap<T>(Span<T> a, int i, int j)
        {
            if (i != j)
            {
                T t = a[i];
                a[i] = a[j];
                a[j] = t;
            }
        }

        public void DownHeap(SortData sortData, Span<int> keys, int i, int n, Func<SortData, int, int, int> comparer)
        {
            int d = keys[i - 1];
            while (i <= n >> 1)
            {
                int child = 2 * i;
                if (child < n && comparer(sortData, keys[child - 1], keys[child]) < 0)
                {
                    child++;
                }

                if (!(comparer(sortData, d, keys[child - 1]) < 0))
                    break;

                keys[i - 1] = keys[child - 1];
                i = child;
            }

            keys[i - 1] = d;
        }
        #endregion
    }
}
