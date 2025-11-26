using ReactiveUI;
using ReactiveUI.Validation.Helpers;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class TopbarItem : ReactiveValidationObject
    {
        private bool _isSelected;

        public string Title { get; set; }
        public object ViewModel { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public TopbarItem(string title, object viewModel)
        {
            Title = title;
            ViewModel = viewModel;
        }
    }
}
