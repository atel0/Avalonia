// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Generic;
using Avalonia.Data;

namespace Avalonia.Diagnostics
{
    /// <summary>
    /// Provides diagnostic information about the value of a <see cref="AvaloniaProperty"/>
    /// on a <see cref="AvaloniaObject"/>.
    /// </summary>
    public interface IAvaloniaPropertyValue
    {
        /// <summary>
        /// Gets the property.
        /// </summary>
        AvaloniaProperty Property { get; }

        /// <summary>
        /// Gets the current property value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets the priority of the current value.
        /// </summary>
        BindingPriority ValuePriority { get; }

        /// <summary>
        /// Gets the priority levels of the current value.
        /// </summary>
        IEnumerable<IPriorityLevel> Levels { get; }
    }
}
