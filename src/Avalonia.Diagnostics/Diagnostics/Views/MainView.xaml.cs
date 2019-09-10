using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Avalonia.Diagnostics.Views
{
    internal class MainView : UserControl
    {
        private const int ConsoleRow = 5;
        private readonly ConsoleView _console;
        private readonly GridSplitter _consoleSplitter;
        private readonly Grid _rootGrid;
        private double _consoleHeight = -1;

        public MainView()
        {
            InitializeComponent();
            AddHandler(KeyDownEvent, PreviewKeyDown, RoutingStrategies.Tunnel);
            _console = this.FindControl<ConsoleView>("console");
            _consoleSplitter = this.FindControl<GridSplitter>("consoleSplitter");
            _rootGrid = this.FindControl<Grid>("rootGrid");
        }

        public void ToggleConsole()
        {
            var vm = (MainViewModel)DataContext;

            if (_consoleHeight == -1)
            {
                _consoleHeight = Bounds.Height / 3;
            }

            vm.Console.ToggleVisibility();
            _consoleSplitter.IsEnabled = vm.Console.IsVisible;

            if (vm.Console.IsVisible)
            {
                _rootGrid.RowDefinitions[ConsoleRow].Height = new GridLength(_consoleHeight, GridUnitType.Pixel);
                Dispatcher.UIThread.Post(() => _console.FocusInput(), DispatcherPriority.Background);
            }
            else
            {
                _consoleHeight = _rootGrid.RowDefinitions[ConsoleRow].Height.Value;
                _rootGrid.RowDefinitions[ConsoleRow].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ToggleConsole();
                e.Handled = true;
            }
        }
    }
}
