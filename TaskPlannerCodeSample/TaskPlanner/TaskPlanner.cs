using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TaskPlanner
{
    public class TaskPlanner : ITaskPlanner
    {
        private readonly List<ITask> _tasksCollection = new List<ITask>(); 
        private readonly object _tasksCollectionSync = new object();
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
        private readonly Thread _worker;
        private volatile bool _keepDoingWork = true;

        public TaskPlanner()
        {
            _worker = new Thread(DoWork);
            _worker.Start();
        }

        public void AddTask(ITask task)
        {
            lock (_tasksCollectionSync)
            {
                _tasksCollection.Add(task);
                if (_tasksCollection.Count == 1)
                {
                    _resetEvent.Set();
                }
            }
        }

        public string GetTasksQueueInfo()
        {
            int tasksCount;
            var infoString = new StringBuilder();

            lock (_tasksCollectionSync)
            {
                tasksCount = _tasksCollection.Count;
                foreach (var task in _tasksCollection)
                {
                    infoString.AppendFormat("{0} - {1}\n", task.Name, task.State);
                }
            }

            infoString.Insert(0, string.Format("Count of tasks: {0}\n", tasksCount));

            return infoString.ToString();
        }

        private void DoWork()
        {
            _resetEvent.Wait();

            while (_keepDoingWork)
            {
                var currentTask = _tasksCollection[0];
                currentTask.DoWork();
                currentTask.ProcessResult();

                RemoveFirstTask();

                _resetEvent.Wait();
            }
        }

        private void RemoveFirstTask()
        {
            lock (_tasksCollectionSync)
            {
                _tasksCollection.RemoveAt(0);
                if (_tasksCollection.Count == 0)
                {
                    _resetEvent.Reset();
                }
            }
        }

        public void Dispose()
        {
            _keepDoingWork = false;
            if (!_resetEvent.IsSet)
            {
                // to unlock working thread
                _resetEvent.Set();
            }
            _worker.Join();
            _resetEvent.Dispose();
        }
    }
}
