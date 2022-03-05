using System.Collections.Generic;
using System.Linq;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit
{
    public class ResultTests
    {
        [Fact]
        public void Bind_GivenSyncFuncs_AndAllSucceed_ThenCallsAllFuncsAndReturnsValue()
        {
            FuncWrapper<Result<string>> stringOperation = new (() => "test");
            FuncWrapper<Result<int>> intOperation = new (() => 4);
            FuncWrapper<Result<IEnumerable<string>>> enumerableOperation = new (() => Enumerable.Empty<string>().ToResult());
            
            Result<IEnumerable<string>> syncResult = stringOperation.Invoke()
                .Bind(someString => intOperation.Invoke())
                .Bind(someInt => enumerableOperation.Invoke());

            syncResult.Match(
                results =>
                {
                    Assert.Equal(1, stringOperation.InvocationCount);
                    Assert.Equal(1, intOperation.InvocationCount);
                    Assert.Equal(1, enumerableOperation.InvocationCount);
                },
                failure => Assert.True(false, "Expected Success path"));
        }
        
        [Fact]
        public void Bind_GivenSyncFuncs_AndOneFails_ThenAllSubsequentFuncsAreNotCalled_AndReturnsError()
        {
            FuncWrapper<Result<string>> failedStringOperation = new (() => new TestFault(1));
            FuncWrapper<Result<int>> intOperation = new (() => 4);
            FuncWrapper<Result<IEnumerable<string>>> enumerableOperation = new (() => Enumerable.Empty<string>().ToResult());
            
            Result<IEnumerable<string>> syncResult = failedStringOperation.Invoke()
                .Bind(someString => intOperation.Invoke())
                .Bind(someInt => enumerableOperation.Invoke());

            syncResult.Match(
                results => Assert.True(false, "Expected Failure path"),
                failure =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(failure);
                    Assert.Equal(1, testFault.Counter);
                    Assert.Equal(1, failedStringOperation.InvocationCount);
                    Assert.Equal(0, intOperation.InvocationCount);
                    Assert.Equal(0, enumerableOperation.InvocationCount);
                });
        }

        [Fact]
        public void Bind_GivenMaybeFunc_AndResultFails_ThenReturnsFaultFromResult()
        {
            FuncWrapper<Result<string>> failedStringOperation = new (() => new TestFault(1));

            Maybe<Fault> result = failedStringOperation.Invoke().Bind(_ => new TestFault(2));

            result.Match(
                failure =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(failure);
                    Assert.Equal(1, testFault.Counter);
                }, 
                () => Assert.True(false, "Expected Some path"));
        }

        [Fact]
        public void Bind_GivenMaybeFunc_AndResultSucceeds_ThenReturnsFaultFromDelegate()
        {
            FuncWrapper<Result<string>> stringOperation = new (() => "success");

            Maybe<Fault> result = stringOperation.Invoke().Bind(_ => new TestFault(1));

            result.Match(
                failure =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(failure);
                    Assert.Equal(1, testFault.Counter);
                }, 
                () => Assert.True(false, "Expected Some path"));
        }

        [Fact]
        public void Bind_GivenMaybeFunc_AndResultSucceeds_AndDelegateSucceeds_ThenReturnsNone()
        {
            FuncWrapper<Result<string>> stringOperation = new (() => "success");

            Maybe<Fault> result = stringOperation.Invoke().Bind(_ => Maybe<Fault>.None);

            result.Match(
                fault => Assert.True(false, "Expected None path"), 
                () => Assert.True(true));
        }
    }
}