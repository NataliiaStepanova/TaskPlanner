using System;

namespace TaskPlanner
{
    public interface ITaskPlanner : IDisposable
    {
        void AddTask(ITask task);

        string GetTasksQueueInfo();
    }
}
