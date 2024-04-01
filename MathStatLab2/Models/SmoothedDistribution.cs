using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public class SmoothedDistribution : IDistribution
    {
        private readonly List<double> sample;
        private readonly double h; // Шаг сглаживания

        public SmoothedDistribution(List<double> sample, double h)
        {
            this.sample = sample;
            this.h = h;
        }

        private static double KernelFunction(double x)
        {
            if (Math.Abs(x) <= 1)
                return 0.75 * (1 - x * x);
            return 0;
        }

        private static double IntegratedKernelFunction(double x)
        {
            if (x < -1)
                return 0;
            else if (-1 <= x && x < 1)
                return 0.5 + 0.75 * (x - Math.Pow(x, 3) / 3);
            else
                return 1;
        }

        public double Pdf(double x)
        {
            return sample.Average(y => KernelFunction((x - y) / h)) / h;
        }

        public double Cdf(double x)
        {
            return sample.Average(y => IntegratedKernelFunction((x - y) / h));
        }

        // Quantile не реализован из-за сложности вычисления обратной функции
        public double Quantile(double alpha)
        {
            throw new NotImplementedException();
        }
    }
}
