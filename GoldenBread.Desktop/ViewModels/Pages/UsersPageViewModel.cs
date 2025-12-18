using DynamicData;
using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Domain.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Pages;

public class UsersPageViewModel : PageViewModelBase<User>
{
    // ==== Reactive Properties (для редактирования) ====
    [Reactive] public string EditFirstname { get; set; } = string.Empty;
    [Reactive] public string EditLastname { get; set; } = string.Empty;
    [Reactive] public string EditPatronymic { get; set; } = string.Empty;
    [Reactive] public string EditBirthday { get; set; } = string.Empty;
    [Reactive] public string EditEmail { get; set; } = string.Empty;
    [Reactive] public string EditPassword { get; set; } = string.Empty;
    [Reactive] public UserRoleItem? EditRole { get; set; }
    [Reactive] public VerificationStatusItem? EditStatus { get; set; }

    public IEnumerable<UserRoleItem> AvailableRoles { get; set; }
    public IEnumerable<VerificationStatusItem> AvailableStatuses { get; set; }

    private readonly AuthorizationApiService _authService;

    public UsersPageViewModel(
        IApiService<User> userApiService,
        ICrudService<User> userCrudService,
        AuthorizationApiService authService)
        : base(e => e.UserId, userApiService, userCrudService)
    {
        _authService = authService;

        // Инициализация коллекций
        AvailableRoles = Enum.GetValues<UserRole>()
            .Select(role => new UserRoleItem { Role = role })
            .ToList();

        AvailableStatuses = Enum.GetValues<VerificationStatus>()
            .Select(status => new VerificationStatusItem { Status = status })
            .ToList();

        // Правила валидации
        this.ValidateRequired(this, vm => vm.EditFirstname);
        this.ValidateRequired(this, vm => vm.EditLastname);
        this.ValidateRequired(this, vm => vm.EditBirthday);
        this.ValidateRequired(this, vm => vm.EditEmail);
        this.ValidateRequired(this, vm => vm.EditPassword);
        this.ValidateAge(this, vm => vm.EditBirthday);
        this.ValidateDateFormat(this, vm => vm.EditBirthday);
    }

    // ==== Overrides ====
    protected override void ClearEditFields()
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

    protected override bool CanDelete(User entity)
    {
        return entity.UserId != _authService.CurrentUser?.UserId;
    }

    protected override string GetViewTitle() => "Информация о пользователе";
    protected override string GetEditTitle() => "Редактирование пользователя";
    protected override string GetAddTitle() => "Добавление пользователя";

    protected override string GetDeleteConfirmationMessage() =>
        "Вы действительно хотите уволить выбранного пользователя?";

    public override async Task LoadDataAsync()
    {
        var users = await _apiService.GetAllAsync();
        _sourceCache.Clear();
        _sourceCache.AddOrUpdate(users);
    }
}

