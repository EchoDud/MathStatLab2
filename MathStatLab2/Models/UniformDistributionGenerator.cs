using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class UniformDistributionGenerator : IRandomNumberGenerator
    {
        private readonly double min;
        private readonly double max;

        public UniformDistributionGenerator(double min, double max)
        {
            if (max <= min)
                throw new ArgumentException("Max must be greater than min");

            this.min = min;
            this.max = max;
        }

        public List<double> GenerateRandomNumbers(int size)
        {
            var rng = new Random();
            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                double sample = rng.NextDouble();
                double result = min + (sample * (max - min));
                numbers.Add(result);
            }

            return numbers;
        }
        public double Pdf(double x)
        {
            if (x >= min && x <= max)
                return 1.0 / (max - min);
            return 0.0;
        }

        public double Cdf(double x)
        {
            if (x < min) return 0.0;
            if (x > max) return 1.0;
            return (x - min) / (max - min);
        }

        public double Quantile(double p)
        {
            return min + p * (max - min);
        }
    }
}
