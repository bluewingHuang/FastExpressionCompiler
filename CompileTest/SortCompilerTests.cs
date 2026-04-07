using CompileTest.TestDepency;
using FastExpressionCompiler;
using System.Reflection;

namespace CompileTest
{

    [TestClass]
    public class SortCompilerTests
    {
        /// <summary>
        /// TestPass: Compile a sorting function for sorting integers in ascending order, and verify that it correctly sorts the data.
        /// </summary>
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Compile_ShouldSortIntAscending_Pass(bool UseFastExpressionCompiler, bool useHeapSort)
        {
            var compiler = new SortCompiler()
            {
                Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
                FieldDataTypes = new[] { EnumFieldDataType.INT },
                Capacity = 8
            };

            Action<SortData, int, int, int> sortFun = null;
            if (useHeapSort)
                sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            else
                sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            var sortData = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("Age", 3, 4, 1, 2) },
                RowNumbers = new[] { 0, 1, 2, 3 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { true },
                Begin = 0,
                End = 3,
                SortFun = sortFun
            };

            sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

            CollectionAssert.AreEqual(new[] { 2, 3, 0, 1 }, sortData.RowNumbers);
        }

        private static ColumnData<int?> CreateIntColumn(string name, params int?[] values)
        {
            var column = new ColumnData<int?>(EnumFieldDataType.INT, capacity: 8) { Name = name };
            for (var i = 0; i < values.Length; i++)
            {
                column.Set(0, i, values[i]);
            }

            return column;
        }
    
        /// <summary>
        /// TestPass: Compile a sorting function for sorting integers in ascending order, and verify that it correctly sorts the data.
        /// </summary>
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Compile_ShouldSortStringAscending_Pass(bool UseFastExpressionCompiler, bool useHeapSort)
        {
            var compiler = new SortCompiler()
            {
                Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
                FieldDataTypes = new[] { EnumFieldDataType.STRING },
                Capacity = 8
            };

            Action<SortData, int, int, int> sortFun = null;
            if (useHeapSort)
                sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            else
                sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            var sortData = new SortData
            {
                Columns = new ColumnDataBase[] { CreateStringColumn("UserName", "cc", "dd", "aa", "bb") },
                RowNumbers = new[] { 0, 1, 2, 3 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { true },
                Begin = 0,
                End = 3,
                SortFun = sortFun,
                StringComparer = new OracleStringComparer()
            };

            sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

            CollectionAssert.AreEqual(new[] { 2, 3, 0, 1 }, sortData.RowNumbers);
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Compile_TwoRow2_ShouldSortStringAscending(bool UseFastExpressionCompiler, bool useHeapSort)
        {
            var compiler = new SortCompiler()
            {
                Orders = new[] { new OrderInfo { Field = "UserName", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST }, 
                    new OrderInfo { Field = "UserName2", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
                FieldDataTypes = new[] { EnumFieldDataType.STRING, EnumFieldDataType.STRING },
                Capacity = 8
            };

            Action<SortData, int, int, int> sortFun = null;
            if (useHeapSort)
                sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            else
                sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
            var sortData = new SortData
            {
                Columns = new ColumnDataBase[] { CreateStringColumn("UserName", "bb", "bb", "aa", "aa"),
                    CreateStringColumn("UserName2", "dd", "cc", "dd", "cc") },
                RowNumbers = new[] { 0, 1, 2, 3 },
                //NilValues = new[,] { { 1, -1 }, { 2, -1 } },
                NilValues = new[,] { { 0, -1 }, { 1, -1 } },
                isAsc = new[] { true, true },
                Begin = 0,
                End = 3,
                SortFun = sortFun,
                StringComparer = new OracleStringComparer()
            };

            sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

            CollectionAssert.AreEqual(new[] { 3, 2, 1, 0 }, sortData.RowNumbers);
        }

        private static ColumnData<string> CreateStringColumn(string name, params string[] values)
        {
            var column = new ColumnData<string>(EnumFieldDataType.STRING, capacity: 50) { Name = name };
            for (var i = 0; i < values.Length; i++)
            {
                column.Set(0, i, values[i]);
            }

            return column;
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CompileComparisonFunction_ShouldReturnExpectedSign_ForDescendingInt(bool UseFastExpressionCompiler)
        {
            var compiler = new SortCompiler()
            {
                Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.DESC, NilMode = EnumNilMode.LAST } },
                FieldDataTypes = new[] { EnumFieldDataType.INT },
                Capacity = 8
            };

            var comparer = compiler.CompileComparisonFunction(UseFastExpressionCompiler);
            var sortData = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };

            Assert.AreEqual(-1, comparer(sortData, 0, 1));
            Assert.AreEqual(1, comparer(sortData, 1, 0));
        }
    }
}
