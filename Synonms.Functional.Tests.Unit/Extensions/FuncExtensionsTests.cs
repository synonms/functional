using System;
using Synonms.Functional.Extensions;
using Xunit;

namespace Synonms.Functional.Tests.Unit.Extensions
{
    public class FuncExtensionsTests
    {
        [Fact]
        public void Compose_GivenSingleFunc_ThenChainsFuncs()
        {
            const string expectedInput = "input";
            const string expectedOutput = "output";
            
            bool isFunc1Executed = false;
            bool isFunc2Executed = false;
            
            Func<string, int> stringToIntFunc = s =>
            {
                isFunc1Executed = true;
                return 1;
            };
            Func<int, string> intToStringFunc = i =>
            {
                isFunc2Executed = true;
                return expectedOutput;
            };

            Func<string, string> composedFunc = stringToIntFunc.Compose(intToStringFunc);

            string actualOutput = composedFunc(expectedInput); 
            
            Assert.Equal(expectedOutput, actualOutput);
            Assert.True(isFunc1Executed);
            Assert.True(isFunc2Executed);
        }
        
        [Fact]
        public void Compose_GivenMultipleFuncs_ThenChainsAllFuncs()
        {
            const string expectedInput = "input";
            const string expectedOutput = "output";
            
            bool isFunc1Executed = false;
            bool isFunc2Executed = false;
            bool isFunc3Executed = false;
            bool isFunc4Executed = false;

            Func<string, int> stringToIntFunc = s =>
            {
                isFunc1Executed = true;
                return 1;
            };
            Func<int, double> intToDoubleFunc = i =>
            {
                isFunc2Executed = true;
                return 99.0;
            };
            Func<double, char> doubleToCharFunc = i =>
            {
                isFunc3Executed = true;
                return 'a';
            };
            Func<char, string> charToStringFunc = i =>
            {
                isFunc4Executed = true;
                return expectedOutput;
            };

            Func<string, string> composedFunc = stringToIntFunc
                .Compose(intToDoubleFunc)
                .Compose(doubleToCharFunc)
                .Compose(charToStringFunc);

            string actualOutput = composedFunc(expectedInput); 
            
            Assert.Equal(expectedOutput, actualOutput);
            Assert.True(isFunc1Executed);
            Assert.True(isFunc2Executed);
            Assert.True(isFunc3Executed);
            Assert.True(isFunc4Executed);
        }
    }
}