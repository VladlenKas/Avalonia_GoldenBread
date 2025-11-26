using GoldenBread.Desktop.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Interfaces
{
    public interface IDetailPanelViewModel
    {
        bool IsDetailPanelOpen { get; set; }
        PanelMode CurrentMode { get; set; }
        string ModeTitle { get; }
        bool ShowViewButtons { get; }
        bool ShowEditButtons { get; }
        bool CanAdd { get; }
        bool CanEdit { get; }
        bool CanDelete { get; }

        ReactiveCommand<Unit, Unit>? AddCommand { get; }
        ReactiveCommand<Unit, Unit>? SaveCommand { get; }
        ReactiveCommand<Unit, Unit>? EnterEditModeCommand { get; }
        ReactiveCommand<Unit, Unit>? DeleteFromPanelCommand { get; }
        ReactiveCommand<Unit, Unit>? CancelCommand { get; }

        void ClearEditFields();
        void ResetOnCancel();
    }
}
