using System;
using System.Threading;
using System.Windows.Input;
using TaskPlanner;
using TaskPlannerCodeSample.ViewModel.Base;
using Ninject;

namespace TaskPlannerCodeSample.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private static int TaskCounter = 0;
        private readonly ITaskPlanner _planner;
        private readonly Random _taskTypeGen = new Random();

        [Inject]
        public MainViewModel(ITaskPlanner planner)
        {
            _planner = planner;
        }

        #region Text Info Properties

        private string _taskProcessingInfo = "";
        public string TaskProcessingInfo
        {
            get { return _taskProcessingInfo; }
            set
            {
                _taskProcessingInfo = value;
                OnPropertyChanged("TaskProcessingInfo");
            }
        }

        private string _plannerInfo = "";
        public string PlannerInfo
        {
            get { return _plannerInfo; }
            set
            {
                _plannerInfo = value;
                OnPropertyChanged("PlannerInfo");
            }
        }

        #endregion

        #region Add Task

        private ICommand _addTaskCommand = null;
        public ICommand AddTaskCommand
        {
            get
            {
                if (_addTaskCommand == null)
                {
                    _addTaskCommand = new RelayCommand(AddTask);
                }
                return _addTaskCommand;
            }
        }

        private void AddTask()
        {
            _planner.AddTask(CreateTask());
        }

        private ITask CreateTask()
        {
            TaskCounter++;
            ITask task;
            // create one of 3 tasks randomly
            var taskType = _taskTypeGen.Next(0, 3);
            switch (taskType)
            {
                case 0:
                    task = new Task<int, int>(CalculateSum, TaskCounter, ShowResult, ProcessException) { Name = string.Format("Task #{0}. Calc Sum", TaskCounter) };
                    break;
                case 1:
                    task = new Task<int, int>(CalculateFactorial, TaskCounter, ShowResult, ProcessException) { Name = string.Format("Task #{0}. Calc Factorial", TaskCounter) };
                    break;
                default:
                    task = new Task<int, int>(ThrowException, TaskCounter, ShowResult, ProcessException) { Name = string.Format("Task #{0}. Throw exception", TaskCounter) };
                    break;
            }
            TaskProcessingInfo += string.Format("{0}: {1} created.\n", DateTime.Now.ToLongTimeString(), task.Name);
            return task;
        }

        #endregion

        #region Tasks work, results & errors handling

        private int CalculateSum(int number)
        {
            if (number < 1)
            {
                return 0;
            }
            var result = 0;
            for (var i = 1; i <= number; i++)
            {
                result += i;
            }

            Thread.Sleep(1000);

            return result;
        }

        private int CalculateFactorial(int number)
        {
            if (number < 1)
            {
                return 1;
            }
            var result = 1;
            for (var i = 2; i <= number; i++)
            {
                result *= i;
            }

            Thread.Sleep(1000);

            return result;
        }

        private int ThrowException(int number)
        {
            throw new Exception("exception details");
        }

        private void ShowResult(ITask task, int result)
        {
            Thread.Sleep(500);
            var text = string.Format("{0}: {1} finished. Result = {2}\n", DateTime.Now.ToLongTimeString(), task.Name,
                result);
            TaskProcessingInfo += text;
        }

        private void ProcessException(ITask task, Exception ex)
        {
            var text = string.Format("{0}: {1} failed. Exception: {2}\n", DateTime.Now.ToLongTimeString(), task.Name, ex.Message);
            TaskProcessingInfo += text;
        }

        #endregion

        #region Update Planner Info

        private ICommand _updatePlannerInfoCommand = null;

        public ICommand UpdatePlannerInfoCommand
        {
            get
            {
                if (_updatePlannerInfoCommand == null)
                {
                    _updatePlannerInfoCommand = new RelayCommand(UpdatePlannerInfo);
                }
                return _updatePlannerInfoCommand;
            }
        }

        private void UpdatePlannerInfo()
        {
            PlannerInfo = _planner.GetTasksQueueInfo();
        }

        #endregion
    }
}
