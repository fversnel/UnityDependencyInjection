using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamjetAnvil.Util
{
    public static class Memoization
    {
        public static Func<TArg, TResult> Memoize<TArg, TResult>(this Func<TArg, TResult> func)
        {
            var values = new ConcurrentDictionary<TArg, TResult>();
            return param => {
                TResult value;
                if (!values.TryGetValue(param, out value)) {
                    value = func(param);
                    values[param] = value;
                }
                return value;
            };
        }
    }
}
