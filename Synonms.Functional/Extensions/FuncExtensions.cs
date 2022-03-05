using System;

namespace Synonms.Functional.Extensions
{
    public static class FuncExtensions
    {
        public static Func<TIn, TOut> Compose<TIn, TIntermediate, TOut>(this Func<TIn, TIntermediate> inFunc, Func<TIntermediate, TOut> chainedFunc)
        {
            return x => chainedFunc(inFunc(x));
        }
    }
}