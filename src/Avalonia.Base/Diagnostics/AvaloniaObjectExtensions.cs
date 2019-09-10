// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Avalonia.Data;

namespace Avalonia.Diagnostics
{
    /// <summary>
    /// Defines diagnostic extensions on <see cref="AvaloniaObject"/>s.
    /// </summary>
    public static class AvaloniaObjectExtensions
    {
        /// <summary>
        /// Gets a diagnostic for a <see cref="AvaloniaProperty"/> on a <see cref="AvaloniaObject"/>.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// An <see cref="IAvaloniaPropertyValue"/> that can be used to diagnose the state of the
        /// property on the object, or null if the property is not set on the object.
        /// </returns>
        public static IAvaloniaPropertyValue GetDiagnostic(this AvaloniaObject o, AvaloniaProperty property)
        {
            var set = o.GetSetValues();

            if (set.TryGetValue(property, out var obj))
            {
                return obj as IAvaloniaPropertyValue ?? new LocalValuePropertyValue(property, obj);
            }

            return null;
        }

        private class LocalValuePropertyValue : IAvaloniaPropertyValue
        {
            public LocalValuePropertyValue(
                AvaloniaProperty property,
                object value)
            {
                Property = property;
                Value = value;
                Levels = new[] { new LocalValuePriorityLevel(value) };
            }

            public AvaloniaProperty Property { get; }

            public object Value { get; }

            public BindingPriority ValuePriority => BindingPriority.LocalValue;

            public IEnumerable<IPriorityLevel> Levels { get; }
        }


        private class LocalValuePriorityLevel : IPriorityLevel
        {
            public LocalValuePriorityLevel(object value) => DirectValue = value;
            public object DirectValue { get; }
            public BindingPriority Priority => BindingPriority.LocalValue;
            public int ActiveBindingIndex => -1;
            public IEnumerable<IPriorityBindingEntry> Bindings => Enumerable.Empty<IPriorityBindingEntry>();
        }
    }
}
