using System.Reflection;
using System.Windows;
using Ninject;
using TaskPlannerCodeSample.DI;

namespace TaskPlannerCodeSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IKernel AppKernel;
        protected override void OnStartup(StartupEventArgs e)
        {
            AppKernel = new StandardKernel();
            AppKernel.Load(Assembly.GetExecutingAssembly());

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Bindings.DestroyTaskPanel();
            AppKernel.Dispose();
            base.OnExit(e);
        }
    }
}
