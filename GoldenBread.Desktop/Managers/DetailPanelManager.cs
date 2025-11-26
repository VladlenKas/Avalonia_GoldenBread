using GoldenBread.Desktop.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Managers
{
    public class DetailPanelManager : ReactiveObject
    {
        // ==== Props ====
        [Reactive] public bool IsDetailPanelOpen { get; set; }
        [Reactive] public PanelMode CurrentMode { get; set; }

        public string ModeTitle => CurrentMode switch
        {
            PanelMode.Add => "Добавление",
            PanelMode.Edit => "Редактирование",
            _ => "Просмотр"
        };

        public bool ShowViewButtons => CurrentMode == PanelMode.View;
        public bool ShowEditButtons => CurrentMode != PanelMode.View;


        // ==== Methods ====
        public void OpenView()
        {
            CurrentMode = PanelMode.View;
            IsDetailPanelOpen = true;
        }

        public void OpenAdd()
        {
            CurrentMode = PanelMode.Add;
            IsDetailPanelOpen = true;
        }

        public void EnterEdit() => CurrentMode = PanelMode.Edit;
    }
}
