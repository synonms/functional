using System;
using System.Threading.Tasks;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class MaybeAsyncExtensionsTests
    {
        [Fact]
        public async Task BiBindAsync_GivenPreconditionFuncs_WithNoFailures_ThenCallsOperation()
        {
            Func<Task<Maybe<Fault>>> preconditionFunc1 = () => Maybe<Fault>.NoneAsync;
            Func<Task<Maybe<Fault>>> preconditionFunc2 = () => Maybe<Fault>.NoneAsync;
            Func<Task<Maybe<Fault>>> preconditionFunc3 = () => Maybe<Fault>.NoneAsync;

            FuncWrapper<Task<Maybe<Fault>>> someOperationAsync = new (() => Maybe<Fault>.NoneAsync);

            Maybe<Fault> result = await preconditionFunc1.Invoke()
                .BiBindAsync(preconditionFunc2)
                .BiBindAsync(preconditionFunc3)
                .BiBindAsync(() => someOperationAsync.Invoke());

            Assert.True(result.IsNone);
            Assert.Equal(1, someOperationAsync.InvocationCount);            
        }
        
        [Fact]
        public async Task BiBindAsync_GivenPreconditionFuncs_WithFailure_ThenDoesNotCallOperation()
        {
            Func<Task<Maybe<Fault>>> preconditionFunc1 = () => Maybe<Fault>.NoneAsync;
            Func<Task<Maybe<Fault>>> preconditionFunc2 = () => Maybe<Fault>.SomeAsync(new TestFault(1));
            Func<Task<Maybe<Fault>>> preconditionFunc3 = () => Maybe<Fault>.NoneAsync;

            FuncWrapper<Task<Maybe<Fault>>> someOperationAsync = new (() => Maybe<Fault>.NoneAsync);

            Maybe<Fault> result = await preconditionFunc1.Invoke()
                .BiBindAsync(preconditionFunc2)
                .BiBindAsync(preconditionFunc3)
                .BiBindAsync(() => someOperationAsync.Invoke());

            Assert.True(result.IsSome);
            Assert.Equal(0, someOperationAsync.InvocationCount);            
        }
        
        [Fact]
        public async Task BindAsync_GivenTaskWithSome_AndTaskWithSome_ThenCallsProjection_AndReturnsSome()
        {
            FuncWrapper<Task<Maybe<string>>> someStringOperationAsync = new (() => Maybe<string>.SomeAsync("test"));
            FuncWrapper<Task<Maybe<int>>> someIntOperationAsync = new (() => Maybe<int>.SomeAsync(1));

            Maybe<int> result = await someStringOperationAsync.Invoke()
                .BindAsync(someString => someIntOperationAsync.Invoke());

            Assert.True(result.IsSome);
            Assert.Equal(1, someStringOperationAsync.InvocationCount);
            Assert.Equal(1, someIntOperationAsync.InvocationCount);
        }

        [Fact]
        public async Task BindAsync_GivenTaskWithSome_AndSome_ThenCallsProjection_AndReturnsSome()
        {
            FuncWrapper<Task<Maybe<string>>> someStringOperationAsync = new (() => Maybe<string>.SomeAsync("test"));
            FuncWrapper<Maybe<int>> someIntOperation = new (() => Maybe<int>.Some(1));

            Maybe<int> result = await someStringOperationAsync.Invoke()
                .BindAsync(someString => someIntOperation.Invoke());

            Assert.True(result.IsSome);
            Assert.Equal(1, someStringOperationAsync.InvocationCount);
            Assert.Equal(1, someIntOperation.InvocationCount);
        }

        [Fact]
        public async Task BindAsync_GivenTaskWithNone_AndTaskWithSome_ThenDoesNotCallProjection_AndReturnsNone()
        {
            FuncWrapper<Task<Maybe<int>>> someIntOperationAsync = new (() => Maybe<int>.SomeAsync(1));

            Task<Maybe<string>> maybeTask = Maybe<string>.NoneAsync;
            
            Maybe<int> result = await maybeTask
                .BindAsync(someString => someIntOperationAsync.Invoke());

            Assert.True(result.IsNone);
            Assert.Equal(0, someIntOperationAsync.InvocationCount);
        }

        [Fact]
        public async Task BindAsync_GivenTaskWithNone_AndSome_ThenDoesNotCallProjection_AndReturnsNone()
        {
            FuncWrapper<Maybe<int>> someIntOperation = new (() => Maybe<int>.Some(1));

            Task<Maybe<string>> maybeTask = Maybe<string>.NoneAsync;

            Maybe<int> result = await maybeTask
                .BindAsync(someString => someIntOperation.Invoke());

            Assert.True(result.IsNone);
            Assert.Equal(0, someIntOperation.InvocationCount);
        }

        [Fact]
        public async Task ToResultAsync_GivenNone_AndAsyncValueFunc_ReturnsResultWithValue()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = await maybe.ToResultAsync(() => Task.FromResult(value));
            
            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenNone_AndAsyncResultFunc_ReturnsResult()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = await maybe.ToResultAsync(() => Task.FromResult(expectedResult));

            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenNoneTask_AndAsyncValueFunc_ReturnsResultWithValue()
        {
            const int value = 1;
            Task<Maybe<Fault>> maybeTask = Maybe<Fault>.NoneAsync;

            Result<int> result = await maybeTask.ToResultAsync(() => Task.FromResult(value));
            
            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenNoneTask_AndAsyncResultFunc_ReturnsResult()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Task<Maybe<Fault>> maybeTask = Maybe<Fault>.NoneAsync;

            Result<int> result = await maybeTask.ToResultAsync(() => Task.FromResult(expectedResult));

            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenSome_AndAsyncValueFunc_ReturnsFault()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = await maybe.ToResultAsync(() => Task.FromResult(value));
            
            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenSome_AndAsyncResultFunc_ReturnsFault()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = await maybe.ToResultAsync(() => Task.FromResult(expectedResult));

            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenSomeTask_AndAsyncValueFunc_ReturnsFault()
        {
            const int value = 1;
            Task<Maybe<Fault>> maybeTask = Maybe<Fault>.SomeAsync(new TestFault());

            Result<int> result = await maybeTask.ToResultAsync(() => Task.FromResult(value));
            
            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
        
        [Fact]
        public async Task ToResultAsync_GivenSomeTask_AndAsyncResultFunc_ReturnsFault()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Task<Maybe<Fault>> maybeTask = Maybe<Fault>.SomeAsync(new TestFault());

            Result<int> result = await maybeTask.ToResultAsync(() => Task.FromResult(expectedResult));

            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
    }
}