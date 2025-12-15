using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.ReactiveModels;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoldenBread.Desktop.ViewModels.Pages;

public class UsersPageViewModel : PageViewModelBase<UserReactive>
{
    private readonly IApiService<User> _userApiService;

    public UsersPageViewModel(ICrudService<UserReactive> userCrudService,
        IApiService<User> userApiService)
        : base(userCrudService, u => u.UserId)
    {
        _userApiService = userApiService;
        DetailsPanel.SetKeySelector(u => u.UserId);
        LoadData();
    }

    private async void LoadData()
    {
        var users = await _userApiService.GetAllAsync(); // Получаем User из API
        var reactiveUsers = users.Select(UserApiService.ToViewModel);
        AddOrUpdateItems(reactiveUsers);
    }

}

/*public class UsersPageViewModelDesigner : PageViewModelBase<User>
{
    private static ICrudService<User> userCrudService;

    public UsersPageViewModelDesigner() : base(userCrudService)
    {
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
        Items.Add(new User
        {
            Lastname = "АЛалалла",
            Email = "dsadads"
        });
    }
}*/

