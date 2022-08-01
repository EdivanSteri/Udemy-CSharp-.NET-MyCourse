using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCourse.Models.Enums;

namespace MyCourse.Models.ValueTypes
{
    public class Money
    {

        private decimal amount;

        public decimal Amount{
            get{
                return amount;
            }
            set{
                if(value < 0){
                    throw new InvalidOperationException("the amount cannot be negative");
                }
                amount = value;
            }
        }
        public Currency Currency{ get;set;}
        
        public Money() : this(Currency.EUR, 0.00m)
        {
        }

         public Money(Currency  currency, decimal amount)
        {
            Amount = amount;
            Currency = currency;
        }

        public override bool Equals(object? obj){
            var money = obj as Money;
            return money != null && 
                   Amount == money.Amount &&
                   Currency == money.Currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Currency} {Amount:#.00}";
        }
    }
}