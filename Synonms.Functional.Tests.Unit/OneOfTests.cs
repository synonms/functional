using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Synonms.Functional.Tests.Unit
{
    public class OneOfTests
    {
        private enum Side
        {
            Left,
            Right
        }

        [Fact]
        public void Ctor_GivenLeftValue_ThenIsLeft_AndNotRight()
        {
            OneOf<string, int> oneOf = new("test");

            Assert.True(oneOf.IsLeft);
            Assert.False(oneOf.IsRight);
        }

        [Fact]
        public void Ctor_GivenRightValue_ThenIsRight_AndNotLeft()
        {
            OneOf<string, int> oneOf = new(1);

            Assert.True(oneOf.IsRight);
            Assert.False(oneOf.IsLeft);
        }

        [Fact]
        public void ExplicitCast_GivenToLeftValue_AndIsLeft_ThenCasts()
        {
            const string expectedValue = "test";
            OneOf<string, int> oneOf = new(expectedValue);

            string actualValue = (string)oneOf;
            
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void ExplicitCast_GivenToLeftValue_AndIsRight_ThenThrows()
        {
            OneOf<string, int> oneOf = new(1);

            Assert.Throws<InvalidCastException>(() =>
            {
                string result = (string)oneOf;
            });
        }

        [Fact]
        public void ExplicitCast_GivenToRightValue_AndIsRight_ThenCasts()
        {
            const int expectedValue = 1;
            OneOf<string, int> oneOf = new(expectedValue);

            int actualValue = (int)oneOf;
            
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void ExplicitCast_GivenToRightValue_AndIsLeft_ThenThrows()
        {
            OneOf<string, int> oneOf = new("test");

            Assert.Throws<InvalidCastException>(() =>
            {
                int result = (int)oneOf;
            });
        }

        [Fact]
        public void Match_GivenLeftValue_AndActions_ThenExecutesLeftAction()
        {
            OneOf<string, int> oneOf = "test";

            oneOf.Match(
                valueAsString => Assert.True(true),
                valueAsInt => Assert.True(false, "Expected Left action"));
        }

        [Fact]
        public void Match_GivenRightValue_AndActions_ThenExecutesRightAction()
        {
            OneOf<string, int> oneOf = 123;

            oneOf.Match(
                valueAsString => Assert.True(false, "Expected Right action"),
                valueAsInt => Assert.True(true));
        }
        
        [Fact]
        public void Match_GivenLeftValue_AndFunctions_ThenExecutesLeftFunc_AndReturnsValue()
        {
            OneOf<string, int> oneOf = "test";

            Side result = oneOf.Match(
                valueAsString => Side.Left,
                valueAsInt => Side.Right);

            Assert.Equal(Side.Left, result);
        }

        [Fact]
        public void Match_GivenRightValue_AndFunctions_ThenExecutesRightFunc_AndReturnsValue()
        {
            OneOf<string, int> oneOf = 123;

            Side result = oneOf.Match(
                valueAsString => Side.Left,
                valueAsInt => Side.Right);

            Assert.Equal(Side.Right, result);
        }
        
        [Fact]
        public async Task Match_GivenLeftValue_AndFunctionsReturningTasks_ThenExecutesLeftFunc_AndReturnsTaskForValue()
        {
            OneOf<string, int> oneOf = "test";

            Side result = await oneOf.MatchAsync(
                valueAsString => Task.FromResult(Side.Left),
                valueAsInt => Task.FromResult(Side.Right));

            Assert.Equal(Side.Left, result);
        }

        [Fact]
        public async Task Match_GivenRightValue_AndFunctionsReturningTasks_ThenExecutesRightFunc_AndReturnsTaskForValue()
        {
            OneOf<string, int> oneOf = 123;

            Side result = await oneOf.MatchAsync(
                valueAsString => Task.FromResult(Side.Left),
                valueAsInt => Task.FromResult(Side.Right));

            Assert.Equal(Side.Right, result);
        }
        
        [Fact]
        public void LeftAsEnumerable_GivenIsLeft_ThenReturnsCollectionWithValue()
        {
            const string value = "test";
            OneOf<string, int> oneOf = new (value);

            IEnumerable<string> enumerable = oneOf.LeftAsEnumerable();
            
            Assert.Single(enumerable);
            Assert.Equal(value, enumerable.ElementAt(0));
        }
        
        [Fact]
        public void LeftAsEnumerable_GivenIsLeft_ThenReturnsEmptyCollection()
        {
            const int value = 1;
            OneOf<string, int> oneOf = new (value);

            IEnumerable<string> enumerable = oneOf.LeftAsEnumerable();
            
            Assert.Empty(enumerable);
        }
        
        [Fact]
        public void RightAsEnumerable_GivenIsRight_ThenReturnsCollectionWithValue()
        {
            const int value = 1;
            OneOf<string, int> oneOf = new (value);

            IEnumerable<int> enumerable = oneOf.RightAsEnumerable();
            
            Assert.Single(enumerable);
            Assert.Equal(value, enumerable.ElementAt(0));
        }
        
        [Fact]
        public void RightAsEnumerable_GivenIsLeft_ThenReturnsEmptyCollection()
        {
            const string value = "test";
            OneOf<string, int> oneOf = new (value);

            IEnumerable<int> enumerable = oneOf.RightAsEnumerable();
            
            Assert.Empty(enumerable);
        }
    }
}