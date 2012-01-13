using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TernarySearchTree
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return (list == null) || (list.Count == 0);
        }

        public static void OnNext<TS>(this IList<IObserver<IEnumerable<TS>>> observers, IEnumerable<TS> val)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(val);
            }
        }
    }
}
