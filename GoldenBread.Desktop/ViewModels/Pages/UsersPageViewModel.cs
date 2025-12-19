using AutoMapper;
using DynamicData;
using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Mappers;
using GoldenBread.Desktop.Repositories;
using GoldenBread.Desktop.Services;
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

    private readonly AuthService _authService;

    public UsersPageViewModel(
        IService<User> service,
        IMapper<User> mapper,
        AuthService authService)
        : base(e => e.UserId, service, mapper)
    {
        _authService = authService;

        InitializeCollections();
        InitializeValidationRules();
        Initialize();
    }

    private void InitializeCollections()
    {
        AvailableRoles = Enum.GetValues<UserRole>()
            .Select(role => new UserRoleItem { Role = role })
            .ToList();

        AvailableStatuses = Enum.GetValues<VerificationStatus>()
            .Select(status => new VerificationStatusItem { Status = status })
            .ToList();
    }

    private void InitializeValidationRules()
    {
        ValidateRequired(this, vm => vm.EditFirstname);
        ValidateRequired(this, vm => vm.EditLastname);
        ValidateRequired(this, vm => vm.EditBirthday);
        ValidateRequired(this, vm => vm.EditEmail);
        ValidateRequired(this, vm => vm.EditPassword);
        ValidateAge(this, vm => vm.EditBirthday);
        ValidateDateFormat(this, vm => vm.EditBirthday);
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

    public override async Task LoadDataAsync()
    {
        var users = await _service.GetAllAsync();
        _sourceCache.Clear();
        _sourceCache.AddOrUpdate(users);
    }
}

