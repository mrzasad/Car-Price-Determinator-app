#region Instructions
/*
 * You are tasked with writing an algorithm that determines the value of a used car, 
 * given several factors.
 *
 *    AGE:    Given the number of months of how old the car is, reduce its value one-half 
 *            (0.5) percent.
 *            After 10 years, it's value cannot be reduced further by age. This is not 
 *            cumulative.
 *            
 *    MILES:    For every 1,000 miles on the car, reduce its value by one-fifth of a
 *              percent (0.2). Do not consider remaining miles. After 150,000 miles, it's 
 *              value cannot be reduced further by miles.
 *            
 *    PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value 
 *                       by twenty-five (25) percent. If the car has had no previous  
 *                       owners, add ten (10) percent of the FINAL car value at the end.
 *                    
 *    COLLISION:        For every reported collision the car has been in, remove two (2) 
 *                      percent of it's value up to five (5) collisions.
 *
 *    RELIABILITY:      If the Make is Toyota, add 5%.  If the Make is Ford, subtract $500.
 *
 *
 *    PROFITABILITY:    The final calculated price cannot exceed 90% of the purchase price. 
 *    
 * 
 *    Each factor should be off of the result of the previous value in the order of
 *        1. AGE
 *        2. MILES
 *        3. PREVIOUS OWNER
 *        4. COLLISION
 *        5. RELIABILITY
 *        
 *    E.g., Start with the current value of the car, then adjust for age, take that  
 *    result then adjust for miles, then collision, previous owner, and finally reliability. 
 *    Note that if previous owner, had a positive effect, then it should be applied 
 *    AFTER step 5. If a negative effect, then BEFORE step 5.
 */
#endregion

using System;
using NUnit.Framework;

namespace CarPricer
{
    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
        public string Make { get; set; }
    }
    public delegate void Print(string msg);
    public class PriceDeterminator
    {
        public decimal DetermineCarPrice(Car car)
        {
            //TODO: Implement this method
            double newValue = DecrementByAge((double)car.PurchaseValue, car.AgeInMonths);
            newValue = DecrementByMilesTravelled(newValue, car.NumberOfMiles);
            newValue = DecrementByOwners(newValue, car.NumberOfPreviousOwners, true);
            newValue = DecrementByCollision(newValue, car.NumberOfCollisions);
            newValue = DecrementByReliability(newValue, car.Make);
            newValue = DecrementByOwners(newValue, car.NumberOfPreviousOwners, false);
            newValue = DecrementByProfitability((double)car.PurchaseValue, newValue);

            return (decimal)newValue;
        }
        private double DecrementByAge(double previousValue, double noOfMonthsUsed)
        {
            double newValue = previousValue;
            if (noOfMonthsUsed > 120)
            {
                newValue = newValue - newValue * ((0.5 / 100)* 120);
            }
            else
            {
                newValue = newValue - newValue * ((0.5 / 100) * noOfMonthsUsed);
            }
            return newValue;
        }
        private double DecrementByMilesTravelled(double previousValue, double milesTravelled)
        {
            milesTravelled = Math.Floor(milesTravelled / 1000);
            double newValue = previousValue;
            if (milesTravelled > 150)
            {
                newValue = newValue - newValue * ( 0.2 / 100)* 150;
            }
            else
            {
                newValue = newValue - newValue * (0.2 / 100) * milesTravelled;
            }
            return newValue;
        }
        private double DecrementByOwners(double previousValue, int OwnerCount, bool IsStep3)
        {
            double newValue = previousValue;
            if (OwnerCount > 2 && IsStep3)
            {
                newValue = newValue - newValue * (25 / 100);
            }
            else
            {
                if (!IsStep3 && OwnerCount == 0)
                    newValue = newValue + newValue * (10.0 / 100);
            }
            return newValue;
        }

        private double DecrementByCollision(double previousValue, int NoOfCollisions)
        {
            double newValue = previousValue;
            if (NoOfCollisions > 5)
            {
                newValue = newValue * Math.Pow((1 - 2.0 / 100), 5);
            }
            else
            {
                newValue = newValue * Math.Pow((1 - 2.0 / 100), NoOfCollisions);
            }
            return newValue;
        }
        private double DecrementByReliability(double previousValue, string make)
        {
            double newValue = previousValue;

            if (make?.ToLower() == "toyota")
            {
                newValue = newValue + newValue * 5 / 100;
            }
            else if (make?.ToLower() == "ford")
            {
                newValue = newValue - 500;
            }
            return newValue;
        }
        private double DecrementByProfitability(double previousValue, double newPrice)
        {
            if (newPrice > 0.9 * previousValue)
                newPrice = 0.9 * previousValue;
            return newPrice;
        }

    }


    [TestFixture]
    public class UnitTests
    {
        public static event Print printMsg = (e) => { };

        [Test]
        public void CalculateCarValue()
        {
            AssertCarValue(24813.40m, 35000m, 3 * 12, 50000, 1, 1, "Ford");
            AssertCarValue(20672.61m, 35000m, 3 * 12, 150000, 1, 1, "Toyota");
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1, "Tesla");
            AssertCarValue(21094.5m, 35000m, 3 * 12, 250000, 1, 0, "toyota");
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1, "Acura");
            AssertCarValue(72000m, 80000m, 8, 10000, 0, 1, null);
        }

        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions, string make)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue,
                Make = make
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            string msg = $"expected value:{expectValue} and calculated value: {carPrice}";
            Console.WriteLine(msg);
            printMsg(msg);
            Assert.AreEqual(expectValue, carPrice);
        }
    }
}