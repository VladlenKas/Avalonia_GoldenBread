using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Domain.Models;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoldenBread.Desktop.ViewModels.Pages;

public class UsersPageViewModel : PageViewModelBase<User>
{
    private readonly IApiService<User> _userApiService;

    public UsersPageViewModel(
        ICrudService<User> userCrudService,
        IApiService<User> userApiService)
        : base(userCrudService, u => u.UserId)
    {
        _userApiService = userApiService;
        DetailsPanel.SetKeySelector(u => u.UserId);
        LoadData();
    }

    private async void LoadData()
    {
        var users = await _userApiService.GetAllAsync(); 
        AddOrUpdateItems(users);
    }
}

