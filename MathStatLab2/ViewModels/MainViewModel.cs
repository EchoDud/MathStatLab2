using OxyPlot.Series;
using OxyPlot;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using MathStatLab2.Models;

namespace MathStatLab2.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _selectedDistribution;
        private int _sampleSize;
        private double _smoothingParameter = 0.5;
        private double _parameter1;
        private double _parameter2;
        private bool _isParameter1Visible;
        private bool _isParameter2Visible;
        private string _parameter1Name;
        private string _parameter2Name;
        private PlotModel _distributionPlotModel;
        private PlotModel _densityPlotModel;
        private bool _isAutoCalculateH;

        public string SelectedDistribution
        {
            get { return _selectedDistribution; }
            set
            {
                SetProperty(ref _selectedDistribution, value);
                UpdateParameterVisibility();
            }
        }

        public int SampleSize
        {
            get { return _sampleSize; }
            set { SetProperty(ref _sampleSize, value); }
        }

        public double SmoothingParameter
        {
            get { return _smoothingParameter; }
            set { SetProperty(ref _smoothingParameter, value); }
        }

        public double Parameter1
        {
            get { return _parameter1; }
            set { SetProperty(ref _parameter1, value); }
        }

        public double Parameter2
        {
            get { return _parameter2; }
            set { SetProperty(ref _parameter2, value); }
        }

        public bool IsParameter1Visible
        {
            get { return _isParameter1Visible; }
            set { SetProperty(ref _isParameter1Visible, value); }
        }

        public bool IsParameter2Visible
        {
            get { return _isParameter2Visible; }
            set { SetProperty(ref _isParameter2Visible, value); }
        }

        public string Parameter1Name
        {
            get { return _parameter1Name; }
            set { SetProperty(ref _parameter1Name, value); }
        }

        public string Parameter2Name
        {
            get { return _parameter2Name; }
            set { SetProperty(ref _parameter2Name, value); }
        }

        public PlotModel DistributionPlotModel
        {
            get { return _distributionPlotModel; }
            set { SetProperty(ref _distributionPlotModel, value); }
        }

        public PlotModel DensityPlotModel
        {
            get { return _densityPlotModel; }
            set { SetProperty(ref _densityPlotModel, value); }
        }

        public bool IsAutoCalculateH
        {
            get { return _isAutoCalculateH; }
            set { SetProperty(ref _isAutoCalculateH, value); }
        }

        public List<string> AvailableDistributions { get; }

        public DelegateCommand GenerateCommand { get; }

        public MainViewModel()
        {
            AvailableDistributions = new List<string>
            {
                "Normal",
                "Exponential",
                "Uniform",
                "Binomial",
                "Poisson"
            };

            GenerateCommand = new DelegateCommand(Generate);
            DistributionPlotModel = new PlotModel { Title = "Distribution" };
            DensityPlotModel = new PlotModel { Title = "Density" };
        }

        private void Generate()
        {
            IRandomNumberGenerator generator = null;

            switch (SelectedDistribution)
            {
                case "Normal":
                    generator = new NormalDistributionGenerator(Parameter1, Parameter2);
                    break;
                case "Exponential":
                    generator = new ExponentialDistributionGenerator(Parameter1);
                    break;
                case "Uniform":
                    generator = new UniformDistributionGenerator(Parameter1, Parameter2);
                    break;
                case "Binomial":
                    generator = new BinomialDistributionGenerator((int)Parameter1, Parameter2);
                    break;
                case "Poisson":
                    generator = new PoissonDistributionGenerator(Parameter1);
                    break;
                default:
                    throw new NotImplementedException($"The distribution '{SelectedDistribution}' is not implemented yet.");
            }

            if (generator != null)
            {
                var numbers = generator.GenerateRandomNumbers(SampleSize);
                if (IsAutoCalculateH)
                {
                    SmoothingParameter = CalculateOptimalH(numbers);
                }
                UpdatePlots(numbers, generator, SmoothingParameter);
            }
        }

        private double CalculateOptimalH(List<double> arr, double eps = 0.01)
        {
            double K(double x) => Math.Abs(x) <= 1 ? 0.75 * (1 - x * x) : 0;
            double dK(double x) => Math.Abs(x) <= 1 ? -1.5 * x : 0;

            double Iteration(double prevH)
            {
                double result = 0;

                for (int i = 0; i < arr.Count; i++)
                {
                    double numerator = 0;
                    double denominator = 0;

                    for (int j = 0; j < arr.Count; j++)
                    {
                        if (i != j)
                        {
                            double diff = (arr[i] - arr[j]) / prevH;
                            numerator += dK(diff) * (arr[i] - arr[j]);
                            denominator += K(diff);
                        }
                    }

                    if (denominator != 0)  // Check for division by zero
                    {
                        result += numerator / denominator;
                    }
                }

                return -result;
            }

            double prevH = 0.01;
            double curH = Iteration(prevH) / arr.Count;

            // Ensure curH is within a reasonable range
            double minH = 0.01;
            double maxH = 1.0;
            if (curH < minH || curH > maxH || double.IsNaN(curH) || double.IsInfinity(curH))
            {
                // Fallback to a default value or heuristic
                curH = Math.Max(minH, Math.Min(maxH, Math.Sqrt(1.0 / arr.Count)));
            }

            while (Math.Abs(curH - prevH) > eps)
            {
                prevH = curH;
                curH = Iteration(prevH) / arr.Count;

                // Ensure curH is within a reasonable range during iterations
                if (curH < minH || curH > maxH || double.IsNaN(curH) || double.IsInfinity(curH))
                {
                    curH = Math.Max(minH, Math.Min(maxH, Math.Sqrt(1.0 / arr.Count)));
                    break;
                }
            }

            return curH;
        }

        private void UpdateParameterVisibility()
        {
            switch (SelectedDistribution)
            {
                case "Normal":
                    IsParameter1Visible = true;
                    IsParameter2Visible = true;
                    Parameter1Name = "Mean";
                    Parameter2Name = "Standard Deviation";
                    break;
                case "Exponential":
                    IsParameter1Visible = true;
                    IsParameter2Visible = false;
                    Parameter1Name = "Lambda";
                    break;
                case "Uniform":
                    IsParameter1Visible = true;
                    IsParameter2Visible = true;
                    Parameter1Name = "Minimum";
                    Parameter2Name = "Maximum";
                    break;
                case "Binomial":
                    IsParameter1Visible = true;
                    IsParameter2Visible = true;
                    Parameter1Name = "Trials";
                    Parameter2Name = "Probability";
                    break;
                case "Poisson":
                    IsParameter1Visible = true;
                    IsParameter2Visible = false;
                    Parameter1Name = "Lambda";
                    break;
                default:
                    IsParameter1Visible = false;
                    IsParameter2Visible = false;
                    break;
            }
        }

        private void UpdatePlots(List<double> numbers, IRandomNumberGenerator generator, double smoothingParameter)
        {
            DensityPlotModel = InitializeDensityPlotModel(numbers, generator, smoothingParameter);
            DistributionPlotModel = InitializeDistributionPlotModel(numbers, generator, smoothingParameter);
            FinalizePlots();
        }

        private PlotModel InitializeDensityPlotModel(List<double> numbers, IRandomNumberGenerator generator, double smoothingParameter)
        {
            numbers.Sort();
            var min = numbers.Min();
            var max = numbers.Max();
            var densityModel = new PlotModel { Title = "Density" };
            AddDensityHistogram(densityModel, numbers, min, max);
            AddTrueDensityLine(densityModel, min, max, generator);
            AddSmoothedDensityLine(densityModel, numbers, min, max, smoothingParameter);

            return densityModel;
        }

        private PlotModel InitializeDistributionPlotModel(List<double> numbers, IRandomNumberGenerator generator, double smoothingParameter)
        {
            numbers.Sort();
            var min = numbers.Min();
            var max = numbers.Max();
            var distributionModel = new PlotModel { Title = "Distribution" };
            AddCumulativeDistributionLine(distributionModel, numbers);
            AddTrueDistributionLine(distributionModel, min, max, generator);
            AddSmoothedCDFLine(distributionModel, numbers, min, max, smoothingParameter);

            return distributionModel;
        }

        private void AddDensityHistogram(PlotModel model, List<double> numbers, double min, double max)
        {
            var histogramSeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Black
            };

            var sortedNumbers = numbers.OrderBy(n => n).ToList();
            double q1 = sortedNumbers[(int)(0.25 * sortedNumbers.Count)];
            double q3 = sortedNumbers[(int)(0.75 * sortedNumbers.Count)];
            double iqr = q3 - q1;

            double binSize = 2 * iqr * Math.Pow(numbers.Count, -1.0 / 3.0);
            double normalizationFactor = 1 / (numbers.Count * binSize);

            for (double x = min; x <= max; x += binSize)
            {
                var count = numbers.Count(n => n >= x && n < x + binSize);
                double height = count * normalizationFactor;
                histogramSeries.Points.Add(new DataPoint(x, 0));
                histogramSeries.Points.Add(new DataPoint(x, height));
                histogramSeries.Points.Add(new DataPoint(x + binSize, height));
                histogramSeries.Points.Add(new DataPoint(x + binSize, 0));
                histogramSeries.Points.Add(DataPoint.Undefined);
            }

            model.Series.Add(histogramSeries);
        }

        private void AddTrueDensityLine(PlotModel model, double min, double max, IRandomNumberGenerator generator)
        {
            var trueDensitySeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Red,
                Title = "True Density"
            };

            double step = (max - min) / 100;
            for (double x = min; x <= max; x += step)
            {
                trueDensitySeries.Points.Add(new DataPoint(x, generator.Pdf(x)));
            }

            model.Series.Add(trueDensitySeries);
        }

        private void AddSmoothedDensityLine(PlotModel model, List<double> numbers, double min, double max, double smoothingParameter)
        {
            var smoothedDensitySeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Green,
                Title = "Smoothed Density (Parzen-Rosenblatt)"
            };

            var smoothedDistribution = new SmoothedDistribution(numbers, smoothingParameter);
            double step = (max - min) / 100;
            for (double x = min; x <= max; x += step)
            {
                smoothedDensitySeries.Points.Add(new DataPoint(x, smoothedDistribution.Pdf(x)));
            }

            model.Series.Add(smoothedDensitySeries);
        }

        private void AddCumulativeDistributionLine(PlotModel model, List<double> numbers)
        {
            var distributionSeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.SteelBlue
            };

            for (int i = 0; i < numbers.Count; i++)
            {
                distributionSeries.Points.Add(new DataPoint(numbers[i], (double)(i + 1) / numbers.Count));
            }

            model.Series.Add(distributionSeries);
        }

        private void AddTrueDistributionLine(PlotModel model, double min, double max, IRandomNumberGenerator generator)
        {
            var trueDistributionSeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Red,
                Title = "True Distribution"
            };

            double step = (max - min) / 100;
            for (double x = min; x <= max; x += step)
            {
                trueDistributionSeries.Points.Add(new DataPoint(x, generator.Cdf(x)));
            }

            model.Series.Add(trueDistributionSeries);
        }

        private void AddSmoothedCDFLine(PlotModel model, List<double> numbers, double min, double max, double smoothingParameter)
        {
            var smoothedSeries = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Green,
                Title = "Smoothed CDF"
            };

            var smoothedDistribution = new SmoothedDistribution(numbers, smoothingParameter);
            double step = (max - min) / 100;
            for (double x = min; x <= max; x += step)
            {
                smoothedSeries.Points.Add(new DataPoint(x, smoothedDistribution.Cdf(x)));
            }

            model.Series.Add(smoothedSeries);
        }

        private void FinalizePlots()
        {
            DensityPlotModel.InvalidatePlot(true);
            DistributionPlotModel.InvalidatePlot(true);
        }
    }
}
