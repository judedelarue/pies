using Serilog;
using System;

namespace Services
{
    public interface IPastryService
    {
        int Get(int quantity);
        void Order();
    }

    public class PastryService : IPastryService
    {
        private int? TotalPastry;
        private readonly int BatchSize = 500;
 

        public PastryService()
        {
            TotalPastry = BatchSize;
        }

        public int Get(int quantity)
        {
            if(TotalPastry < quantity)
            {
                throw new ArgumentException("No pastry!");
            }
            TotalPastry -= quantity;
            return quantity;
        }

        public void Order()
        {
            TotalPastry += BatchSize;
        }
    }
}