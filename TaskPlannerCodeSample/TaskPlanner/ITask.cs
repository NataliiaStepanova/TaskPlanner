namespace TaskPlanner
{
    public interface ITask
    {
        void DoWork();

        void ProcessResult();

        TaskStates State { get; }

        string Name { get; }
    }
}
