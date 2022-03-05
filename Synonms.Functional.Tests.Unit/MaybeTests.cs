using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit
{
    public class MaybeTests
    {
        [Fact]
        public void Ctor_GivenValue_ThenIsSome_AndNotNone()
        {            
            Maybe<string> maybe = new("test");

            Assert.True(maybe.IsSome);
            Assert.False(maybe.IsNone);
        }

        [Fact]
        public void Ctor_GivenDefault_ThenIsNone_AndNotSome()
        {
            Maybe<string> maybe = new(default!);

            Assert.True(maybe.IsNone);
            Assert.False(maybe.IsSome);
        }

        [Fact]
        public void Ctor_GivenNull_ThenIsNone_AndNotSome()
        {
            Maybe<string> maybe = new(null!);

            Assert.True(maybe.IsNone);
            Assert.False(maybe.IsSome);
        }
        
        [Fact]
        public void Some_GivenValue_ThenIsSome_AndNotNone()
        {
            Maybe<string> maybe = Maybe<string>.Some("test");

            Assert.True(maybe.IsSome);
            Assert.False(maybe.IsNone);
        }

        [Fact]
        public void Some_GivenDefault_ThenIsNone_AndNotSome()
        {
            Maybe<string> maybe = Maybe<string>.Some(default!);

            Assert.True(maybe.IsNone);
            Assert.False(maybe.IsSome);
        }

        [Fact]
        public void Some_GivenNull_ThenIsNone_AndNotSome()
        {
            Maybe<string> maybe = Maybe<string>.Some(null!);

            Assert.True(maybe.IsNone);
            Assert.False(maybe.IsSome);
        }

        [Fact]
        public void None_GivenDefault_ThenIsNone_AndNotSome()
        {
            Maybe<string> maybe = Maybe<string>.None;

            Assert.True(maybe.IsNone);
            Assert.False(maybe.IsSome);
        }

        [Fact]
        public void ExplicitCast_GivenIsSome_ThenCasts()
        {
            string expectedValue = "test";
            Maybe<string> maybe = new(expectedValue);

            string actualValue = (string)maybe;
            
            Assert.Equal(expectedValue, actualValue);
        }
        
        [Fact]
        public void ExplicitCast_GivenIsNone_ThenThrows()
        {
            Maybe<string> maybe = Maybe<string>.None;

            Assert.Throws<InvalidCastException>(() =>
            {
                string result = (string)maybe;
            });
        }
        
        [Fact]
        public void EqualsOperator_GivenSameValue_ThenReturnsTrue()
        {
            int value = 1;
            Maybe<int> maybeA = Maybe<int>.Some(value);
            Maybe<int> maybeB = Maybe<int>.Some(value);

            Assert.True(maybeA == maybeB);
            Assert.True(maybeB == maybeA);
        }

        [Fact]
        public void EqualsOperator_GivenDifferentValue_ThenReturnsFalse()
        {
            Maybe<int> maybeA = Maybe<int>.Some(1);
            Maybe<int> maybeB = Maybe<int>.Some(2);

            Assert.False(maybeA == maybeB);
            Assert.False(maybeB == maybeA);
        }

        [Fact]
        public void NotEqualsOperator_GivenSameValue_ThenReturnsFalse()
        {
            int value = 1;
            Maybe<int> maybeA = Maybe<int>.Some(value);
            Maybe<int> maybeB = Maybe<int>.Some(value);

            Assert.False(maybeA != maybeB);
            Assert.False(maybeB != maybeA);
        }

        [Fact]
        public void NotEqualsOperator_GivenDifferentValue_ThenReturnsTrue()
        {
            Maybe<int> maybeA = Maybe<int>.Some(1);
            Maybe<int> maybeB = Maybe<int>.Some(2);

            Assert.True(maybeA != maybeB);
            Assert.True(maybeB != maybeA);
        }

        [Fact]
        public void AsEnumerable_GivenSome_ThenReturnsCollectionWithValue()
        {
            int value = 1;
            Maybe<int> maybe = Maybe<int>.Some(value);

            IEnumerable<int> enumerable = maybe.AsEnumerable();

            Assert.Single(enumerable);
            Assert.Equal(value, enumerable.ElementAt(0));
        }
        
        [Fact]
        public void AsEnumerable_GivenNone_ThenReturnsEmptyCollection()
        {
            Maybe<int> maybe = Maybe<int>.None;

            IEnumerable<int> enumerable = maybe.AsEnumerable();
            
            Assert.Empty(enumerable);
        }
        
        [Fact]
        public void Bind_GivenNone_ThenDoesNotCallProjection_AndReturnsNone()
        {
            FuncWrapper<Maybe<int>> someIntOperation = new (() => 1);

            Maybe<string> maybe = Maybe<string>.None;

            Maybe<int> result = maybe.Bind(someString => someIntOperation.Invoke());

            Assert.True(result.IsNone);
            Assert.Equal(0, someIntOperation.InvocationCount);
        }

        [Fact]
        public void Bind_GivenSome_ThenCallsProjection_AndReturnsSome()
        {
            const int expectedValue = 1;
            FuncWrapper<Maybe<int>> someIntOperation = new (() => expectedValue);

            Maybe<string> maybe = Maybe<string>.Some("test");

            Maybe<int> result = maybe.Bind(someString => someIntOperation.Invoke());

            result.Match(
                someInt =>
                {
                    Assert.Equal(expectedValue, someInt);
                    Assert.Equal(1, someIntOperation.InvocationCount);
                },
                () => Assert.True(false, "Expected Some"));
        }

        [Fact]
        public void Coalesce_GivenNone_WithConcreteFallback_ThenReturnsFallback()
        {
            const int fallback = 1;
            
            Maybe<int> maybe = Maybe<int>.None;

            int result = maybe.Coalesce(fallback);
            
            Assert.Equal(fallback, result);
        }

        [Fact]
        public void Coalesce_GivenNone_WithFuncFallback_ThenCallsFuncAndReturnsFallback()
        {
            const int fallback = 1;
            
            Maybe<int> maybe = Maybe<int>.None;

            int result = maybe.Coalesce(() => fallback);
            
            Assert.Equal(fallback, result);
        }

        [Fact]
        public void Coalesce_GivenSome_WithConcreteFallback_ThenReturnsSome()
        {
            const int some = 99;
            const int fallback = 1;
            
            Maybe<int> maybe = Maybe<int>.Some(some);

            int result = maybe.Coalesce(fallback);
            
            Assert.Equal(some, result);
        }

        [Fact]
        public void Coalesce_GivenSome_WithFuncFallback_ThenReturnsSome()
        {
            const int some = 99;
            const int fallback = 1;
            
            Maybe<int> maybe = Maybe<int>.Some(some);

            int result = maybe.Coalesce(() => fallback);
            
            Assert.Equal(some, result);
        }

        [Fact]
        public void Collect_GivenNoneAndFunctionReturningNone_ThenCallsFunction_AndReturnsCorrectEnumerable()
        {
            FuncWrapper<Maybe<int>> noneIntOperation = new (() => Maybe<int>.None);

            Maybe<int> maybe = Maybe<int>.None;

            List<Maybe<int>> results = maybe.Collect(() => noneIntOperation.Invoke()).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsNone);
            
            Assert.Equal(1, noneIntOperation.InvocationCount);
        }

        [Fact]
        public void Collect_GivenNoneAndFunctionReturningSome_ThenCallsFunction_AndReturnsCorrectEnumerable()
        {
            FuncWrapper<Maybe<int>> someIntOperation = new (() => 1);

            Maybe<int> maybe = Maybe<int>.None;

            List<Maybe<int>> results = maybe.Collect(() => someIntOperation.Invoke()).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsSome);
            
            Assert.Equal(1, someIntOperation.InvocationCount);
        }

        [Fact]
        public void Collect_GivenSomeAndFunctionReturningNone_ThenCallsFunction_AndReturnsCorrectEnumerable()
        {
            FuncWrapper<Maybe<int>> noneIntOperation = new (() => Maybe<int>.None);

            Maybe<int> maybe = 99;

            List<Maybe<int>> results = maybe.Collect(() => noneIntOperation.Invoke()).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsNone);
            
            Assert.Equal(1, noneIntOperation.InvocationCount);
        }
        
        [Fact]
        public void Collect_GivenSomeAndFunctionReturningSome_ThenCallsFunction_AndReturnsCorrectEnumerable()
        {
            FuncWrapper<Maybe<int>> someIntOperation = new (() => 1);

            Maybe<int> maybe = 99;

            List<Maybe<int>> results = maybe.Collect(() => someIntOperation.Invoke()).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsSome);
            
            Assert.Equal(1, someIntOperation.InvocationCount);
        }

        [Fact]
        public void Collect_GivenNoneAndNone_ThenReturnsCorrectEnumerable()
        {
            Maybe<Fault> maybe = Maybe<Fault>.None;

            List<Maybe<Fault>> results = maybe.Collect(Maybe<Fault>.None).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsNone);
        }

        [Fact]
        public void Collect_GivenNoneAndSome_ThenReturnsCorrectEnumerable()
        {
            Maybe<Fault> maybe = Maybe<Fault>.None;

            List<Maybe<Fault>> results = maybe.Collect(Maybe<Fault>.Some(new TestFault(1))).ToList();

            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsSome);
        }

        [Fact]
        public void Collect_GivenSomeAndNone_ThenReturnsCorrectEnumerable()
        {
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault(0));

            List<Maybe<Fault>> results = maybe.Collect(Maybe<Fault>.None).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsNone);
        }
        
        [Fact]
        public void Collect_GivenSomeAndSome_ThenReturnsCorrectEnumerable()
        {
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault(0));

            List<Maybe<Fault>> results = maybe.Collect(Maybe<Fault>.Some(new TestFault(1))).ToList();
            
            Assert.Equal(2, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsSome);
        }

        [Fact]
        public void Filter_GivenUnmatchedPredicate_ThenReturnsNone()
        {
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault(0));

            Maybe<Fault> result = maybe.Filter(error => false);
            
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Filter_GivenMatchedPredicate_ThenReturnsNone()
        {
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault(0));

            Maybe<Fault> result = maybe.Filter(error => true);
            
            Assert.True(result.IsSome);
        }

        [Fact]
        public void IfNone_GivenNone_WithAction_ThenExecutesAction()
        {
            bool isExecuted = false;
            
            Maybe<string> maybe = Maybe<string>.None;

            maybe.IfNone(() => isExecuted = true);
            
            Assert.True(isExecuted);
        }
        
        [Fact]
        public void IfNone_GivenSome_WithAction_ThenDoesNotExecuteAction()
        {
            bool isExecuted = false;
            
            Maybe<string> maybe = Maybe<string>.Some("test");

            maybe.IfNone(() => isExecuted = true);
            
            Assert.False(isExecuted);
        }

        [Fact]
        public void IfSome_GivenSome_WithAction_ThenExecutesAction()
        {
            bool isExecuted = false;
            
            Maybe<string> maybe = Maybe<string>.Some("test");

            maybe.IfSome(someString => isExecuted = true);
            
            Assert.True(isExecuted);
        }
        
        [Fact]
        public void IfSome_GivenNone_WithAction_ThenDoesNotExecuteAction()
        {
            bool isExecuted = false;
            
            Maybe<string> maybe = Maybe<string>.None;

            maybe.IfSome(someString => isExecuted = true);
            
            Assert.False(isExecuted);
        }

        [Fact]
        public async Task Match_GivenNone_WithTaskOutput_ThenExecutesNonePath_AndReturnsTaskForValue()
        {
            const int expectedResult = 123;
            FuncWrapper<Task<int>> intOperation = new (() => Task.FromResult(expectedResult));

            Maybe<string> maybe = Maybe<string>.None;

            int result = await maybe.Match(
                someString =>
                {
                    Assert.True(false, "Expected None");
                    return Task.FromResult(-1);
                },
                () => intOperation.Invoke());
            
            Assert.Equal(expectedResult, result);
            Assert.Equal(1, intOperation.InvocationCount);
        }

        [Fact]
        public async Task Match_GivenSome_WithTaskOutput_ThenExecutesSomePath_AndReturnsTaskForValue()
        {
            const int expectedResult = 123;
            FuncWrapper<Task<int>> intOperation = new (() => Task.FromResult(-1));

            Maybe<string> maybe = "test";

            int result = await maybe.Match(
                someString => Task.FromResult(expectedResult),
                () => intOperation.Invoke());
            
            Assert.Equal(expectedResult, result);
            Assert.Equal(0, intOperation.InvocationCount);
        }

        [Fact]
        public void Match_GivenNone_ThenExecutesNonePath_AndReturnsValue()
        {
            const int expectedResult = 123;
            FuncWrapper<int> intOperation = new (() => expectedResult);

            Maybe<string> maybe = Maybe<string>.None;

            int result = maybe.Match(
                someString =>
                {
                    Assert.True(false, "Expected None");
                    return -1;
                },
                () => intOperation.Invoke());
            
            Assert.Equal(expectedResult, result);
            Assert.Equal(1, intOperation.InvocationCount);
        }

        [Fact]
        public void Match_GivenSome_ThenExecutesSomePath_AndReturnsValue()
        {
            const int expectedResult = 123;
            FuncWrapper<int> intOperation = new (() => expectedResult);

            Maybe<string> maybe = "test";

            int result = maybe.Match(
                someString => expectedResult,
                () => intOperation.Invoke());
            
            Assert.Equal(expectedResult, result);
            Assert.Equal(0, intOperation.InvocationCount);
        }
        
        [Fact]
        public void Map_GivenNone_ThenDoesNotExecuteFunc_AndReturnsNone()
        {
            FuncWrapper<int> intOperation = new (() => 123);

            Maybe<string> maybe = Maybe<string>.None;

            Maybe<int> result = maybe.Map(someString => intOperation.Invoke());

            result.Match(
                someInt => Assert.True(false, "Expected None"),
                () =>
                {
                    Assert.Equal(0, intOperation.InvocationCount);
                });
        }

        [Fact]
        public void Map_GivenSome_ThenExecutesFunc_AndReturnsSome()
        {
            const int expectedResult = 123;
            FuncWrapper<int> intOperation = new (() => expectedResult);

            Maybe<string> maybe = "test";

            Maybe<int> result = maybe.Map(someString => intOperation.Invoke());
            
            result.Match(
                someInt =>
                {
                    Assert.Equal(expectedResult, someInt);
                    Assert.Equal(1, intOperation.InvocationCount);
                },
                () => Assert.True(false, "Expected Some"));
        }
    }
}