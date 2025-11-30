using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
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
        public CrudCommandsManager(PageViewModelBase<T> page, 
            AuthorizationService authService,
            IObservable<bool> canDelete)
        {
            CanAdd = authService.HasPermission(Permission.Add);
            CanEdit = authService.HasPermission(Permission.Edit);
            CanDelete = authService.HasPermission(Permission.Delete);

            // Open the add-view panel 
            if (CanAdd)
                AddCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    page.ClearEditFields();
                    page.PanelManager.OpenAdd();
                });

            // Save 
            if (CanEdit)
            {
                EnterEditModeCommand = ReactiveCommand.Create(() => page.PanelManager.EnterEdit());
                SaveCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    if (!page.Validate()) return;

                    var result = await MessageBoxHelper
                        .ShowQuestionMessageBox("Вы подтверждаете сохранение?");
                    if (!result) return; 

                    await page.OnSaveAsync();
                    page.PanelManager.CurrentMode = PanelMode.View;
                }); 
            }

            // Delete
            if (CanDelete)
            {
                DeleteFromPanelCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var result = await MessageBoxHelper
                        .ShowQuestionMessageBox("Вы действительно хотите удалить выбранный элемент?");
                    if (!result) return;

                    await page.OnDeleteAsync();
                    page.PanelManager.IsDetailPanelOpen = false;
                }, canDelete);
            }

            // Exit
            CancelCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (page.CurrentMode == PanelMode.View)
                {
                    page.PanelManager.IsDetailPanelOpen = false;
                    return;
                }

                var result = await MessageBoxHelper
                    .ShowQuestionMessageBox("Вы действительно хотите выйти?\n" +
                    "Несохраненные изменения будут потеряны!");

                if (result) 
                {
                    page.ResetOnCancel();
                    await page.LoadDataAsync();
                }
            });
        }
    }
}
