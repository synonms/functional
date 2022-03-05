using System.Collections.Generic;
using Synonms.Functional.Extensions;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void Coalesce_SingleSome_ReturnsSome()
        {
            IEnumerable<uint> enumerable = new List<uint>
            {
                1,
                0,
                0
            };

            Maybe<uint> result = enumerable.Coalesce(item => item > 0 ? Maybe<uint>.Some(item) : Maybe<uint>.None,
                Maybe<uint>.Some(99));

            result.Match(
                value => Assert.Equal(1u, value), 
                () => Assert.True(false, "Expected 1"));
        }
        
        [Fact]
        public void Coalesce_MultipleSome_ReturnsFirstSome()
        {
            IEnumerable<uint> enumerable = new List<uint>
            {
                0,
                1,
                2,
                0,
                3
            };

            Maybe<uint> result = enumerable.Coalesce(item => item > 0 ? Maybe<uint>.Some(item) : Maybe<uint>.None,
                Maybe<uint>.Some(99));

            result.Match(
                value => Assert.Equal(1u, value), 
                () => Assert.True(false, "Expected 1"));
        }
        
        [Fact]
        public void Coalesce_None_ReturnsFallback()
        {
            IEnumerable<uint> enumerable = new List<uint>
            {
                0,
                0,
                0
            };

            Maybe<uint> result = enumerable.Coalesce(item => item > 0 ? Maybe<uint>.Some(item) : Maybe<uint>.None,
                Maybe<uint>.Some(99));

            result.Match(
                value => Assert.Equal(99u, value), 
                () => Assert.True(false, "Expected 99"));
        }
    }
}