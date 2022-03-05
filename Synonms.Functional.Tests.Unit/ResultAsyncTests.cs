using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit
{
    public class ResultAsyncTests
    {
        [Fact]
        public async Task BindAsync_GivenAsyncFuncs_AndAllSucceed_ThenCallsAllFuncsAndReturnsValue()
        {
            FuncWrapper<Task<Result<string>>> stringOperationAsync = new (() => Result<string>.SuccessAsync("test"));
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.SuccessAsync(4));
            FuncWrapper<Task<Result<IEnumerable<string>>>> enumerableOperationAsync = new (() => Result<IEnumerable<string>>.SuccessAsync(Enumerable.Empty<string>()));
            
            Result<IEnumerable<string>> asyncResult = await stringOperationAsync.Invoke()
                .BindAsync(someString => intOperationAsync.Invoke())
                .BindAsync(someInt => enumerableOperationAsync.Invoke());

            asyncResult.Match(
                results =>
                {
                    Assert.Equal(1, stringOperationAsync.InvocationCount);
                    Assert.Equal(1, intOperationAsync.InvocationCount);
                    Assert.Equal(1, enumerableOperationAsync.InvocationCount);
                },
                failure => Assert.True(false, "Expected Success path"));
        }
        
        [Fact]
        public async Task BindAsync_GivenAsyncFuncs_AndOneFails_ThenAllSubsequentFuncsAreNotCalled_AndReturnsError()
        {
            FuncWrapper<Task<Result<string>>> failedStringOperationAsync = new (() => Result<string>.FailureAsync(new TestFault(1)));
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.SuccessAsync(4));
            FuncWrapper<Task<Result<IEnumerable<string>>>> enumerableOperationAsync = new (() => Result<IEnumerable<string>>.SuccessAsync(Enumerable.Empty<string>()));

            Result<IEnumerable<string>> asyncResult = await failedStringOperationAsync.Invoke()
                .BindAsync(someString => intOperationAsync.Invoke())
                .BindAsync(someInt => enumerableOperationAsync.Invoke());

            asyncResult.Match(
                results => Assert.True(false, "Expected Failure path"),
                failure =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(failure);
                    Assert.Equal(1, testFault.Counter);
                    Assert.Equal(1, failedStringOperationAsync.InvocationCount);
                    Assert.Equal(0, intOperationAsync.InvocationCount);
                    Assert.Equal(0, enumerableOperationAsync.InvocationCount);
                });
        }
    }
}