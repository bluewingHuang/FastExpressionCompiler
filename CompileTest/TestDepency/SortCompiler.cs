using FastExpressionCompiler;
using FastExpressionCompiler.ImTools;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CompileTest.TestDepency
{
    /// <summary>
    /// 
    /// </summary>
    public class SortCompiler
    {
        static Type[] ColumnTypes = new Type[] {
          typeof(ColumnData<int?>),
          typeof(ColumnData<string>),
          typeof(ColumnData<DateTime?>),
          typeof(ColumnData<float?>),
          typeof(ColumnData<double?>),
          typeof(ColumnData<long?>),
          typeof(ColumnData<decimal?>),
        };

        static Type[] NullableDataTypes = new Type[] {
          typeof(int?),
          typeof(string),
          typeof(DateTime?),
          typeof(float?),
          typeof(double?),
          typeof(long?),
          typeof(decimal?),
        };

        static Type[] DataTypes = new Type[] {
          typeof(int),
          typeof(string),
          typeof(DateTime),
          typeof(float),
          typeof(double),
          typeof(long),
          typeof(decimal),
        };

        /// <summary>
        /// 
        /// </summary>
        internal OrderInfo[] Orders;
        /// <summary>
        /// 
        /// </summary>
        internal int Capacity;

        /// <summary>
        /// 
        /// </summary>
        internal EnumFieldDataType[] FieldDataTypes;

        #region
        /// <summary>
        /// 
        /// </summary>
        ParameterExpression low;
        ParameterExpression high;
        ParameterExpression depthLimit;
        /// <summary>
        /// 
        /// </summary>
        ParameterExpression sortData;

        /// <summary>
        /// 
        /// </summary>
        ParameterExpression i;
        ParameterExpression j;
        ParameterExpression pivot;

        /// <summary>
        /// 
        /// </summary>
        ParameterExpression[] columns;
        ParameterExpression[] leftVars;
        ParameterExpression[] rightVars;
        ParameterExpression compareResult;

        /// <summary>
        /// 
        /// </summary>
        ParameterExpression left_ListIndex;
        ParameterExpression left_ArrayIndex;
        ParameterExpression right_ListIndex;
        ParameterExpression right_ArrayIndex;

        /// <summary>
        /// 
        /// </summary>
        ParameterExpression comparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vars"></param>
        /// <param name="exps"></param>
        private void init(List<ParameterExpression> vars, List<Expression> exps)
        {
            left_ListIndex = Expression.Parameter(typeof(int), "left_ListIndex");
            left_ArrayIndex = Expression.Parameter(typeof(int), "left_ArrayIndex");
            right_ListIndex = Expression.Parameter(typeof(int), "right_ListIndex");
            right_ArrayIndex = Expression.Parameter(typeof(int), "right_ArrayIndex");
            compareResult = Expression.Parameter(typeof(int), "compareResult");
            vars.AddRange(new ParameterExpression[] { left_ListIndex, left_ArrayIndex, right_ListIndex, right_ArrayIndex, compareResult });

            columns = new ParameterExpression[Orders.Length];
            leftVars = new ParameterExpression[Orders.Length];
            rightVars = new ParameterExpression[Orders.Length];
            for (int k = 0; k < Orders.Length; ++k)
            {
                columns[k] = Expression.Variable(ColumnTypes[(int)FieldDataTypes[k]], $"column_{k}");
                leftVars[k] = Expression.Parameter(NullableDataTypes[(int)FieldDataTypes[k]], $"left_{k}");
                rightVars[k] = Expression.Parameter(NullableDataTypes[(int)FieldDataTypes[k]], $"right_{k}");
                vars.Add(leftVars[k]);
                vars.Add(rightVars[k]);
                vars.Add(columns[k]);
                exps.Add(Expression.Assign(columns[k], Expression.Convert(Expression.ArrayAccess(Expression.Field(sortData, "Columns"), Expression.Constant(k)), ColumnTypes[(int)FieldDataTypes[k]])));
            }
        }
        #endregion

        #region
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action<SortData, int, int, int> Compile(bool UseFastExpressionCompiler = false, bool ifFastFailedReturnNull = false, CompilerFlags flags = CompilerFlags.Default)
        {
            List<ParameterExpression> vars = new List<ParameterExpression>();
            List<Expression> exps = new List<Expression>();

            low = Expression.Parameter(typeof(int), "low");
            high = Expression.Parameter(typeof(int), "high");
            sortData = Expression.Parameter(typeof(SortData), "sortData");
            depthLimit = Expression.Parameter(typeof(int), "depthLimit");
            pivot = Expression.Parameter(typeof(int), "pivot");
            i = Expression.Parameter(typeof(int), "i");
            j = Expression.Parameter(typeof(int), "j");
            var temp = Expression.Parameter(typeof(int), "temp");
            vars.AddRange(new ParameterExpression[] { i, j, pivot, temp });
            init(vars, exps);

            LabelTarget endMain = Expression.Label(typeof(void), "endMain");
            LabelTarget endSub1 = Expression.Label(typeof(void), "endSub1");
            LabelTarget endSub2 = Expression.Label(typeof(void), "endSub2");
            LabelTarget continue1 = Expression.Label(typeof(void), "continue1");
            LabelTarget continue2 = Expression.Label(typeof(void), "continue2");

            exps.Add(Expression.Assign(i, low));
            exps.Add(Expression.Assign(j, high));
            exps.Add(Expression.Assign(pivot, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), Expression.Divide(Expression.Add(i, j), Expression.Constant(2)))));

            exps.Add(Expression.Loop(
                Expression.Block(
                            Expression.IfThen(Expression.GreaterThan(i, j), Expression.Break(endMain)),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i),
                                        pivot,
                                        Expression.Break(endSub1),
                                        Expression.Block(Expression.PostIncrementAssign(i), Expression.Continue(continue1))
                                        ).ToArray()
                                    ),
                                endSub1,
                                continue1
                                ),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        pivot,
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j),
                                        Expression.Break(endSub2),
                                        Expression.Block(Expression.PostDecrementAssign(j), Expression.Continue(continue2))
                                        ).ToArray()
                                    ),
                                endSub2,
                                continue2
                                ),

                            Expression.IfThen(Expression.LessThanOrEqual(i, j), Expression.Block(
                                Expression.Assign(temp, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i), Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j), temp),
                                Expression.PostIncrementAssign(i),
                                Expression.PostDecrementAssign(j)
                                ))
                 ),
                endMain
                ));

            var invokeMethod = typeof(Action<SortData, int, int, int>).GetMethod("Invoke");
            var inRangeMethod = (typeof(SortData)).GetMethod("InRange");

            exps.Add(Expression.IfThen(Expression.AndAlso(Expression.LessThan(low, j), Expression.Call(sortData, inRangeMethod, low, j)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, low, j, depthLimit)));
            exps.Add(Expression.IfThen(Expression.AndAlso(Expression.LessThan(i, high), Expression.Call(sortData, inRangeMethod, i, high)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, i, high, depthLimit)));

            BlockExpression block = Expression.Block(
                vars.ToArray(), exps
            );

            if (UseFastExpressionCompiler)
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).CompileFast(ifFastFailedReturnNull, flags);
                return re;
            }
            else
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).Compile();
                return re;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action<SortData, int, int, int> Compile_Test2(bool UseFastExpressionCompiler = false, bool ifFastFailedReturnNull = false, CompilerFlags flags = CompilerFlags.Default)
        {
            List<ParameterExpression> vars = new List<ParameterExpression>();
            List<Expression> exps = new List<Expression>();

            low = Expression.Parameter(typeof(int), "low");
            high = Expression.Parameter(typeof(int), "high");
            sortData = Expression.Parameter(typeof(SortData), "sortData");
            depthLimit = Expression.Parameter(typeof(int), "depthLimit");
            pivot = Expression.Parameter(typeof(int), "pivot");
            i = Expression.Parameter(typeof(int), "i");
            j = Expression.Parameter(typeof(int), "j");
            var temp = Expression.Parameter(typeof(int), "temp");
            vars.AddRange(new ParameterExpression[] { i, j, pivot, temp });
            init(vars, exps);

            LabelTarget endMain = Expression.Label(typeof(void), "endMain");
            LabelTarget endSub1 = Expression.Label(typeof(void), "endSub1");
            LabelTarget endSub2 = Expression.Label(typeof(void), "endSub2");
            LabelTarget continue1 = Expression.Label(typeof(void), "continue1");
            LabelTarget continue2 = Expression.Label(typeof(void), "continue2");

            exps.Add(Expression.Assign(i, low));
            exps.Add(Expression.Assign(j, high));
            exps.Add(Expression.Assign(pivot, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), Expression.Divide(Expression.Add(i, j), Expression.Constant(2)))));

            exps.Add(Expression.Loop(
                Expression.Block(
                            Expression.IfThen(Expression.GreaterThan(i, j), Expression.Break(endMain)),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i),
                                        pivot,
                                        Expression.Break(endSub1),
                                        Expression.Block(Expression.PostIncrementAssign(i), Expression.Continue(continue1))
                                        ).ToArray()
                                    ),
                                endSub1,
                                continue1
                                ),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        pivot,
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j),
                                        Expression.Break(endSub2),
                                        Expression.Block(Expression.PostDecrementAssign(j), Expression.Continue(continue2))
                                        ).ToArray()
                                    ),
                                endSub2,
                                continue2
                                ),

                            Expression.IfThen(Expression.LessThanOrEqual(i, j), Expression.Block(
                                Expression.Assign(temp, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i), Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j), temp),
                                Expression.PostIncrementAssign(i),
                                Expression.PostDecrementAssign(j)
                                ))
                 ),
                endMain
                ));

            var invokeMethod = typeof(Action<SortData, int, int, int>).GetMethod("Invoke");
            var inRangeMethod = (typeof(SortData)).GetMethod("InRange");

            exps.Add(Expression.IfThen(Expression.AndAlso(Expression.LessThan(low, j), Expression.Call(sortData, inRangeMethod, low, j)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, low, j, depthLimit)));
            exps.Add(Expression.IfThen(Expression.AndAlso(Expression.LessThan(i, high), Expression.Call(sortData, inRangeMethod, i, high)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, i, high, depthLimit)));

            BlockExpression block = Expression.Block(
                vars.ToArray(), exps
            );

            if (UseFastExpressionCompiler)
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).CompileFast(ifFastFailedReturnNull, flags);
                return re;
            }
            else
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).Compile();
                return re;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Action<SortData, int, int, int> CompileHeap(bool UseFastExpressionCompiler = false, bool ifFastFailedReturnNull = false, CompilerFlags flags = CompilerFlags.Default)
        {
            List<ParameterExpression> vars = new List<ParameterExpression>();
            List<Expression> exps = new List<Expression>();

            low = Expression.Parameter(typeof(int), "low");
            high = Expression.Parameter(typeof(int), "high");
            sortData = Expression.Parameter(typeof(SortData), "sortData");
            depthLimit = Expression.Parameter(typeof(int), "depthLimit");
            pivot = Expression.Parameter(typeof(int), "pivot");
            i = Expression.Parameter(typeof(int), "i");
            j = Expression.Parameter(typeof(int), "j");
            comparer = Expression.Parameter(typeof(Func<SortData, int, int, int>), "comparer");
            var temp = Expression.Parameter(typeof(int), "temp");
            vars.AddRange(new ParameterExpression[] { i, j, pivot, temp, comparer });
            init(vars, exps);

            LabelTarget endMain = Expression.Label(typeof(void), "endMain");
            LabelTarget endSub1 = Expression.Label(typeof(void), "endSub1");
            LabelTarget endSub2 = Expression.Label(typeof(void), "endSub2");
            LabelTarget continue1 = Expression.Label(typeof(void), "continue1");
            LabelTarget continue2 = Expression.Label(typeof(void), "continue2");

            exps.Add(Expression.Assign(i, low));
            exps.Add(Expression.Assign(j, high));
            exps.Add(Expression.Assign(pivot, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), Expression.Divide(Expression.Add(i, j), Expression.Constant(2)))));
            exps.Add(Expression.Assign(comparer, Expression.Field(sortData, "Comparer")));
            MethodInfo span = typeof(SortCompiler).GetMethod("Span");
            MethodInfo heap = typeof(SortData).GetMethod("HeapSort");

            var loopExpression = Expression.Loop(
                Expression.Block(
                            Expression.IfThen(Expression.GreaterThan(i, j), Expression.Break(endMain)),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i),
                                        pivot,
                                        Expression.Break(endSub1),
                                        Expression.Block(Expression.PostIncrementAssign(i), Expression.Continue(continue1))
                                        ).ToArray()
                                    ),
                                endSub1,
                                continue1
                                ),

                            Expression.Loop(
                                Expression.Block(
                                    makeCondition(
                                        pivot,
                                        Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j),
                                        Expression.Break(endSub2),
                                        Expression.Block(Expression.PostDecrementAssign(j), Expression.Continue(continue2))
                                        ).ToArray()
                                    ),
                                endSub2,
                                continue2
                                ),

                            Expression.IfThen(Expression.LessThanOrEqual(i, j), Expression.Block(
                                Expression.Assign(temp, Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), i), Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j)),
                                Expression.Assign(Expression.ArrayAccess(Expression.Field(sortData, "RowNumbers"), j), temp),
                                Expression.PostIncrementAssign(i),
                                Expression.PostDecrementAssign(j)
                                ))
                 ),
                endMain
                );
            var invokeMethod = typeof(Action<SortData, int, int, int>).GetMethod("Invoke");
            var inRangeMethod = (typeof(SortData)).GetMethod("InRange");

            var leftExpression = Expression.IfThen(Expression.AndAlso(Expression.LessThan(low, j), Expression.Call(sortData, inRangeMethod, low, j)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, low, j, depthLimit));
            var rightExpression = Expression.IfThen(Expression.AndAlso(Expression.LessThan(i, high), Expression.Call(sortData, inRangeMethod, i, high)), Expression.Call(Expression.Field(sortData, "SortFun"), invokeMethod, sortData, i, high, depthLimit));

            exps.Add(Expression.IfThenElse(Expression.LessThanOrEqual(depthLimit, Expression.Constant(0)), Expression.Block(Expression.Call(sortData, heap, sortData, Expression.Call(span, Expression.Field(sortData, "RowNumbers"), i, j), comparer)),
                Expression.Block(Expression.PostDecrementAssign(depthLimit), loopExpression, leftExpression, rightExpression)));
            BlockExpression block = Expression.Block(
                vars.ToArray(), exps
            );

            if (UseFastExpressionCompiler)
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).CompileFast(ifFastFailedReturnNull, flags);
                return re;
            }
            else
            {
                var re = Expression.Lambda<Action<SortData, int, int, int>>(block, sortData, low, high, depthLimit).Compile();
                return re;
            }
        }
        #endregion

        #region

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftRowNumber"></param>
        /// <param name="rightRowNumber"></param>
        /// <param name="breakExp"></param>
        /// <param name="continueExp"></param>
        /// <returns></returns>
        private List<Expression> makeCondition(Expression leftRowNumber, Expression rightRowNumber, Expression breakExp, Expression continueExp)
        {
            List<Expression> exps = new List<Expression>();

            exps.Add(Expression.Assign(left_ListIndex, Expression.Divide(leftRowNumber, Expression.Constant(Capacity))));
            exps.Add(Expression.Assign(left_ArrayIndex, Expression.Modulo(leftRowNumber, Expression.Constant(Capacity))));

            exps.Add(Expression.Assign(right_ListIndex, Expression.Divide(rightRowNumber, Expression.Constant(Capacity))));
            exps.Add(Expression.Assign(right_ArrayIndex, Expression.Modulo(rightRowNumber, Expression.Constant(Capacity))));

            for (int k = 0; k < Orders.Length; ++k)
            {
                var dataArrayList = Expression.PropertyOrField(Expression.PropertyOrField(columns[k], "Datas"), "DataArrayList");

                exps.Add(Expression.Assign(leftVars[k], Expression.ArrayAccess(Expression.Property(dataArrayList, "Item", left_ListIndex), left_ArrayIndex)));
                exps.Add(Expression.Assign(rightVars[k], Expression.ArrayAccess(Expression.Property(dataArrayList, "Item", right_ListIndex), right_ArrayIndex)));

                var leftNil = Expression.IfThen(Expression.NotEqual(rightVars[k], Expression.Constant(null)), Expression.IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"), Expression.Constant(k), Expression.Constant(0)), Expression.Constant(-1)), continueExp, breakExp));
                var rightNil = Expression.IfThen(Expression.NotEqual(leftVars[k], Expression.Constant(null)), Expression.IfThenElse(Expression.Equal(Expression.ArrayAccess(Expression.Field(sortData, "NilValues"), Expression.Constant(k), Expression.Constant(1)), Expression.Constant(-1)), continueExp, breakExp));

                var isAsc = Expression.ArrayAccess(Expression.Field(sortData, "isAsc"), Expression.Constant(k));
                List<Expression> allNotNil = new List<Expression>();

                if (FieldDataTypes[k] == EnumFieldDataType.STRING)
                    if (Orders[k].Order == EnumOrderMode.ASCCH || Orders[k].Order == EnumOrderMode.DESCCH)
                        allNotNil.Add(Expression.Assign(compareResult, chineseStringCompare(leftVars[k], rightVars[k])));
                    else
                        allNotNil.Add(Expression.Assign(compareResult, stringCompare(leftVars[k], rightVars[k])));
                else
                {
                    var method = DataTypes[(int)FieldDataTypes[k]].GetMethod("CompareTo", new[] { DataTypes[(int)FieldDataTypes[k]] });
                    allNotNil.Add(Expression.Assign(compareResult, Expression.Call(Expression.PropertyOrField(leftVars[k], "Value"), method, Expression.PropertyOrField(rightVars[k], "Value"))));
                }

                allNotNil.Add(Expression.IfThen(Expression.Not(isAsc), Expression.Assign(compareResult, Expression.Negate(compareResult))));
                allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult, Expression.Constant(-1)), continueExp));

                if (k < Orders.Length - 1)
                    allNotNil.Add(Expression.IfThen(Expression.Equal(compareResult, Expression.Constant(1)), breakExp));


                var exp = Expression.IfThenElse(Expression.Equal(leftVars[k], Expression.Constant(null)), leftNil, Expression.IfThenElse(Expression.Equal(rightVars[k], Expression.Constant(null)), rightNil, Expression.Block(allNotNil.ToArray())));
                exps.Add(exp);
            }
            exps.Add(breakExp);
            return exps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Func<SortData, int, int, int> CompileComparisonFunction(bool UseFastExpressionCompiler)
        {
            sortData = Expression.Parameter(typeof(SortData), "sortData");
            var leftIndex = Expression.Parameter(typeof(int), "leftIndex");
            var rightIndex = Expression.Parameter(typeof(int), "rightIndex");
            LabelTarget endMain = Expression.Label(typeof(int), "endMain");           

            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> vars = new List<ParameterExpression>();
            init(vars, exps);

            exps.AddRange(makeCondition(leftIndex, rightIndex, Expression.Return(endMain, Expression.Constant(1)), Expression.Return(endMain, Expression.Constant(-1))));
            exps.Add(Expression.Label(endMain, Expression.Constant(-1)));
            BlockExpression block = Expression.Block(
                vars.ToArray(), exps
            );
            if (UseFastExpressionCompiler)
            {
                var re = Expression.Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileFast();
                return re;
            }
            else
            {
                var re = Expression.Lambda<Func<SortData, int, int, int>>(block, sortData, leftIndex, rightIndex).CompileSys();
                return re;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Expression stringCompare(Expression left, Expression right)
        {
            MethodInfo compareMethod = typeof(OracleStringComparer).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
            return Expression.Call(Expression.Field(sortData, "StringComparer"), compareMethod, left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        private Expression chineseStringCompare(Expression left, Expression right)
        {
            MethodInfo compareMethod = typeof(OracleStringComparer).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
            return Expression.Call(Expression.Field(sortData, "ChineseStringComparer"), compareMethod, left, right);
        }

        #endregion

        #region

        public static Span<int> Span(int[] rownumbers, int low, int high)
        {
            return rownumbers.AsSpan(low, high - low + 1);
        }

        #endregion
    }
}
