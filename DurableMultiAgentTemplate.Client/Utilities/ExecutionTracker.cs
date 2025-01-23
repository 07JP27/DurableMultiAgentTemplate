namespace DurableMultiAgentTemplate.Client.Utilities;

public class ExecutionTracker
{
    public bool IsInProgress { get; private set; }

    public Scope Start()
    {
        if (IsInProgress)
        {
            throw new InvalidOperationException("Execution is already in progress.");
        }

        IsInProgress = true;
        return new Scope(this);
    }

    public struct Scope(ExecutionTracker executionTracker) : IDisposable
    {
        public void Dispose() => executionTracker.IsInProgress = false;
    }
}
