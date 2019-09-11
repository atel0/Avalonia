using System;
using Avalonia.Controls;
using Avalonia.Diagnostics.Models;
using Avalonia.Input;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly IControl _root;
        private readonly TreePageViewModel _logicalTree;
        private readonly TreePageViewModel _visualTree;
        private TreePageViewModel _content;
        private int _selectedTab;
        private string _focusedControl;
        private string _pointerOverElement;

        public MainViewModel(IControl root)
        {
            _root = root;
            _logicalTree = new TreePageViewModel(LogicalTreeNode.Create(root));
            _visualTree = new TreePageViewModel(VisualTreeNode.Create(root));

            UpdateFocusedControl();
            KeyboardDevice.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(KeyboardDevice.Instance.FocusedElement))
                {
                    UpdateFocusedControl();
                }
            };

            SelectedTab = 0;
            root.GetObservable(TopLevel.PointerOverElementProperty)
                .Subscribe(x => PointerOverElement = x?.GetType().Name);
            Console = new ConsoleViewModel(UpdateConsoleContext);
        }

        public ConsoleViewModel Console { get; }

        public TreePageViewModel Content
        {
            get { return _content; }
            private set
            {
                if (_content?.SelectedNode?.Visual is IControl control)
                {
                    value.SelectControl(control);
                }

                RaiseAndSetIfChanged(ref _content, value);
            }
        }

        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;

                switch (value)
                {
                    case 0:
                        Content = _logicalTree;
                        break;
                    case 1:
                        Content = _visualTree;
                        break;
                }

                RaisePropertyChanged();
            }
        }

        public string FocusedControl
        {
            get { return _focusedControl; }
            private set { RaiseAndSetIfChanged(ref _focusedControl, value); }
        }

        public string PointerOverElement
        {
            get { return _pointerOverElement; }
            private set { RaiseAndSetIfChanged(ref _pointerOverElement, value); }
        }

        private void UpdateConsoleContext(ConsoleContext context)
        {
            context.root = _root;
            context.e = Content.SelectedNode?.Visual;
        }

        public void SelectControl(IControl control)
        {
            var tree = Content as TreePageViewModel;

            if (tree != null)
            {
                tree.SelectControl(control);
            }
        }

        public void Dispose()
        {
            _logicalTree.Dispose();
            _visualTree.Dispose();
        }

        private void UpdateFocusedControl()
        {
            FocusedControl = KeyboardDevice.Instance.FocusedElement?.GetType().Name;
        }
    }
}
