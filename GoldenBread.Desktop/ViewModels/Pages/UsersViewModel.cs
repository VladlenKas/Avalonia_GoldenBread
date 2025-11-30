using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
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
    public class UsersViewModel : PageViewModelBase<User>, IDetailPanelViewModel
    {
        // ==== Fields ====
        private readonly UserService _userService;
        private readonly AuthorizationService _authService;


        // ==== Props ====
        [Reactive] public string EditFirstname { get; set; }
        [Reactive] public string EditLastname { get; set; }
        [Reactive] public string EditPatronymic { get; set; }
        [Reactive] public string EditBirthday { get; set; }
        [Reactive] public string EditEmail { get; set; }
        [Reactive] public string EditPassword { get; set; }
        [Reactive] public UserRoleItem? EditRole { get; set; }
        [Reactive] public VerificationStatusItem? EditStatus { get; set; }

        public IEnumerable<UserRoleItem> AvailableRoles { get; set; } // Collection roles
        public IEnumerable<VerificationStatusItem> AvailableStatuses { get; set; } // Collection statuses


        // ==== Designer ====
        public UsersViewModel(UserService userService, 
            AuthorizationService service,
            AuthorizationService authorizationService) 
            : base(e => e.UserId, service)
        {
            _userService = userService;
            _authService = authorizationService;

            AvailableRoles = Enum.GetValues<UserRole>()
                .Select(role => new UserRoleItem { Role = role })
                .ToList();

            AvailableStatuses = Enum.GetValues<VerificationStatus>()
                .Select(status => new VerificationStatusItem { Status = status })
                .ToList();

            this.ValidateRequired(this, vm => vm.EditFirstname);
            this.ValidateRequired(this, vm => vm.EditLastname);
            this.ValidateRequired(this, vm => vm.EditBirthday);
            this.ValidateRequired(this, vm => vm.EditEmail);
            this.ValidateRequired(this, vm => vm.EditPassword);

            this.ValidateAge(this, vm => vm.EditBirthday);
            this.ValidateDateFormat(this, vm => vm.EditBirthday);

            this.Initialize();
        }


        // ==== Override Methods ====
        protected override void CopyToEditFields(User user)
        {
            EditFirstname = user.Firstname;
            EditLastname = user.Lastname;
            EditPatronymic = user.Patronymic;
            EditBirthday = user.Birthday.ToDateString(); 
            EditEmail = user.Email;
            EditPassword = user.Password;
            EditRole = AvailableRoles.FirstOrDefault(x => x.Role == user.Role);
            EditStatus = AvailableStatuses.FirstOrDefault(x => x.Status == user.VerificationStatus);
        }

        public override void ClearEditFields()
        {
            EditFirstname = string.Empty;
            EditLastname = string.Empty;
            EditPatronymic = string.Empty;
            EditBirthday = string.Empty;
            EditEmail = string.Empty;
            EditPassword = string.Empty;
            EditRole = null;
            EditStatus = null;
        }

        protected override string GetSearchableText(User user) 
            => $"{user.Firstname} {user.Lastname} {user.RoleValue}";
            
        protected override bool GetDeleteOptions()
        {
            return SelectedItem.UserId != _authService.CurrentUser.UserId;
        }

        public override async Task LoadDataAsync()
        {
            var users = await _userService.GetAllAsync();
            _sourceCache.Clear();
            _sourceCache.AddOrUpdate(users);
        }


        // ==== Commands Methods ====
        public override async Task OnSaveAsync()
        {
            var request = new UserRequest
            {
                Firstname = EditFirstname,
                Lastname = EditLastname,
                Patronymic = EditPatronymic,
                Birthday = EditBirthday.ToDateOnly(),
                Email = EditEmail,
                Password = EditPassword,
                Role = EditRole.Role,
                VerificationStatus = EditStatus.Status,
                AccountType = AccountType.User
            };

            if (CurrentMode == PanelMode.Add)
            {
                var result = await _userService.CreateAsync(request);
                if (result.IsSuccess) { AddOrUpdateItem(result.Data); }
                else { await MessageBoxHelper.ShowErrorMessageBox(result.Message); }
            }
            else if (CurrentMode == PanelMode.Edit)
            {
                request.UserId = SelectedItem.UserId;

                var result = await _userService.UpdateAsync(request);
                if (result.IsSuccess) { AddOrUpdateItem(result.Data); }
                else { await MessageBoxHelper.ShowErrorMessageBox(result.Message); }
            }

            this.DeactivateValidation();
        }

        public override async Task OnDeleteAsync()
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
                else { await MessageBoxHelper.ShowErrorMessageBox(resultResponse.Message); }
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

        public override Task LoadDataAsync() => Task.CompletedTask;
        public override Task OnSaveAsync() => Task.CompletedTask;
        public override Task OnDeleteAsync() => Task.CompletedTask;
    }
}
