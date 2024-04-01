using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class BinomialDistributionGenerator : IRandomNumberGenerator
    {
        private readonly int trials;
        private readonly double probabilityOfSuccess;

        public BinomialDistributionGenerator(int trials, double probabilityOfSuccess)
        {
            if (trials <= 0)
                throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be greater than 0");
            if (probabilityOfSuccess < 0 || probabilityOfSuccess > 1)
                throw new ArgumentOutOfRangeException(nameof(probabilityOfSuccess), "Probability must be between 0 and 1");

            this.trials = trials;
            this.probabilityOfSuccess = probabilityOfSuccess;
        }

        public List<double> GenerateRandomNumbers(int size)
        {
            var rng = new Random();
            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                int success = 0;
                for (int j = 0; j < trials; j++)
                {
                    if (rng.NextDouble() < probabilityOfSuccess)
                        success++;
                }
                numbers.Add(success);
            }

            return numbers;
        }
        public double Pdf(double k)
        {
            return Choose(trials, (int)k) * Math.Pow(probabilityOfSuccess, k) * Math.Pow(1 - probabilityOfSuccess, trials - k);
        }

        public double Cdf(double k)
        {
            double cdf = 0.0;
            for (int i = 0; i <= k; i++)
            {
                cdf += Pdf(i);
            }
            return cdf;
        }

        private static double Choose(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        private static double Factorial(int n)
        {
            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        public double Quantile(double p)
        {
            double k = 0;
            double cdf = Pdf(0);
            while (cdf < p && k <= trials)
            {
                k++;
                cdf += Pdf(k);
            }
            return k;
        }

    }
}
