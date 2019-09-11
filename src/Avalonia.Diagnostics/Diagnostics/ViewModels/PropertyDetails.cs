// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class PropertyDetails : ViewModelBase
    {
        private AvaloniaObject _target;
        private object _value;
        private string _priority;
        private TypeConverter _converter;
        private string _group;

        public PropertyDetails(AvaloniaObject o, AvaloniaProperty property)
        {
            _target = o;
            Property = property;

            Name = property.IsAttached ?
                $"[{property.OwnerType.Name}.{property.Name}]" :
                property.Name;

            if (property.IsDirect)
            {
                Group = "Properties";
                Priority = "Direct";
            }

            Update();
        }

        public AvaloniaProperty Property { get; }

        public string Name { get; }

        public bool IsAttached => Property.IsAttached;

        public string Priority
        {
            get { return _priority; }
            private set { RaiseAndSetIfChanged(ref _priority, value); }
        }

        public string Value
        {
            get
            {
                if (_value == null)
                {
                    return "(null)";
                }

                return Converter?.CanConvertTo(typeof(string)) == true ?
                    Converter.ConvertToString(_value) :
                    _value.ToString();
            }
            set
            {
                try
                {
                    var convertedValue = Converter?.CanConvertFrom(typeof(string)) == true ?
                        Converter.ConvertFromString(value) :
                        DefaultValueConverter.Instance.ConvertBack(value, Property.PropertyType, null, CultureInfo.CurrentCulture);
                    _target.SetValue(Property, convertedValue);
                }
                catch { }
            }
        }

        public string Group
        {
            get { return _group; }
            private set { RaiseAndSetIfChanged(ref _group, value); }
        }

        private TypeConverter Converter
        {
            get
            {
                if (_converter == null)
                {
                    _converter = TypeDescriptor.GetConverter(_value.GetType());
                }

                return _converter;
            }
        }
        
        public void Update()
        {
            if (Property.IsDirect)
            {
                RaiseAndSetIfChanged(ref _value, _target.GetValue(Property), nameof(Value));
            }
            else
            {
                var val = _target.GetDiagnostic(Property);

                RaiseAndSetIfChanged(ref _value, val?.Value, nameof(Value));

                if (val != null)
                {
                    Group = IsAttached ? "Attached Properties" : "Properties";
                    Priority = val.ValuePriority.ToString();
                }
                else
                {
                    Group = Priority = "Unset";
                }
            }
        }
    }
}
