namespace Synonms.Functional.Tests.Unit.Framework
{
    public class TestFault : Fault
    {
        public TestFault(int counter = 0) : base("code", "title", "detail", new FaultSource())
        {
            Counter = counter;
        }
        
        public int Counter { get; }
    }
}