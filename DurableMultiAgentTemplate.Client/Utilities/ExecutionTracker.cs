namespace DurableMultiAgentTemplate.Client.Utilities;

/// <summary>
/// Tracks the execution state of operations.
/// Helps prevent concurrent operations by maintaining a flag for operations in progress.
/// </summary>
public class ExecutionTracker
{
    /// <summary>
    /// Gets a value indicating whether an operation is currently in progress.
    /// </summary>
    public bool IsInProgress { get; private set; }

    /// <summary>
    /// Starts tracking an execution and returns a disposable scope.
    /// When the scope is disposed, the execution is marked as completed.
    /// </summary>
    /// <returns>A disposable scope that, when disposed, marks the execution as completed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an execution is already in progress.</exception>
    public Scope Start()
    {
        if (IsInProgress)
        {
            throw new InvalidOperationException("Execution is already in progress.");
        }

        IsInProgress = true;
        return new Scope(this);
    }

    /// <summary>
    /// Represents a scope of execution.
    /// When disposed, marks the associated execution tracker's operation as completed.
    /// </summary>
    public struct Scope(ExecutionTracker executionTracker) : IDisposable
    {
        /// <summary>
        /// Marks the execution as completed.
        /// </summary>
        public void Dispose() => executionTracker.IsInProgress = false;
    }
}
