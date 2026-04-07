using System.Collections;

namespace CompileTest.TestDepency
{
    [Serializable]
    public class OracleStringComparer : IComparer<string>, IComparer
    {
        public int Compare(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
            {
                if (x == null)
                    return 0;
                else
                    return 1;
            }
            else
            {
                int len = x.Length < y.Length ? x.Length : y.Length;
                for (int k = 0; k < len; ++k)
                {
                    var re = x[k].CompareTo(y[k]);
                    if (re != 0)
                        return re < 0 ? -1 : 1;
                }
                return x.Length.CompareTo(y.Length);
            }

        }

        public int Compare(object x, object y)
        {
            return Compare((string)x, (string)y);
        }
    }
}
