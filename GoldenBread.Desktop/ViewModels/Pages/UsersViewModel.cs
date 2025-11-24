using Avalonia.Controls;
using DynamicData.Binding;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Shared.Entities;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Pages
{
    public class UsersViewModel : PageViewModelBase<User>
    {
        // == Fields ==
        private readonly UserService _userService;


        // == Designer ==
        public UsersViewModel(UserService userService, 
            AuthorizationService service) : base(e => e.UserId, service)
        {
            _userService = userService;

            RefreshCommand.Execute().Subscribe();
        }


        // == Override Methods ==
        protected override string GetSearchableText(User user)
        {
            return $"{user.Firstname} {user.Lastname} {user.Role}";
        }

        protected override async Task LoadDataAsync()
        {
            var users = await _userService.GetAllAsync();
            ClearItems();
            foreach (var employee in users)
            {
                AddOrUpdateItem(employee);
            }
        }

        protected override bool CanDeleteOptions()
        {
            return SelectedItem.UserId != _service.CurrentUser.UserId;
        }

        // == Commands Methods ==
        protected override async Task OnSaveAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task OnDeleteAsync()
        {
            var result = await MessageBoxHelper.ShowQuestionMessageBox("Вы действительно хотите уволить " +
                "выбранного пользователя?");

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


    // == Designer For View ==
    public class DesignUsersViewModel : PageViewModelBase<User>
    {
        public DesignUsersViewModel() : base(e => e.UserId, service: null)
        {
            var users = new List<User>
            {
                new User { UserId = 1, Firstname = "Иван", Lastname = "Иванов", Email = "ivan@gmail.com", Role = UserRole.Admin },
                new User { UserId = 2, Firstname = "Никита", Lastname = "Никитов", Email = "nikita@gmail.com", Role = UserRole.Admin },
                new User { UserId = 3, Firstname = "Мария", Lastname = "Петрова", Email = "maria@gmail.com", Role = UserRole.ManagerProduction },
            };

            foreach (var user in users)
            {
                AddOrUpdateItem(user);
            }
        }

        protected override Task LoadDataAsync() => Task.CompletedTask;
        protected override async Task OnAddAsync() { }
        protected override async Task OnSaveAsync() { }
        protected override async Task OnDeleteAsync() { }
    }
}
