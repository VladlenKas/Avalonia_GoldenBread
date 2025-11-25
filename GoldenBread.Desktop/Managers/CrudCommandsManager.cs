using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Managers
{
    public class CrudCommandsManager<T> where T : class
    {
        // ==== Props ====
        public ReactiveCommand<Unit, Unit>? AddCommand { get; }
        public ReactiveCommand<Unit, Unit>? SaveCommand { get; }
        public ReactiveCommand<Unit, Unit>? EnterEditModeCommand { get; }
        public ReactiveCommand<Unit, Unit>? DeleteFromPanelCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public bool CanAdd { get; }
        public bool CanEdit { get; }
        public bool CanDelete { get; }


        // ==== Designer ====
        public CrudCommandsManager(
            DetailPanelManager panelManager,
            AuthorizationService authService,
            Func<Task> saveAction,
            Func<Task> deleteAction,
            Func<Task> refreshAction,
            IObservable<bool> canDelete)
        {
            CanAdd = authService.HasPermission(Permission.Add);
            CanEdit = authService.HasPermission(Permission.Edit);
            CanDelete = authService.HasPermission(Permission.Delete);

            if (CanAdd)
                AddCommand = ReactiveCommand.CreateFromTask(async () => panelManager.OpenAdd());

            if (CanEdit)
            {
                EnterEditModeCommand = ReactiveCommand.Create(() => panelManager.EnterEdit());
                SaveCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    await saveAction();
                    panelManager.CurrentMode = PanelMode.View;
                    await refreshAction();
                });
            }

            if (CanDelete)
            {
                DeleteFromPanelCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    await deleteAction();
                    panelManager.IsDetailPanelOpen = false;
                    await refreshAction();
                }, canDelete);
            }

            CancelCommand = ReactiveCommand.Create(() =>
            {
                if (panelManager.CurrentMode == PanelMode.Edit)
                {
                    panelManager.CurrentMode = PanelMode.View;
                }
                else
                {
                    panelManager.IsDetailPanelOpen = false;
                }
            });
        }
    }
}
