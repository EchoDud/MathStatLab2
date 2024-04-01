using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class PoissonDistributionGenerator : IRandomNumberGenerator
    {
        private readonly double lambda; // Среднее количество событий за интервал

        public PoissonDistributionGenerator(double lambda)
        {
            if (lambda <= 0)
                throw new ArgumentOutOfRangeException(nameof(lambda), "Lambda must be greater than 0");

            this.lambda = lambda;
        }

        public List<double> GenerateRandomNumbers(int size)
        {
            var rng = new Random();
            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                double L = Math.Exp(-lambda);
                int k = 0;
                double p = 1;
                do
                {
                    k++;
                    double u = rng.NextDouble();
                    p *= u;
                } while (p > L);

                numbers.Add(k - 1);
            }

            return numbers;
        }
        public double Pdf(double k)
        {
            return (Math.Pow(lambda, k) * Math.Exp(-lambda)) / Factorial((int)k);
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
            while (cdf < p)
            {
                k++;
                cdf += Pdf(k);
            }
            return k;
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

    }

}
