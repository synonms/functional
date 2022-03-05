using System;
using System.Collections.Generic;
using System.Linq;
using Synonms.Functional.Extensions;
using Synonms.Functional.Tests.Unit.Framework;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class ResultExtensionsTests
    {
        [Fact]
        public void Successes_GivenAllSuccesses_ThenReturnsAll()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Success(1),
                Result<int>.Success(2),
                Result<int>.Success(3),
                Result<int>.Success(4)
            };

            IEnumerable<int> successResults = enumerableResults.Successes();
            
            Assert.Collection(successResults,
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x),
                x => Assert.Equal(4, x)
            );
        }

        [Fact]
        public void Successes_GivenSomeSuccesses_ThenReturnsOnlySuccesses()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Success(1),
                Result<int>.Failure(new TestFault(2)),
                Result<int>.Success(3),
                Result<int>.Failure(new TestFault(4))
            };

            IEnumerable<int> successResults = enumerableResults.Successes();
            
            Assert.Collection(successResults,
                x => Assert.Equal(1, x),
                x => Assert.Equal(3, x)
            );
        }

        [Fact]
        public void Successes_GivenNoSuccesses_ThenReturnsEmpty()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Failure(new TestFault(1)),
                Result<int>.Failure(new TestFault(2)),
                Result<int>.Failure(new TestFault(3)),
                Result<int>.Failure(new TestFault(4))
            };

            IEnumerable<int> successResults = enumerableResults.Successes();
            
            Assert.Empty(successResults);
        }

        [Fact]
        public void Failures_GivenAllFailures_ThenReturnsAll()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Failure(new TestFault(1)),
                Result<int>.Failure(new TestFault(2)),
                Result<int>.Failure(new TestFault(3)),
                Result<int>.Failure(new TestFault(4))
            };

            IEnumerable<Fault> failureResults = enumerableResults.Failures();
            
            Assert.Collection(failureResults,
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(1, testFault.Counter);
                },
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(2, testFault.Counter);
                },
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(3, testFault.Counter);
                },
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(4, testFault.Counter);
                }
            );
        }

        [Fact]
        public void Failures_GivenSomeFailures_ThenReturnsOnlyFailures()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Success(1),
                Result<int>.Failure(new TestFault(2)),
                Result<int>.Success(3),
                Result<int>.Failure(new TestFault(4))
            };

            IEnumerable<Fault> failureResults = enumerableResults.Failures();
            
            Assert.Collection(failureResults,
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(2, testFault.Counter);
                },
                fault =>
                {
                    TestFault testFault = Assert.IsType<TestFault>(fault); 
                    Assert.Equal(4, testFault.Counter);
                }
            );
        }

        [Fact]
        public void Failures_GivenNoFailures_ThenReturnsEmpty()
        {
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Success(1),
                Result<int>.Success(2),
                Result<int>.Success(3),
                Result<int>.Success(4)
            };

            IEnumerable<Fault> failureResults = enumerableResults.Failures();
            
            Assert.Empty(failureResults);
        }
        
        [Fact]
        public void Flatten_GivenAllSuccesses_ThenFlattens()
        {
            List<Result<IEnumerable<int>>> enumerableResults = new ()
            {
                Result<IEnumerable<int>>.Success(new[] {1}),
                Result<IEnumerable<int>>.Success(new[] {2, 3}),
                Result<IEnumerable<int>>.Success(Array.Empty<int>()),
                Result<IEnumerable<int>>.Success(new[] {4, 5, 6})
            };

            Result<IEnumerable<int>> flattenedResult = enumerableResults.Flatten();

            flattenedResult.Match(
                ints => Assert.Collection(ints,
                    x => Assert.Equal(1, x),
                    x => Assert.Equal(2, x),
                    x => Assert.Equal(3, x),
                    x => Assert.Equal(4, x),
                    x => Assert.Equal(5, x),
                    x => Assert.Equal(6, x)),
                fault => Assert.True(false, "Expected Some"));
        }

        [Fact]
        public void Reduce_GivenT_WithAllSuccesses_ThenExecutesProjectionAndReturnsResult()
        {
            const int expectedResult = 1 + 2 + 3 + 4;
            
            List<Result<int>> enumerableResults = new ()
            {
                Result<int>.Success(1),
                Result<int>.Success(2),
                Result<int>.Success(3),
                Result<int>.Success(4)
            };

            Result<int> reducedResult = enumerableResults.Reduce(ints => ints.Sum(x => x));

            reducedResult.Match(
                value => Assert.Equal(expectedResult, value),
                fault => Assert.True(false, "Expected Some"));
        }

        [Fact]
        public void Reduce_GivenEnumerableT_WithAllSuccesses_ThenExecutesProjectionAndReturnsResult()
        {
            const int expectedResult = 1 + 2 + 3 + 4 + 5 + 6;
            
            List<Result<IEnumerable<int>>> enumerableResults = new ()
            {
                Result<IEnumerable<int>>.Success(new[] {1}),
                Result<IEnumerable<int>>.Success(new[] {2, 3}),
                Result<IEnumerable<int>>.Success(Array.Empty<int>()),
                Result<IEnumerable<int>>.Success(new[] {4, 5, 6})
            };

            Result<int> reducedResult = enumerableResults.Reduce(ints => ints.Sum(x => x));

            reducedResult.Match(
                value => Assert.Equal(expectedResult, value),
                fault => Assert.True(false, "Expected Some"));
        }
    }
}