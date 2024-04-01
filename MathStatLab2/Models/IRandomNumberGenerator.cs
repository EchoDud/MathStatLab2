using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathStatLab2.Models
{
    public interface IRandomNumberGenerator
    {
        List<double> GenerateRandomNumbers(int size);
        double Pdf(double x);
        double Cdf(double x);
        double Quantile(double alpha);

    }
}
