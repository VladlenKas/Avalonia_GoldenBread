using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.ViewModels.DetailsPanels;
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
        public ObservableCollection<User> Users { get; } = new();

        [Reactive] public User? SelectedUser { get; set; }

        public UsersDetailsPanelViewModel DetailsPanel { get; } = new();

        public ReactiveCommand<Unit, Unit> CreateCommand { get; }

        public UsersPageViewModel()
        {
            LoadSampleData();

            CreateCommand = ReactiveCommand.Create(() => DetailsPanel.ShowCreate());

            this.WhenAnyValue(x => x.SelectedUser)
                .WhereNotNull()
                .Subscribe(m => DetailsPanel.ShowDetails(m));
        }

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
