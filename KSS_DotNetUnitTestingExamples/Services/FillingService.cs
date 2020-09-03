using System;

namespace Services
{
    public interface IFillingService
    {
        int Get(string flavour, int quantity);

        void Order(string flavour);
    }

    public class FillingService : IFillingService
    {
        private static int totalAppleFilling = 90;
        private static int totalCherryFilling = 1000;
        private static int totalCheeseFilling = 75;

        public int Get(string flavour, int quantity)
        {
            switch (flavour)
            {
                case "apple":
                    {
                        return getAppleFilling();
                    }
                case "cherry":
                    {
                        return getCherryFilling(); ;
                    }
                case "cheese":
                    {
                        return getCheeseFilling(); ;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        public void Order(string flavour)
        {
            switch (flavour)
            {
                case "apple":
                    {
                        totalAppleFilling += 10000;
                        break;
                    }
                case "cherry":
                    {
                        totalCherryFilling += 10000;
                        break;
                    }
                case "cheese":
                    {
                        totalCheeseFilling += 10000;
                        break;
                    }
            }
        }

        private int getCherryFilling()
        {
            if (totalCherryFilling < 50)
            {
                throw new ArgumentException("No cherry filling");
            }
            totalCherryFilling -= 50;
            return 50;
        }

        private int getAppleFilling()
        {
            if (totalAppleFilling < 50)
            {
                throw new ArgumentException("No apple filling");
            }
            totalAppleFilling -= 50;
            return 50;
        }

        private int getCheeseFilling()
        {
            if (totalCheeseFilling < 50)
            {
                throw new ArgumentException("No cheese filling");
            }
            totalCheeseFilling -= 50;
            return 50;
        }

        public void OrderFilling()
        {
            totalAppleFilling = 200;
            totalCherryFilling = 150;
            totalCheeseFilling = 120;
        }
    }
}