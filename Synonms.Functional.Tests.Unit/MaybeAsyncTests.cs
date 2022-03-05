using System.Threading.Tasks;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit
{
    public class MaybeAsyncTests
    {
        [Fact]
        public async Task BindAsync_GivenNone_WithTask_ThenDoesNotCallProjection_AndReturnsNone()
        {
            FuncWrapper<Task<Maybe<int>>> someIntOperationAsync = new (() => Maybe<int>.SomeAsync(1));

            Maybe<string> maybe = Maybe<string>.None;

            Maybe<int> result = await maybe.BindAsync(someString => someIntOperationAsync.Invoke());

            Assert.True(result.IsNone);
            Assert.Equal(0, someIntOperationAsync.InvocationCount);
        }

        [Fact]
        public async Task BindAsync_GivenSome_WithTask_ThenCallsProjection_AndReturnsSome()
        {
            const int expectedValue = 1;
            FuncWrapper<Task<Maybe<int>>> someIntOperationAsync = new (() => Maybe<int>.SomeAsync(expectedValue));

            Maybe<string> maybe = Maybe<string>.Some("test");

            Maybe<int> result = await maybe.BindAsync(someString => someIntOperationAsync.Invoke());

            result.Match(
                someInt =>
                {
                    Assert.Equal(expectedValue, someInt);
                    Assert.Equal(1, someIntOperationAsync.InvocationCount);
                },
                () => Assert.True(false, "Expected Some"));
        }

        [Fact]
        public async Task MapAsync_GivenNone_WithAsyncFunc_ThenDoesNotExecuteFunc_AndReturnsNone()
        {
            FuncWrapper<Task<int>> intOperationAsync = new (() => Task.FromResult(-1));

            Maybe<string> maybe = Maybe<string>.None;

            Maybe<int> result = await maybe.MapAsync(someString => intOperationAsync.Invoke());

            result.Match(
                someInt => Assert.True(false, "Expected None"),
                () =>
                {
                    Assert.Equal(0, intOperationAsync.InvocationCount);
                });
        }

        [Fact]
        public async Task MapAsync_GivenSome_WithAsyncFunc_ThenExecutesFunc_AndReturnsSome()
        {
            const int expectedResult = 123;
            FuncWrapper<Task<int>> intOperationAsync = new (() => Task.FromResult(expectedResult));

            Maybe<string> maybe = "test";

            Maybe<int> result = await maybe.MapAsync(someString => intOperationAsync.Invoke());
            
            result.Match(
                someInt =>
                {
                    Assert.Equal(expectedResult, someInt);
                    Assert.Equal(1, intOperationAsync.InvocationCount);
                },
                () => Assert.True(false, "Expected Some"));
        }

        [Fact]
        public async Task MatchAsync_GivenSome_WithActions_ThenExecutesSomeAction()
        {
            const int expectedResult = 123;
            Maybe<int> maybe = expectedResult;

            await maybe.MatchAsync(
                someInt => Assert.Equal(expectedResult, someInt),
                () => Assert.True(false, "Expected Some"));
        }
        
        [Fact]
        public async Task MatchAsync_GivenNone_WithActions_ThenExecutesNoneAction()
        {
            Maybe<int> maybe = Maybe<int>.None;

            await maybe.MatchAsync(
                someInt => Assert.True(false, "Expected None"),
                () => Assert.True(true));
        }
        
        [Fact]
        public async Task MatchAsync_GivenSome_WithAsyncFuncs_ThenExecutesSomeFunc()
        {
            const int expectedResult = 123;
            Maybe<int> maybe = expectedResult;

            await maybe.MatchAsync(
                someInt =>
                {
                    Assert.Equal(expectedResult, someInt);
                    return Task.CompletedTask;
                },
                () =>
                {
                    Assert.True(false, "Expected Some");
                    return Task.CompletedTask;
                });
        }
        
        [Fact]
        public async Task MatchAsync_GivenNone_WithAsyncFuncs_ThenExecutesNoneFunc()
        {
            Maybe<int> maybe = Maybe<int>.None;

            await maybe.MatchAsync(
                someInt =>
                {
                    Assert.True(false, "Expected None");
                    return Task.CompletedTask;
                },
                () =>
                {
                    Assert.True(true);
                    return Task.CompletedTask;
                });
        }

        [Fact]
        public async Task MatchAsync_GivenSome_WithAsyncFuncsOfTOut_ThenExecutesSomeFunc()
        {
            const int expectedResult = 123;
            Maybe<int> maybe = expectedResult;

            int result = await maybe.MatchAsync(
                someInt =>
                {
                    Assert.Equal(expectedResult, someInt);
                    return Task.FromResult(someInt);
                },
                () =>
                {
                    Assert.True(false, "Expected Some");
                    return Task.FromResult(0);
                });
            
            Assert.Equal(expectedResult, result);
        }
        
        [Fact]
        public async Task MatchAsync_GivenNone_WithAsyncFuncsOfTOut_ThenExecutesNoneFunc()
        {
            const int expectedResult = 0;
            Maybe<int> maybe = Maybe<int>.None;

            int result = await maybe.MatchAsync(
                someInt =>
                {
                    Assert.True(false, "Expected None");
                    return Task.FromResult(someInt);
                },
                () =>
                {
                    Assert.True(true);
                    return Task.FromResult(0);
                });
            
            Assert.Equal(expectedResult, result);
        }
    }
}