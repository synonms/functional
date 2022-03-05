using System;
using System.Threading.Tasks;

namespace Synonms.Functional
{
    public partial class Result<TSuccess>
    {
        public Task<Result<TSuccess>> AsAsync() =>
            Task.FromResult(this);

        public Task<Result<TOut>> BindAsync<TOut>(Func<TSuccess, Task<Result<TOut>>> successFunc) =>
            MatchAsync(
                successFunc,
                Synonms.Functional.Result<TOut>.FailureAsync);

        public Task<Maybe<Fault>> BindAsync(Func<TSuccess, Task<Maybe<Fault>>> successFunc) =>
            MatchAsync(
                successFunc,
                Maybe<Fault>.SomeAsync);
        
        public static Task<Result<TSuccess>> FailureAsync(Fault fault) => 
            Task.FromResult(Failure(fault));
        
        public static Task<Result<TSuccess>> SuccessAsync(TSuccess success) =>
            Task.FromResult(Success(success));
    }
}