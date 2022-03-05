using System;

namespace Synonms.Functional.Tests.Unit.Framework
{
    public class FuncWrapper<T>
    {
        private readonly Func<T> _operation;

        public FuncWrapper(Func<T> operation)
        {
            _operation = operation;
        }
        
        public int InvocationCount { get; private set; } = 0;
        
        public T Invoke()
        {
            InvocationCount++;
            
            return _operation(); 
        }
    }
}