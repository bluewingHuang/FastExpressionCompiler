using CompileTest.TestDepency;
using FastExpressionCompiler;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace CompileTest
{

    [TestClass]
    public class SimpleSortCompilerTests
    {
        ///// <summary>
        ///// TestPass: Compile a sorting function for sorting integers in ascending order, and verify that it correctly sorts the data.
        ///// </summary>
        //[TestMethod]
        //[DataRow(true, true)]
        //[DataRow(true, false)]
        //[DataRow(false, true)]
        //[DataRow(false, false)]
        //public void Compile_ShouldSortIntAscending_Pass(bool UseFastExpressionCompiler, bool useHeapSort)
        //{
        //    var compiler = new SortCompiler()
        //    {
        //        Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
        //        FieldDataTypes = new[] { EnumFieldDataType.INT },
        //        Capacity = 8
        //    };

        //    Action<SortData, int, int, int> sortFun = null;
        //    if (useHeapSort)
        //        sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    else
        //        sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    var sortData = new SortData
        //    {
        //        Columns = new ColumnDataBase[] { CreateIntColumn("Age", 3, 4, 1, 2) },
        //        RowNumbers = new[] { 0, 1, 2, 3 },
        //        NilValues = new[,] { { 1, -1 } },
        //        isAsc = new[] { true },
        //        Begin = 0,
        //        End = 3,
        //        SortFun = sortFun
        //    };

        //    sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

        //    CollectionAssert.AreEqual(new[] { 2, 3, 0, 1 }, sortData.RowNumbers);
        //}

        private static ColumnData<int?> CreateIntColumn(string name, params int?[] values)
        {
            var column = new ColumnData<int?>(EnumFieldDataType.INT, capacity: 8) { Name = name };
            for (var i = 0; i < values.Length; i++)
            {
                column.Set(0, i, values[i]);
            }

            return column;
        }

        ///// <summary>
        ///// TestPass: Compile a sorting function for sorting integers in ascending order, and verify that it correctly sorts the data.
        ///// </summary>
        //[TestMethod]
        //[DataRow(true, true)]
        //[DataRow(true, false)]
        //[DataRow(false, true)]
        //[DataRow(false, false)]
        //public void Compile_ShouldSortStringAscending_Pass(bool UseFastExpressionCompiler, bool useHeapSort)
        //{
        //    var compiler = new SortCompiler()
        //    {
        //        Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
        //        FieldDataTypes = new[] { EnumFieldDataType.STRING },
        //        Capacity = 8
        //    };

        //    Action<SortData, int, int, int> sortFun = null;
        //    if (useHeapSort)
        //        sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    else
        //        sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    var sortData = new SortData
        //    {
        //        Columns = new ColumnDataBase[] { CreateStringColumn("UserName", "cc", "dd", "aa", "bb") },
        //        RowNumbers = new[] { 0, 1, 2, 3 },
        //        NilValues = new[,] { { 1, -1 } },
        //        isAsc = new[] { true },
        //        Begin = 0,
        //        End = 3,
        //        SortFun = sortFun,
        //        StringComparer = new OracleStringComparer()
        //    };

        //    sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

        //    CollectionAssert.AreEqual(new[] { 2, 3, 0, 1 }, sortData.RowNumbers);
        //}

        //[TestMethod]
        //[DataRow(true, true)]
        //[DataRow(true, false)]
        //[DataRow(false, true)]
        //[DataRow(false, false)]
        //public void Compile_TwoRow2_ShouldSortStringAscending(bool UseFastExpressionCompiler, bool useHeapSort)
        //{
        //    var compiler = new SortCompiler()
        //    {
        //        Orders = new[] { new OrderInfo { Field = "UserName", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST }, 
        //            new OrderInfo { Field = "UserName2", Order = EnumOrderMode.ASC, NilMode = EnumNilMode.LAST } },
        //        FieldDataTypes = new[] { EnumFieldDataType.STRING, EnumFieldDataType.STRING },
        //        Capacity = 8
        //    };

        //    Action<SortData, int, int, int> sortFun = null;
        //    if (useHeapSort)
        //        sortFun = compiler.CompileHeap(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    else
        //        sortFun = compiler.Compile(UseFastExpressionCompiler, true, CompilerFlags.EnableDelegateDebugInfo);
        //    var sortData = new SortData
        //    {
        //        Columns = new ColumnDataBase[] { CreateStringColumn("UserName", "bb", "bb", "aa", "aa"),
        //            CreateStringColumn("UserName2", "dd", "cc", "dd", "cc") },
        //        RowNumbers = new[] { 0, 1, 2, 3 },
        //        //NilValues = new[,] { { 1, -1 }, { 2, -1 } },
        //        NilValues = new[,] { { 0, -1 }, { 1, -1 } },
        //        isAsc = new[] { true, true },
        //        Begin = 0,
        //        End = 3,
        //        SortFun = sortFun,
        //        StringComparer = new OracleStringComparer()
        //    };

        //    sortFun(sortData, 0, sortData.RowNumbers.Length - 1, 16);

        //    CollectionAssert.AreEqual(new[] { 3, 2, 1, 0 }, sortData.RowNumbers);
        //}

        //private static ColumnData<string> CreateStringColumn(string name, params string[] values)
        //{
        //    var column = new ColumnData<string>(EnumFieldDataType.STRING, capacity: 50) { Name = name };
        //    for (var i = 0; i < values.Length; i++)
        //    {
        //        column.Set(0, i, values[i]);
        //    }

        //    return column;
        //}


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


        [TestMethod]
        public void CompileComparisonFunction2_ShouldReturnExpectedSign_ForDescendingInt()
        {
            int Capacity = 8;
            var Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.DESC, NilMode = EnumNilMode.LAST } };
            var FieldDataTypes = new[] { EnumFieldDataType.INT };

            var sortData = Parameter(typeof(SortData), "sortData");
            var leftIndex = Parameter(typeof(int), "leftIndex");
            var rightIndex = Parameter(typeof(int), "rightIndex");
            LabelTarget endMain = Label(typeof(int), "endMain");

            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> vars = new List<ParameterExpression>();
            //init(vars, exps);


            var left_ListIndex =Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex =Parameter(typeof(int), "left_ArrayIndex");
            var right_ListIndex =Parameter(typeof(int), "right_ListIndex");
            var right_ArrayIndex =Parameter(typeof(int), "right_ArrayIndex");
            var compareResult =Parameter(typeof(int), "compareResult");
            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex, right_ListIndex, right_ArrayIndex, compareResult });

            var columns = new ParameterExpression[Orders.Length];
            var leftVars = new ParameterExpression[Orders.Length];
            var rightVars = new ParameterExpression[Orders.Length];
            for (int k = 0; k < Orders.Length; ++k)
            {
                columns[k] =Variable(typeof(ColumnData<int?>), $"column_{k}");
                leftVars[k] =Parameter(typeof(int?), $"left_{k}");
                rightVars[k] =Parameter(typeof(int?), $"right_{k}");
                vars.Add(leftVars[k]);
                vars.Add(rightVars[k]);
                vars.Add(columns[k]);
                exps.Add(Expression.Assign(columns[k],Convert(Expression.ArrayAccess(Expression.Field(sortData, "Columns"),Constant(k)), typeof(ColumnData<int?>))));
            }

            //exps.AddRange(makeCondition(leftIndex, rightIndex,Return(endMain,Constant(1)),Return(endMain,Constant(-1))));

            Expression breakExp =Return(endMain,Constant(1));
            Expression continueExp =Return(endMain,Constant(-1));
            exps.Add(Expression.Assign(left_ListIndex,Divide(leftIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(left_ArrayIndex,Modulo(leftIndex,Constant(Capacity))));

            exps.Add(Expression.Assign(right_ListIndex,Divide(rightIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(right_ArrayIndex,Modulo(rightIndex,Constant(Capacity))));


            for (int k = 0; k < Orders.Length; ++k)
            {
                var dataArrayList =PropertyOrField(Expression.PropertyOrField(columns[k], "Datas"), "DataArrayList");

                exps.Add(Expression.Assign(leftVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));
                exps.Add(Expression.Assign(rightVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", right_ListIndex), right_ArrayIndex)));

                var leftNil =IfThen(Expression.NotEqual(rightVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(0)),Constant(-1)), continueExp, breakExp));
                var rightNil =IfThen(Expression.NotEqual(leftVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(1)),Constant(-1)), continueExp, breakExp));

                var isAsc =ArrayAccess(Expression.Field(sortData, "isAsc"),Constant(k));
                List<Expression> allNotNil = new List<Expression>();
                MethodInfo compareMethod = typeof(OracleStringComparer).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
                //Expression stringCompare =Call(Expression.Field(sortData, "StringComparer"), compareMethod, leftVars[k], rightVars[k]);

                //if (FieldDataTypes[k] == EnumFieldDataType.STRING)
                //    //allNotNil.Add(Expression.Assign(compareResult, stringCompare));
                //    //if (Orders[k].Order == EnumOrderMode.ASCCH || Orders[k].Order == EnumOrderMode.DESCCH)
                //    //    allNotNil.Add(Expression.Assign(compareResult, chineseStringCompare(leftVars[k], rightVars[k])));
                //    //else
                //    //    allNotNil.Add(Expression.Assign(compareResult, stringCompare(leftVars[k], rightVars[k])));
                //else
                //{
                //    var method = typeof(int?).GetMethod("CompareTo", new[] { typeof(int?) });
                //    allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));
                //}
                var method = typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
                allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));

                allNotNil.Add(Expression.IfThen(Expression.Not(isAsc),Assign(compareResult,Negate(compareResult))));
                allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(-1)), continueExp));

                if (k < Orders.Length - 1)
                    allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(1)), breakExp));


                var exp =IfThenElse(Expression.Equal(leftVars[k],Constant(null)), leftNil,IfThenElse(Expression.Equal(rightVars[k],Constant(null)), rightNil,Block(allNotNil.ToArray())));
                exps.Add(exp);
            }
            exps.Add(breakExp);

            exps.Add(Expression.Label(endMain,Constant(-1)));
            BlockExpression block =Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex);
            expr.PrintCSharp();

            var data1 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };
            var data2 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };


            var fs = expr.CompileSys();
            Console.WriteLine("-----------------test1---------------------");
            fs.PrintIL();
            var resultSys = fs(data1, 0, 1);
            Console.WriteLine($"resultSys:{resultSys}");
            Assert.AreEqual(-1, resultSys);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            Console.WriteLine("-----------------test2---------------------");
            ff.PrintIL();
            var resultFast = ff(data2, 0, 1);
            Console.WriteLine($"resultFast:{resultFast}");
            Assert.AreEqual(-1, resultFast);

            //t.AreEqual(data1, data2);
            Assert.AreEqual(resultSys, resultFast);


            //if (UseFastExpressionCompiler)
            //{
            //    var re = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileFast();
            //    return re;
            //}
            //else
            //{
            //    var re =Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileSys();
            //    return re;
            //}

            //var comparer = compiler.CompileComparisonFunction(UseFastExpressionCompiler);
            //var sortData = new SortData
            //{
            //    Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
            //    RowNumbers = new[] { 0, 1 },
            //    NilValues = new[,] { { 1, -1 } },
            //    isAsc = new[] { false },
            //    StringComparer = new OracleStringComparer(),
            //    //ChineseStringComparer = new ChineseStringComparer()
            //};

            //Assert.AreEqual(-1, comparer(sortData, 0, 1));
            //Assert.AreEqual(1, comparer(sortData, 1, 0));
        }


        [TestMethod]
        public void CompileComparisonFunction3_ShouldReturnExpectedSign_ForDescendingInt()
        {
            int Capacity = 8;
            var Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.DESC, NilMode = EnumNilMode.LAST } };
            var FieldDataTypes = new[] { EnumFieldDataType.INT };

            var sortData = Parameter(typeof(SortData), "sortData");
            var leftIndex = Parameter(typeof(int), "leftIndex");
            var rightIndex = Parameter(typeof(int), "rightIndex");
            LabelTarget endMain = Label(typeof(int), "endMain");

            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> vars = new List<ParameterExpression>();
            //init(vars, exps);


            var left_ListIndex =Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex =Parameter(typeof(int), "left_ArrayIndex");
            var right_ListIndex =Parameter(typeof(int), "right_ListIndex");
            var right_ArrayIndex =Parameter(typeof(int), "right_ArrayIndex");
            var compareResult =Parameter(typeof(int), "compareResult");
            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex, right_ListIndex, right_ArrayIndex, compareResult });

            var columns = new ParameterExpression[Orders.Length];
            var leftVars = new ParameterExpression[Orders.Length];
            var rightVars = new ParameterExpression[Orders.Length];
            for (int k = 0; k < Orders.Length; ++k)
            {
                columns[k] =Variable(typeof(ColumnData<int?>), $"column_{k}");
                leftVars[k] =Parameter(typeof(int?), $"left_{k}");
                rightVars[k] =Parameter(typeof(int?), $"right_{k}");
                vars.Add(leftVars[k]);
                vars.Add(rightVars[k]);
                vars.Add(columns[k]);
                exps.Add(Expression.Assign(columns[k],Convert(Expression.ArrayAccess(Expression.Field(sortData, "Columns"),Constant(k)), typeof(ColumnData<int?>))));
            }

            //exps.AddRange(makeCondition(leftIndex, rightIndex,Return(endMain,Constant(1)),Return(endMain,Constant(-1))));

            Expression breakExp =Return(endMain,Constant(1));
            Expression continueExp =Return(endMain,Constant(-1));
            exps.Add(Expression.Assign(left_ListIndex,Divide(leftIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(left_ArrayIndex,Modulo(leftIndex,Constant(Capacity))));

            exps.Add(Expression.Assign(right_ListIndex,Divide(rightIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(right_ArrayIndex,Modulo(rightIndex,Constant(Capacity))));


            for (int k = 0; k < Orders.Length; ++k)
            {
                var dataArrayList =PropertyOrField(Expression.PropertyOrField(columns[k], "Datas"), "DataArrayList");

                exps.Add(Expression.Assign(leftVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));
                exps.Add(Expression.Assign(rightVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", right_ListIndex), right_ArrayIndex)));

                var leftNil =IfThen(Expression.NotEqual(rightVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(0)),Constant(-1)), continueExp, breakExp));
                var rightNil =IfThen(Expression.NotEqual(leftVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(1)),Constant(-1)), continueExp, breakExp));

                var isAsc =ArrayAccess(Expression.Field(sortData, "isAsc"),Constant(k));
                List<Expression> allNotNil = new List<Expression>();
                MethodInfo compareMethod = typeof(OracleStringComparer).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
                //Expression stringCompare =Call(Expression.Field(sortData, "StringComparer"), compareMethod, leftVars[k], rightVars[k]);

                //if (FieldDataTypes[k] == EnumFieldDataType.STRING)
                //    //allNotNil.Add(Expression.Assign(compareResult, stringCompare));
                //    //if (Orders[k].Order == EnumOrderMode.ASCCH || Orders[k].Order == EnumOrderMode.DESCCH)
                //    //    allNotNil.Add(Expression.Assign(compareResult, chineseStringCompare(leftVars[k], rightVars[k])));
                //    //else
                //    //    allNotNil.Add(Expression.Assign(compareResult, stringCompare(leftVars[k], rightVars[k])));
                //else
                //{
                //    var method = typeof(int?).GetMethod("CompareTo", new[] { typeof(int?) });
                //    allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));
                //}
                var method = typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
                allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));

                allNotNil.Add(Expression.IfThen(Expression.Not(isAsc),Assign(compareResult,Negate(compareResult))));
                allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(-1)), continueExp));

                if (k < Orders.Length - 1)
                    allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(1)), breakExp));


                var exp =IfThenElse(Expression.Equal(leftVars[k],Constant(null)), leftNil,IfThenElse(Expression.Equal(rightVars[k],Constant(null)), rightNil,Block(allNotNil.ToArray())));
                exps.Add(exp);
            }
            exps.Add(breakExp);

            exps.Add(Expression.Label(endMain,Constant(-1)));
            BlockExpression block =Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex);
            expr.PrintCSharp();

            var data1 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };
            var data2 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };


            var fs = expr.CompileSys();
            Console.WriteLine("-----------------test1---------------------");
            fs.PrintIL();
            var resultSys = fs(data1, 0, 1);
            Console.WriteLine($"resultSys:{resultSys}");
            Assert.AreEqual(1, resultSys);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            Console.WriteLine("-----------------test2---------------------");
            ff.PrintIL();
            var resultFast = ff(data2, 0, 1);
            Console.WriteLine($"resultFast:{resultFast}");
            Assert.AreEqual(1, resultFast);

            //t.AreEqual(data1, data2);
            Assert.AreEqual(resultSys, resultFast);


            //if (UseFastExpressionCompiler)
            //{
            //    var re = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileFast();
            //    return re;
            //}
            //else
            //{
            //    var re =Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileSys();
            //    return re;
            //}

            //var comparer = compiler.CompileComparisonFunction(UseFastExpressionCompiler);
            //var sortData = new SortData
            //{
            //    Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
            //    RowNumbers = new[] { 0, 1 },
            //    NilValues = new[,] { { 1, -1 } },
            //    isAsc = new[] { false },
            //    StringComparer = new OracleStringComparer(),
            //    //ChineseStringComparer = new ChineseStringComparer()
            //};

            //Assert.AreEqual(-1, comparer(sortData, 0, 1));
            //Assert.AreEqual(1, comparer(sortData, 1, 0));
        }


        [TestMethod]
        public void CompileComparisonFunction4_ShouldReturnExpectedSign_ForDescendingInt()
        {
            int Capacity = 8;
            var Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.DESC, NilMode = EnumNilMode.LAST } };
            var FieldDataTypes = new[] { EnumFieldDataType.INT };

            var sortData = Parameter(typeof(SortData), "sortData");
            var leftIndex = Parameter(typeof(int), "leftIndex");
            var rightIndex = Parameter(typeof(int), "rightIndex");
            LabelTarget endMain = Label(typeof(int), "endMain");

            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> vars = new List<ParameterExpression>();
            //init(vars, exps);


            var left_ListIndex =Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex =Parameter(typeof(int), "left_ArrayIndex");
            var right_ListIndex =Parameter(typeof(int), "right_ListIndex");
            var right_ArrayIndex =Parameter(typeof(int), "right_ArrayIndex");
            var compareResult =Parameter(typeof(int), "compareResult");
            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex, right_ListIndex, right_ArrayIndex, compareResult });

            var columns = new ParameterExpression[Orders.Length];
            var leftVars = new ParameterExpression[Orders.Length];
            var rightVars = new ParameterExpression[Orders.Length];
            for (int k = 0; k < Orders.Length; ++k)
            {
                columns[k] =Variable(typeof(ColumnData<int?>), $"column_{k}");
                leftVars[k] =Parameter(typeof(int?), $"left_{k}");
                rightVars[k] =Parameter(typeof(int?), $"right_{k}");
                vars.Add(leftVars[k]);
                vars.Add(rightVars[k]);
                vars.Add(columns[k]);
                exps.Add(Expression.Assign(columns[k],Convert(Expression.ArrayAccess(Expression.Field(sortData, "Columns"),Constant(k)), typeof(ColumnData<int?>))));
            }

            //exps.AddRange(makeCondition(leftIndex, rightIndex,Return(endMain,Constant(1)),Return(endMain,Constant(-1))));

            Expression breakExp =Return(endMain,Constant(1));
            Expression continueExp =Return(endMain,Constant(-1));
            exps.Add(Expression.Assign(left_ListIndex,Divide(leftIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(left_ArrayIndex,Modulo(leftIndex,Constant(Capacity))));

            exps.Add(Expression.Assign(right_ListIndex,Divide(rightIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(right_ArrayIndex,Modulo(rightIndex,Constant(Capacity))));


            for (int k = 0; k < Orders.Length; ++k)
            {
                var dataArrayList =PropertyOrField(Expression.PropertyOrField(columns[k], "Datas"), "DataArrayList");

                exps.Add(Expression.Assign(leftVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));
                //exps.Add(Expression.Assign(rightVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", right_ListIndex), right_ArrayIndex)));

                //var leftNil =IfThen(Expression.NotEqual(rightVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(0)),Constant(-1)), continueExp, breakExp));
                //var rightNil =IfThen(Expression.NotEqual(leftVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(1)),Constant(-1)), continueExp, breakExp));

                //var isAsc =ArrayAccess(Expression.Field(sortData, "isAsc"),Constant(k));
                //List<Expression> allNotNil = new List<Expression>();
                //var method = typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
                //allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));

                //allNotNil.Add(Expression.IfThen(Expression.Not(isAsc),Assign(compareResult,Negate(compareResult))));
                //allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(-1)), continueExp));

                //if (k < Orders.Length - 1)
                //    allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(1)), breakExp));


                //var exp =IfThenElse(Expression.Equal(leftVars[k],Constant(null)), leftNil,IfThenElse(Expression.Equal(rightVars[k],Constant(null)), rightNil,Block(allNotNil.ToArray())));
                //exps.Add(exp);
            }
            //exps.Add(breakExp);

            exps.Add(Expression.Label(endMain,Constant(-1)));
            BlockExpression block =Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex);
            expr.PrintCSharp();

            var data1 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };
            var data2 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };


            var fs = expr.CompileSys();
            Console.WriteLine("-----------------test1---------------------");
            fs.PrintIL();
            var resultSys = fs(data1, 0, 1);
            Console.WriteLine($"resultSys:{resultSys}");
            //Assert.AreEqual(1, resultSys);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            Console.WriteLine("-----------------test2---------------------");
            ff.PrintIL();
            var resultFast = ff(data2, 0, 1);
            Console.WriteLine($"resultFast:{resultFast}");
            //Assert.AreEqual(1, resultFast);

            //t.AreEqual(data1, data2);
            //Assert.AreEqual(resultSys, resultFast);


            //if (UseFastExpressionCompiler)
            //{
            //    var re = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileFast();
            //    return re;
            //}
            //else
            //{
            //    var re =Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileSys();
            //    return re;
            //}

            //var comparer = compiler.CompileComparisonFunction(UseFastExpressionCompiler);
            //var sortData = new SortData
            //{
            //    Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
            //    RowNumbers = new[] { 0, 1 },
            //    NilValues = new[,] { { 1, -1 } },
            //    isAsc = new[] { false },
            //    StringComparer = new OracleStringComparer(),
            //    //ChineseStringComparer = new ChineseStringComparer()
            //};

            //Assert.AreEqual(-1, comparer(sortData, 0, 1));
            //Assert.AreEqual(1, comparer(sortData, 1, 0));
        }


        [TestMethod]
        public void CompileComparisonFunction5_ShouldReturnExpectedSign_ForDescendingInt()
        {
            int Capacity = 8;
            var Orders = new[] { new OrderInfo { Field = "f1", Order = EnumOrderMode.DESC, NilMode = EnumNilMode.LAST } };
            var FieldDataTypes = new[] { EnumFieldDataType.INT };

            var sortData = Parameter(typeof(SortData), "sortData");
            var leftIndex = Parameter(typeof(int), "leftIndex");
            var rightIndex = Parameter(typeof(int), "rightIndex");
            LabelTarget endMain = Label(typeof(int), "endMain");

            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> vars = new List<ParameterExpression>();
            //init(vars, exps);


            var left_ListIndex =Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex =Parameter(typeof(int), "left_ArrayIndex");
            var right_ListIndex =Parameter(typeof(int), "right_ListIndex");
            var right_ArrayIndex =Parameter(typeof(int), "right_ArrayIndex");
            var compareResult =Parameter(typeof(int), "compareResult");
            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex, right_ListIndex, right_ArrayIndex, compareResult });

            var columns = new ParameterExpression[Orders.Length];
            var leftVars = new ParameterExpression[Orders.Length];
            var rightVars = new ParameterExpression[Orders.Length];
            for (int k = 0; k < Orders.Length; ++k)
            {
                columns[k] =Variable(typeof(ColumnData<int?>), $"column_{k}");
                leftVars[k] =Parameter(typeof(int?), $"left_{k}");
                rightVars[k] =Parameter(typeof(int?), $"right_{k}");
                vars.Add(leftVars[k]);
                vars.Add(rightVars[k]);
                vars.Add(columns[k]);
                exps.Add(Expression.Assign(columns[k],Convert(Expression.ArrayAccess(Expression.Field(sortData, "Columns"),Constant(k)), typeof(ColumnData<int?>))));
            }

            //exps.AddRange(makeCondition(leftIndex, rightIndex,Return(endMain,Constant(1)),Return(endMain,Constant(-1))));

            Expression breakExp =Return(endMain,Constant(1));
            Expression continueExp =Return(endMain,Constant(-1));
            exps.Add(Expression.Assign(left_ListIndex,Divide(leftIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(left_ArrayIndex,Modulo(leftIndex,Constant(Capacity))));

            exps.Add(Expression.Assign(right_ListIndex,Divide(rightIndex,Constant(Capacity))));
            exps.Add(Expression.Assign(right_ArrayIndex,Modulo(rightIndex,Constant(Capacity))));


            for (int k = 0; k < Orders.Length; ++k)
            {
                var dataArrayList =PropertyOrField(Expression.PropertyOrField(columns[k], "Datas"), "DataArrayList");

                exps.Add(Expression.Assign(leftVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));
                //exps.Add(Expression.Assign(rightVars[k],ArrayAccess(Expression.Property(dataArrayList, "Item", right_ListIndex), right_ArrayIndex)));

                //var leftNil =IfThen(Expression.NotEqual(rightVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(0)),Constant(-1)), continueExp, breakExp));
                //var rightNil =IfThen(Expression.NotEqual(leftVars[k],Constant(null)),IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"),Constant(k),Constant(1)),Constant(-1)), continueExp, breakExp));

                //var isAsc =ArrayAccess(Expression.Field(sortData, "isAsc"),Constant(k));
                //List<Expression> allNotNil = new List<Expression>();
                //var method = typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
                //allNotNil.Add(Expression.Assign(compareResult,Call(Expression.PropertyOrField(leftVars[k], "Value"), method,PropertyOrField(rightVars[k], "Value"))));

                //allNotNil.Add(Expression.IfThen(Expression.Not(isAsc),Assign(compareResult,Negate(compareResult))));
                //allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(-1)), continueExp));

                //if (k < Orders.Length - 1)
                //    allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult,Constant(1)), breakExp));


                //var exp =IfThenElse(Expression.Equal(leftVars[k],Constant(null)), leftNil,IfThenElse(Expression.Equal(rightVars[k],Constant(null)), rightNil,Block(allNotNil.ToArray())));
                //exps.Add(exp);
            }
            //exps.Add(breakExp);

            exps.Add(Expression.Label(endMain,Constant(-1)));
            BlockExpression block =Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex);
            expr.PrintCSharp();

            var data1 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };
            var data2 = new SortData
            {
                Columns = new ColumnDataBase[] { CreateIntColumn("age", 1, 1) },
                RowNumbers = new[] { 0, 1 },
                NilValues = new[,] { { 1, -1 } },
                isAsc = new[] { false },
                StringComparer = new OracleStringComparer(),
                //ChineseStringComparer = new ChineseStringComparer()
            };


            var fs = expr.CompileSys();
            Console.WriteLine("-----------------test1---------------------");
            fs.PrintIL();
            var resultSys = fs(data1, 0, 1);
            Console.WriteLine($"resultSys:{resultSys}");
            //Assert.AreEqual(1, resultSys);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            Console.WriteLine("-----------------test2---------------------");
            ff.PrintIL();
            var resultFast = ff(data2, 0, 1);
            Console.WriteLine($"resultFast:{resultFast}");
            //Assert.AreEqual(1, resultFast);

            //t.AreEqual(data1, data2);
            //Assert.AreEqual(resultSys, resultFast);


            //if (UseFastExpressionCompiler)
            //{
            //    var re = Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileFast();
            //    return re;
            //}
            //else
            //{
            //    var re =Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileSys();
            //    return re;
            //}

            //var comparer = compiler.CompileComparisonFunction(UseFastExpressionCompiler);
            //var sortData = new SortData
            //{
            //    Columns = new ColumnDataBase[] { CreateIntColumn("age", 3, 1) },
            //    RowNumbers = new[] { 0, 1 },
            //    NilValues = new[,] { { 1, -1 } },
            //    isAsc = new[] { false },
            //    StringComparer = new OracleStringComparer(),
            //    //ChineseStringComparer = new ChineseStringComparer()
            //};

            //Assert.AreEqual(-1, comparer(sortData, 0, 1));
            //Assert.AreEqual(1, comparer(sortData, 1, 0));
        }

        [TestMethod]
        public void CompileComparisonFunction6_ShouldReturnExpectedSign_ForDescendingInt()
        {
            List<Expression> exps = new List<Expression>();
            var myValueList = Parameter(typeof(MyValueList<int?>), "myValueList");
            List<ParameterExpression> vars = new List<ParameterExpression>();
            var left_ListIndex = Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex = Parameter(typeof(int), "left_ArrayIndex");

            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex });
            var leftVars = new ParameterExpression[1];
            leftVars[0] =Parameter(typeof(int?), $"left_{0}");
            vars.Add(leftVars[0]);
            var dataArrayList = PropertyOrField(myValueList, "DataArrayList");
            exps.Add(Assign(left_ListIndex, Constant(0)));
            exps.Add(Assign(left_ArrayIndex, Constant(0)));
            exps.Add(Assign(leftVars[0], ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));

            BlockExpression block =Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Action<MyValueList<int?>>>(block, myValueList);
            expr.PrintCSharp();

            MyValueList<int?> data1 = new CompileTest.MyValueList<int?>() {
                DataArrayList = new List<int?[]> { new int?[] { 1 } }
            };
            MyValueList<int?> data2 = new CompileTest.MyValueList<int?>()
            {
                DataArrayList = new List<int?[]> { new int?[] { 1 } }
            };


            var fs = expr.CompileSys();
            fs.PrintIL();
            fs(data1);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            ff.PrintIL();
            ff(data2);
        }

        [TestMethod]
        public void CompileComparisonFunction7_ShouldReturnExpectedSign_ForDescendingInt()
        {
            List<Expression> exps = new List<Expression>();
            var dataArrayList = Parameter(typeof(List<int?[]>), "dataArrayList");
            List<ParameterExpression> vars = new List<ParameterExpression>();
            var left_ListIndex = Parameter(typeof(int), "left_ListIndex");
            var left_ArrayIndex = Parameter(typeof(int), "left_ArrayIndex");

            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex });
            var leftVars = new ParameterExpression[1];
            leftVars[0] = Parameter(typeof(int?), $"left_{0}");
            vars.Add(leftVars[0]);
            exps.Add(Assign(left_ListIndex, Constant(0)));
            exps.Add(Assign(left_ArrayIndex, Constant(0)));
            exps.Add(Assign(leftVars[0], ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));

            BlockExpression block = Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Action<List<int?[]>>>(block, dataArrayList);
            expr.PrintCSharp();

            List<int?[]> data1 = new List<int?[]> { new int?[] { 1 } };
            List<int?[]> data2 = new List<int?[]> { new int?[] { 1 } };


            var fs = expr.CompileSys();
            fs.PrintIL();
            fs(data1);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            ff.PrintIL();
            ff(data2);
        }


        [TestMethod]
        public void CompileComparisonFunction8_ShouldReturnExpectedSign_ForDescendingInt()
        {
            List<Expression> exps = new List<Expression>();
            var dataArrayList = Parameter(typeof(List<int?[]>), "dataArrayList");
            List<ParameterExpression> vars = new List<ParameterExpression>();
            var leftVars = new ParameterExpression[1];
            leftVars[0] = Parameter(typeof(int?), $"left_{0}");
            vars.Add(leftVars[0]);
            exps.Add(Assign(leftVars[0], ArrayAccess(Expression.Property(dataArrayList, "Item", Constant(0)), Constant(0))));

            BlockExpression block = Block(
                vars.ToArray(), exps
            );
            var expr = Lambda<Action<List<int?[]>>>(block, dataArrayList);
            expr.PrintCSharp();

            List<int?[]> data1 = new List<int?[]> { new int?[] { 1 } };
            List<int?[]> data2 = new List<int?[]> { new int?[] { 1 } };


            var fs = expr.CompileSys();
            fs.PrintIL();
            fs(data1);

            var ff = expr.CompileFast(ifFastFailedReturnNull: true);
            //t.IsNotNull(ff);
            Assert.IsNotNull(ff);
            ff.PrintIL();
            ff(data2);
        }
    }

    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MyValueList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T[]> DataArrayList;
    }

}
