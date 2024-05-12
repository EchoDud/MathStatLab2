using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class NormalDistributionGenerator : IRandomNumberGenerator
    {
        private readonly double mean;
        private readonly double stddev;

        public NormalDistributionGenerator(double mean, double stddev)
        {
            this.mean = mean;
            this.stddev = stddev;
        }

        public List<double> GenerateRandomNumbers(int size)
        {
            var rng = new Random();
            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                double u1 = 1.0 - rng.NextDouble();
                double u2 = 1.0 - rng.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                double randNormal = mean + stddev * randStdNormal;
                numbers.Add(randNormal);
            }

            return numbers;
        }
        public double Pdf(double x)
        {
            double z = (x - mean) / stddev;
            return Math.Exp(-0.5 * z * z) / (stddev * Math.Sqrt(2 * Math.PI));
        }

        public double Cdf(double x)
        {
            double z = (x - mean) / stddev;
            if (z <= 0)
            {
                return 0.852 * Math.Exp(-Math.Pow((-z + 1.5774) / 2.0637, 2.34));
            }
            return 1 - 0.852 * Math.Exp(-Math.Pow((z + 1.5774) / 2.0637, 2.34));
        }

        public double Quantile(double alpha)
        {
            if (alpha < 0.5)
            {
                return mean - Math.Sqrt(-2 * Math.Log(1 - alpha)) * stddev;
            }
            return mean + Math.Sqrt(-2 * Math.Log(alpha)) * stddev;
        }
    }
}
