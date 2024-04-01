using MathStatLab2.Views;
using OxyPlot.Series;
using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MathStatLab2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication

    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            /*containerRegistry.Register<Services.ICustomerStore, Services.DbCustomerStore>();*/
            // register other needed services here
        }
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }

    }

}
