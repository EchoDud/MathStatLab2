using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class ExponentialDistributionGenerator : IRandomNumberGenerator
    {
        private readonly double lambda;

        public ExponentialDistributionGenerator(double lambda)
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
                double sample = rng.NextDouble();
                double result = -Math.Log(1 - sample) / lambda;
                numbers.Add(result);
            }

            return numbers;
        }
        public double Pdf(double x)
        {
            return x < 0 ? 0 : lambda * Math.Exp(-lambda * x);
        }

        public double Cdf(double x)
        {
            return x < 0 ? 0 : 1 - Math.Exp(-lambda * x);
        }

        public double Quantile(double p)
        {
            return -Math.Log(1 - p) / lambda;
        }
    }
}
