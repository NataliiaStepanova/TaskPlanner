using System.Windows;
using Ninject;
using TaskPlannerCodeSample.ViewModel;

namespace TaskPlannerCodeSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.AppKernel.Get<MainViewModel>();
        }
    }
}
