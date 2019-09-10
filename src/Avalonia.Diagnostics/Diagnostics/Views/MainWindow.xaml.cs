using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.Diagnostics.Views
{
    internal class MainWindow : Window, IStyleHost
    {
        private TopLevel _root;

        public MainWindow()
        {
            InitializeComponent();
        }

        public TopLevel Root
        {
            get => _root;
            set
            {
                if (_root != value)
                {
                    _root = value;
                    DataContext = new MainViewModel(value);
                }
            }
        }

        IStyleHost IStyleHost.StylingParent => null;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
