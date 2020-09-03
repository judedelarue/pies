using System;

namespace Services
{
    /// <summary>
    /// Simple adapter that allows unit tests to mock DateTime.Now service without affecting normal operation 
    /// </summary>
    public interface INowAdapter
    {
        DateTime Now();
    }

    public class NowAdapter : INowAdapter
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}