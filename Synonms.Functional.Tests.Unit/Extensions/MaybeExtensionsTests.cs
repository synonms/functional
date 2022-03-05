using System.Collections.Generic;
using System.Linq;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class MaybeExtensionsTests
    {
        [Fact]
        public void Collect_GivenNoFailures_ThenExecutesAllFuncs_AndCollectsResults()
        {
            FuncWrapper<Maybe<Fault>> successOperation = new (() => Maybe<Fault>.None);

            List<Maybe<Fault>> results = successOperation.Invoke()
                .Collect(successOperation.Invoke)
                .Collect(successOperation.Invoke)
                .ToList();

            Assert.Equal(3, successOperation.InvocationCount);

            Assert.Equal(3, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsNone);
            Assert.True(results.ElementAt(2).IsNone);
        }

        [Fact]
        public void Collect_GivenInitialSuccess_AndSubsequentFailures_ThenExecutesAllFuncs_AndCollectsResults()
        {
            FuncWrapper<Maybe<Fault>> successOperation = new (() => Maybe<Fault>.None);
            FuncWrapper<Maybe<Fault>> failureOperation = new (() => new TestFault());

            List<Maybe<Fault>> results = successOperation.Invoke()
                .Collect(failureOperation.Invoke)
                .Collect(successOperation.Invoke)
                .Collect(failureOperation.Invoke)
                .Collect(successOperation.Invoke)
                .ToList();

            Assert.Equal(2, failureOperation.InvocationCount);
            Assert.Equal(3, successOperation.InvocationCount);

            Assert.Equal(5, results.Count);
            Assert.True(results.ElementAt(0).IsNone);
            Assert.True(results.ElementAt(1).IsSome);
            Assert.True(results.ElementAt(2).IsNone);
            Assert.True(results.ElementAt(3).IsSome);
            Assert.True(results.ElementAt(4).IsNone);
        }
        
        [Fact]
        public void Collect_GivenInitialFailure_AndNoSubsequentFailures_ThenExecutesAllFuncs_AndCollectsResults()
        {
            FuncWrapper<Maybe<Fault>> successOperation = new (() => Maybe<Fault>.None);
            FuncWrapper<Maybe<Fault>> failureOperation = new (() => new TestFault());

            List<Maybe<Fault>> results = failureOperation.Invoke()
                .Collect(successOperation.Invoke)
                .Collect(successOperation.Invoke)
                .ToList();

            Assert.Equal(1, failureOperation.InvocationCount);
            Assert.Equal(2, successOperation.InvocationCount);

            Assert.Equal(3, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsNone);
            Assert.True(results.ElementAt(2).IsNone);
        }

        [Fact]
        public void Collect_GivenInitialFailure_AndSubsequentFailures_ThenExecutesAllFuncs_AndCollectsResults()
        {
            FuncWrapper<Maybe<Fault>> successOperation = new (() => Maybe<Fault>.None);
            FuncWrapper<Maybe<Fault>> failureOperation = new (() => new TestFault());

            List<Maybe<Fault>> results = failureOperation.Invoke()
                .Collect(successOperation.Invoke)
                .Collect(failureOperation.Invoke)
                .Collect(successOperation.Invoke)
                .Collect(failureOperation.Invoke)
                .ToList();

            Assert.Equal(3, failureOperation.InvocationCount);
            Assert.Equal(2, successOperation.InvocationCount);

            Assert.Equal(5, results.Count);
            Assert.True(results.ElementAt(0).IsSome);
            Assert.True(results.ElementAt(1).IsNone);
            Assert.True(results.ElementAt(2).IsSome);
            Assert.True(results.ElementAt(3).IsNone);
            Assert.True(results.ElementAt(4).IsSome);
        }

        [Fact] 
        public void Reduce_GivenNoFailures_ThenReturnsNone()
        {
            IEnumerable<Maybe<Fault>> maybes = new List<Maybe<Fault>>
            {
                Maybe<Fault>.None,
                Maybe<Fault>.None,
                Maybe<Fault>.None
            };

            Maybe<Fault> result = maybes.Reduce<Fault, Fault>(failures => new TestFault(1));

            Assert.True(result.IsNone);
        }
        
        [Fact] 
        public void Reduce_GivenFailures_ThenPassesErrorsToProjectionFunc_AndReturnsSome()
        {
            const int expectedCounter = 99;
            IEnumerable<Maybe<Fault>> maybes = new List<Maybe<Fault>>
            {
                new TestFault(1),
                Maybe<Fault>.None,
                new TestFault(2)
            };

            Maybe<Fault> result = maybes.Reduce<Fault, Fault>(failures => new TestFault(expectedCounter));

            result.Match(
                someFailure =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(someFailure);
            
                    Assert.Equal(expectedCounter, testFault.Counter);
                },
                () => Assert.True(false, "Expected Some"));
        }
        
        [Fact]
        public void ToResult_GivenNone_AndValue_ReturnsResultWithValue()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = maybe.ToResult(value);

            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }

        [Fact]
        public void ToResult_GivenNone_AndResult_ReturnsResult()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = maybe.ToResult(expectedResult);

            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }

        [Fact]
        public void ToResult_GivenNone_AndValueFunc_ReturnsResultWithValue()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = maybe.ToResult(() => value);
            
            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }
        
        [Fact]
        public void ToResult_GivenNone_AndResultFunc_ReturnsResult()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.None;

            Result<int> result = maybe.ToResult(() => expectedResult);

            result.Match(
                success => Assert.Equal(value, success), 
                fault => Assert.True(false, "Expected success"));
        }

        [Fact]
        public void ToResult_GivenSome_AndValue_ReturnsFault()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = maybe.ToResult(value);

            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }

        [Fact]
        public void ToResult_GivenSome_AndResult_ReturnsFault()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = maybe.ToResult(expectedResult);

            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }

        [Fact]
        public void ToResult_GivenSome_AndValueFunc_ReturnsFault()
        {
            const int value = 1;
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = maybe.ToResult(() => value);
            
            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
        
        [Fact]
        public void ToResult_GivenSome_AndResultFunc_ReturnsFault()
        {
            const int value = 1;
            Result<int> expectedResult = Result<int>.Success(value);
            Maybe<Fault> maybe = Maybe<Fault>.Some(new TestFault());

            Result<int> result = maybe.ToResult(() => expectedResult);

            result.Match(
                success => Assert.True(false, "Expected fault"), 
                fault => Assert.IsType<TestFault>(fault));
        }
    }
}