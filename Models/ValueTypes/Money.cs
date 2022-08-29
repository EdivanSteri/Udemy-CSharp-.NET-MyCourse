using MyCourse.Models.Enums;

namespace MyCourse.Models.ValueTypes
{
    public record Money
    {

        private decimal amount;

        public decimal Amount{
            get{
                return amount;
            }
            init{
                if(value < 0){
                    throw new InvalidOperationException("the amount cannot be negative");
                }
                amount = value;
            }
        }
        public Currency Currency{ get;init;}
        
        public Money() : this(Currency.EUR, 0.00m)
        {
        }

         public Money(Currency  currency, decimal amount){
            Amount = amount;
            Currency = currency;
        }

        public override string ToString()
        {
            return $"{Currency} {Amount:#.00}";
        }
    }
}