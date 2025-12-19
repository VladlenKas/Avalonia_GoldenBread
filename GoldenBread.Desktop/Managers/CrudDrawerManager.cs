using GoldenBread.Desktop.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoldenBread.Desktop.Managers
{
    public class CrudDrawerManager : ReactiveObject
    {
        // ==== Props ====
        [Reactive] public bool IsOpen { get; set; }

        private DrawerMode _currentMode;
        public DrawerMode CurrentMode
        {
            get => _currentMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentMode, value);
                this.RaisePropertyChanged(nameof(ModeTitle));
                this.RaisePropertyChanged(nameof(ShowViewButtons));
                this.RaisePropertyChanged(nameof(ShowEditButtons));
            }
        }

        public string ModeTitle => CurrentMode switch
        {
            DrawerMode.Add => "Добавление",
            DrawerMode.Edit => "Редактирование",
            DrawerMode.View => "Просмотр",
            _ => string.Empty
        };

        public bool ShowViewButtons => CurrentMode == DrawerMode.View;
        public bool ShowEditButtons => CurrentMode != DrawerMode.View;

        // ==== Methods ====
        public void OpenView()
        {
            CurrentMode = DrawerMode.View;
            IsOpen = true;
        }

        public void OpenAdd()
        {
            CurrentMode = DrawerMode.Add;
            IsOpen = true;
        }

        public void OpenEdit()
        {
            CurrentMode = DrawerMode.Edit;
        }

        public void Close()
        {
            CurrentMode = DrawerMode.None;
            IsOpen = false;
        }
    }
}
