using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Domain.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Pages
{
    public class UsersPageViewModel : ViewModelBase
    {
        // ==== Services ====
        private readonly ICrudService<User> _userCrudService;

        // ==== Props ====
        [Reactive] public User? SelectedUser { get; set; }
        public ObservableCollection<User> Users { get; } = new();
        public DetailsPanelViewModel<User> DetailsPanel { get; }

        // ==== Commands ====
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }

        // ==== Designer ====
        public UsersPageViewModel(ICrudService<User> userCrudService)
        {
            _userCrudService = userCrudService;

            LoadSampleData();

            DetailsPanel = new DetailsPanelViewModel<User>(_userCrudService);
            CreateCommand = ReactiveCommand.Create(() => DetailsPanel.ShowCreate());

            this.WhenAnyValue(x => x.SelectedUser)
                .WhereNotNull()
                .Subscribe(m => DetailsPanel.ShowDetails(m));
        }

        // ==== Methods ====
        private void LoadSampleData()
        {
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
            Users.Add(new User
            {
                Lastname = "АЛалалла",
                Email = "dsadads"
            });
        }
    }
}
