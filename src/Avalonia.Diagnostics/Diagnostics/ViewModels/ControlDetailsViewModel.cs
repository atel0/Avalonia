using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.VisualTree;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class ControlDetailsViewModel : ViewModelBase, IDisposable
    {
        private readonly IVisual _control;
        private readonly IDictionary<AvaloniaProperty, PropertyDetails> _propertyIndex;
        private string _propertyFilter;

        public ControlDetailsViewModel(IVisual control)
        {
            _control = control;

            if (control is AvaloniaObject avaloniaObject)
            {
                var properties = AvaloniaPropertyRegistry.Instance.GetRegistered(avaloniaObject)
                    .Concat(AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(avaloniaObject.GetType()))
                    .Select(x => new PropertyDetails(avaloniaObject, x))
                    .OrderBy(x => x, PropertyComparer.Instance)
                    .ThenBy(x => x.IsAttached)
                    .ThenBy(x => x.Name)
                    .ToList();

                _propertyIndex = new Dictionary<AvaloniaProperty, PropertyDetails>();

                foreach (var p in properties)
                {
                    if (!_propertyIndex.ContainsKey(p.Property))
                    {
                        _propertyIndex.Add(p.Property, p);
                    }
                }

                var view = new DataGridCollectionView(properties);
                view.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(PropertyDetails.Group)));
                view.Filter = FilterProperty;
                PropertiesView = view;

                avaloniaObject.PropertyChanged += ControlPropertyChanged;
            }
        }

        public IEnumerable<string> Classes
        {
            get;
            private set;
        }

        public DataGridCollectionView PropertiesView { get; }

        public string PropertyFilter
        {
            get => _propertyFilter;
            set
            {
                if (RaiseAndSetIfChanged(ref _propertyFilter, value))
                {
                    PropertiesView.Refresh();
                }
            }
        }

        public void Dispose()
        {
            if (_control is AvaloniaObject avaloniaObject)
            {
                avaloniaObject.PropertyChanged -= ControlPropertyChanged;
            }
        }

        private void ControlPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_propertyIndex.TryGetValue(e.Property, out var details))
            {
                details.Update();
            }
        }

        private bool FilterProperty(object arg)
        {
            if (!string.IsNullOrWhiteSpace(PropertyFilter) && arg is PropertyDetails property)
            {
                return property.Name.IndexOf(PropertyFilter, StringComparison.OrdinalIgnoreCase) != -1;
            }

            return true;
        }

        private class PropertyComparer : IComparer<PropertyDetails>
        {
            public static PropertyComparer Instance { get; } = new PropertyComparer();

            public int Compare(PropertyDetails x, PropertyDetails y)
            {
                var groupX = GroupIndex(x.Group);
                var groupY = GroupIndex(y.Group);

                if (groupX != groupY)
                {
                    return groupX - groupY;
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            private int GroupIndex(string group)
            {
                switch (group)
                {
                    case "Properties": return 0;
                    case "Attached Properties": return 1;
                    default: return 2;
                }
            }
        }
    }
}
