using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardsREST.Model
{
    public static class Extensions
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static bool IsBetween(this DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

    }
}