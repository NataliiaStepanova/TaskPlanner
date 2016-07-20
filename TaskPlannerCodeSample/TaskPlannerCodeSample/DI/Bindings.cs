using Ninject.Modules;
using TaskPlanner;

namespace TaskPlannerCodeSample.DI
{
    public class Bindings : NinjectModule
    {
        private static object _taskPanelLifeTime = new object();

        public override void Load()
        {
            Bind<ITaskPlanner>().To<TaskPlanner.TaskPlanner>().InScope(ctx => _taskPanelLifeTime);
        }

        public static void DestroyTaskPanel()
        {
            _taskPanelLifeTime = new object();
        }
    }
}
