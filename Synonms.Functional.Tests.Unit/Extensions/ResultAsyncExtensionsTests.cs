using System.Threading.Tasks;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class ResultAsyncExtensionsTests
    {
        [Fact]
        public async Task BindAsync_GivenSuccessTask_WithSuccessFunc_ThenInvokesProjectionFunc_AndReturnsSuccess()
        {
            const int expectedResult = 4;
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.SuccessAsync(expectedResult));
            
            Task<Result<string>> successTask = Result<string>.SuccessAsync("test");

            Result<int> result = await successTask.BindAsync(someString => intOperationAsync.Invoke());

            Assert.Equal(1, intOperationAsync.InvocationCount);
            
            result.Match(
                x => Assert.Equal(expectedResult, x), 
                fault => Assert.True(false, "Expected success"));
        }

        [Fact]
        public async Task BindAsync_GivenSuccessTask_WithFailureFunc_ThenInvokesProjectionFunc_AndReturnsFailure()
        {
            const int expectedResult = 4;
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.FailureAsync(new TestFault(expectedResult)));
            
            Task<Result<string>> successTask = Result<string>.SuccessAsync("test");

            Result<int> result = await successTask.BindAsync(someString => intOperationAsync.Invoke());

            Assert.Equal(1, intOperationAsync.InvocationCount);
            
            result.Match(
                x => Assert.True(false, "Expected failure"), 
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault);
                    Assert.Equal(expectedResult, testFault.Counter);
                });
        }

        [Fact]
        public async Task BindAsync_GivenFailureTask_WithSuccessFunc_ThenDoesNotInvokeProjectionFunc_AndReturnsFailure()
        {
            const int expectedResult = 4;
            const int unexpectedResult = 99;
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.SuccessAsync(unexpectedResult));
            
            Task<Result<string>> failureTask = Result<string>.FailureAsync(new TestFault(expectedResult));

            Result<int> result = await failureTask.BindAsync(someString => intOperationAsync.Invoke());

            Assert.Equal(0, intOperationAsync.InvocationCount);
            
            result.Match(
                x => Assert.True(false, "Expected failure"), 
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault);
                    Assert.Equal(expectedResult, testFault.Counter);
                });
        }

        [Fact]
        public async Task BindAsync_GivenFailureTask_WithFailureFunc_ThenDoesNotInvokeProjectionFunc_AndReturnsFailure()
        {
            const int expectedResult = 4;
            const int unexpectedResult = 99;
            FuncWrapper<Task<Result<int>>> intOperationAsync = new (() => Result<int>.FailureAsync(new TestFault(unexpectedResult)));
            
            Task<Result<string>> failureTask = Result<string>.FailureAsync(new TestFault(expectedResult));

            Result<int> result = await failureTask.BindAsync(someString => intOperationAsync.Invoke());

            Assert.Equal(0, intOperationAsync.InvocationCount);
            
            result.Match(
                x => Assert.True(false, "Expected failure"), 
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault);
                    Assert.Equal(expectedResult, testFault.Counter);
                });
        }
    }
}