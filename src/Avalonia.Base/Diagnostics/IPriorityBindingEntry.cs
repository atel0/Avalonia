namespace Avalonia.Diagnostics
{
    /// <summary>
    /// Provides diagnostic information about a binding in an <see cref="IPriorityLevel"/>.
    /// </summary>
    public interface IPriorityBindingEntry
    {
        /// <summary>
        /// Gets a description of the binding.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the binding entry index. Later bindings will have higher indexes.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the current value of the binding.
        /// </summary>
        object Value { get; }
    }
}
