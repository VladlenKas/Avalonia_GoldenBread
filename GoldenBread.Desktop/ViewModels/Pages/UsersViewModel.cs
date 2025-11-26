using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Domain.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Pages
{
    public class UsersViewModel : PageViewModelBase<User>
    {
        // ==== Fields ====
        private readonly UserService _userService;
        private readonly AuthorizationService _authService;


        // ==== Props ====
        [Reactive] public string EditFirstname { get; set; }
        [Reactive] public string EditLastname { get; set; }
        [Reactive] public string EditEmail { get; set; }
        [Reactive] public UserRole EditRole { get; set; }



        // ==== Designer ====
        public UsersViewModel(UserService userService, 
            AuthorizationService service,
            AuthorizationService authorizationService) 
            : base(e => e.UserId, service)
        {
            _userService = userService;
            _authService = authorizationService;

            NotEmpty(this, vm => vm.EditFirstname);
            NotEmpty(this, vm => vm.EditLastname);
            NotEmpty(this, vm => vm.EditEmail);

            Initialize();
        }


        // ==== Override Methods ====
        protected override void CopyToEditFields(User user)
        {
            EditFirstname = user.Firstname;
            EditLastname = user.Lastname;
            EditEmail = user.Email;
        }

        protected override void ClearEditFields()
        {
            EditFirstname = string.Empty;
            EditLastname = string.Empty;
            EditEmail = string.Empty;
        }

        protected override string GetSearchableText(User user) 
            => $"{user.Firstname} {user.Lastname} {user.RoleValue}";

        protected override bool GetDeleteOptions()
        {
            return SelectedItem.UserId != _authService.CurrentUser.UserId;
        }

        protected override async Task LoadDataAsync()
        {
            var users = await _userService.GetAllAsync();
            _sourceCache.Clear();
            _sourceCache.AddOrUpdate(users);
        }


        // ==== Commands Methods ====
        protected override async Task OnSaveAsync()
        {
            if (CurrentMode == PanelMode.Add)
            {
                var result = await _userService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                    AddOrUpdateItem(result.Data);
                else
                    await MessageBoxHelper.ShowErrorMessageBox(result.Message);
            }
            else if (CurrentMode == PanelMode.Edit)
            {
                var result = await _userService.UpdateAsync(SelectedItem);
                if (result.IsSuccess)
                    AddOrUpdateItem(result.Data);
                else
                    await MessageBoxHelper.ShowErrorMessageBox(result.Message);
            }
        }

        protected override async Task OnDeleteAsync()
        {
            var result = await MessageBoxHelper.ShowQuestionMessageBox(
                "Вы действительно хотите уволить выбранного пользователя?");

            if (result)
            {
                var resultResponse = await _userService.DeleteAsync(SelectedItem.UserId);
                if (resultResponse.IsSuccess)
                {
                    RemoveItem(SelectedItem);
                    await MessageBoxHelper.ShowOkMessageBox(resultResponse.Message);
                }
                else
                    await MessageBoxHelper.ShowErrorMessageBox(resultResponse.Message);
            }
        }
    }


    // ==== Designer For View ====
    public class DesignUsersViewModel : UsersViewModel
    {
        public DesignUsersViewModel() : base(null!, null!, null!)
        {
            var users = new List<User>
            {
                new User { UserId = 1, Firstname = "Иван", Lastname = "Иванов", 
                    Email = "ivan@gmail.com", Role = UserRole.Admin },
                new User { UserId = 2, Firstname = "Никита", Lastname = "Никитов", 
                    Email = "nikita@gmail.com", Role = UserRole.Admin },
                new User { UserId = 3, Firstname = "Мария", Lastname = "Петрова", 
                    Email = "maria@gmail.com", Role = UserRole.ManagerProduction },
            };

            foreach (var user in users)
            {
                AddOrUpdateItem(user);
            }

            SelectedItem = users.First();
            IsDetailPanelOpen = true;
            CurrentMode = PanelMode.View;
        }

        protected override Task LoadDataAsync() => Task.CompletedTask;
        protected override Task OnSaveAsync() => Task.CompletedTask;
        protected override Task OnDeleteAsync() => Task.CompletedTask;
    }
}
