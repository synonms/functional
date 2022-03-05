using System.Collections.Generic;
using Synonms.Functional.Extensions;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class OneOfExtensionsTests
    {
        [Fact]
        public void Lefts_GivenNoLefts_ThenReturnsEmptyCollection()
        {
            List<OneOf<string, int>> oneOfs = new()
            {
                new OneOf<string, int>(1),
                new OneOf<string, int>(2),
                new OneOf<string, int>(3)
            };
            
            IEnumerable<string> enumerable = oneOfs.Lefts();
            
            Assert.Empty(enumerable);
        }
        
        [Fact]
        public void Lefts_GivenLefts_ThenReturnsCollection_WithValues()
        {
            const string expectedValue1 = "A";
            const string expectedValue2 = "B";
            
            List<OneOf<string, int>> oneOfs = new()
            {
                new OneOf<string, int>(expectedValue1),
                new OneOf<string, int>(2),
                new OneOf<string, int>(expectedValue2)
            };
            
            IEnumerable<string> enumerable = oneOfs.Lefts();

            Assert.Collection(enumerable,
                x => Assert.Equal(expectedValue1, x),
                x => Assert.Equal(expectedValue2, x));
        }
        
        [Fact]
        public void Rights_GivenNoRights_ThenReturnsEmptyCollection()
        {
            List<OneOf<string, int>> oneOfs = new()
            {
                new OneOf<string, int>("test1"),
                new OneOf<string, int>("test2"),
                new OneOf<string, int>("test3")
            };
            
            IEnumerable<int> enumerable = oneOfs.Rights();
            
            Assert.Empty(enumerable);
        }
        
        [Fact]
        public void Rights_GivenRights_ThenReturnsCollection_WithValues()
        {
            const int expectedValue1 = 1;
            const int expectedValue2 = 2;
            
            List<OneOf<string, int>> oneOfs = new()
            {
                new OneOf<string, int>(expectedValue1),
                new OneOf<string, int>("test"),
                new OneOf<string, int>(expectedValue2)
            };
            
            IEnumerable<int> enumerable = oneOfs.Rights();
            
            Assert.Collection(enumerable,
                x => Assert.Equal(expectedValue1, x),
                x => Assert.Equal(expectedValue2, x));
        }
    }
}