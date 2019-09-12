using Avalonia.Data;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class BindingDetailsViewModel : ViewModelBase
    {
        private readonly IPriorityBindingEntry _binding;

        public BindingDetailsViewModel(
            IPriorityBindingEntry binding,
            IPriorityLevel level,
            BindingPriority activePriority)
        {
            _binding = binding;
            Priority = level.Priority;
            IsActive = level.Priority == activePriority && level.ActiveBindingIndex == binding.Index;
        }

        public string Description => _binding.Description ?? _binding.Observable.GetType().Name;
        public object Value => _binding.Value;
        public BindingPriority Priority { get; }
        public bool IsActive { get; }
    }
}
