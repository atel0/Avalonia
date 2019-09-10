using System.Collections.Generic;
using Avalonia.Data;

namespace Avalonia.Diagnostics
{
    /// <summary>
    /// Provides diagnostic information for a binding priority level on an
    /// <see cref="AvaloniaPropertyValue"/>.
    /// </summary>
    public interface IPriorityLevel
    {
        /// <summary>
        /// Gets the direct value for the priority level.
        /// </summary>
        object DirectValue { get; }

        /// <summary>
        /// Gets the priority of this level.
        /// </summary>
        BindingPriority Priority { get; }

        /// <summary>
        /// Gets the <see cref="IPriorityBindingEntry.Index"/> value of the active binding, or -1
        /// if no binding is active.
        /// </summary>
        int ActiveBindingIndex { get; }

        /// <summary>
        /// Gets the bindings for the priority level.
        /// </summary>
        IEnumerable<IPriorityBindingEntry> Bindings { get; }
    }
}
