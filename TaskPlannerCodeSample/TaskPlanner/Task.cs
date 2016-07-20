using System;

namespace TaskPlanner
{
    public class Task<TArg, TResult> : ITask
    {
        private readonly Func<TArg, TResult> _work;
        private readonly Action<ITask, TResult> _resultProcessing;
        private readonly Action<ITask, Exception> _failureProcessing;
        private readonly TArg _arg;
        private TResult _result;

        public Task(Func<TArg, TResult> work, TArg arg, Action<ITask, TResult> resultProcessing = null, Action<ITask, Exception> failureProcessing = null)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work", "Task must do some work");
            }
            _work = work;
            _resultProcessing = resultProcessing;
            _failureProcessing = failureProcessing;
            _arg = arg;

            State = TaskStates.New;
        } 

        public TaskStates State
        {
            get; 
            private set;
        }

        public string Name { get; set; }

        public void DoWork()
        {
            State = TaskStates.InWork;
            try
            {
                _result = _work(_arg);
                State = TaskStates.WorkFinished;
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        public void ProcessResult()
        {
            if (State == TaskStates.WorkFinished)
            {
                State = TaskStates.InResultProcessing;
                try
                {
                    if (_resultProcessing != null)
                    {
                        _resultProcessing(this, _result);
                    }
                    State = TaskStates.ResultProcessingFinished;
                }
                catch (Exception ex)
                {
                    Fail(ex);
                }
            }
        }

        private void Fail(Exception ex)
        {
            State = TaskStates.Failed;
            if (_failureProcessing != null)
            {
                _failureProcessing(this, ex);
            }
        }
    }
}
