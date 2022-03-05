using System;
using System.Collections.Generic;
using System.Linq;

namespace Synonms.Functional.Extensions
{
    public static partial class ResultExtensions
    {
        public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> results) =>
            results.Lefts();
        
        public static IEnumerable<Fault> Failures<T>(this IEnumerable<Result<T>> results) =>
            results.Rights();

        public static Result<IEnumerable<T>> Flatten<T>(this IEnumerable<Result<IEnumerable<T>>> results)
        {
            List<Fault> failures = results.Failures().ToList();
            return failures.Any()
                ? new AggregateFault(failures)
                : Result<IEnumerable<T>>.Success(results.Successes().SelectMany(x => x));
        }

        public static Result<TOut> Reduce<T, TOut>(this IEnumerable<Result<T>> results, Func<IEnumerable<T>, TOut> projectionFunc)
        {
            List<Fault> failures = results.Failures().ToList();
            return failures.Any()
                ? new AggregateFault(failures)
                : projectionFunc.Invoke(results.Successes());
        }

        public static Result<TOut> Reduce<T, TOut>(this IEnumerable<Result<IEnumerable<T>>> results, Func<IEnumerable<T>, TOut> projectionFunc)
        {
            List<Fault> failures = results.Failures().ToList();
            return failures.Any()
                ? new AggregateFault(failures)
                : projectionFunc.Invoke(results.Successes().SelectMany(x => x));
        }
    }
}