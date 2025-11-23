using DynamicData.Binding;
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
        private readonly UserService _service;

        public UsersViewModel(UserService userService) : base(e => e.UserId)
        {
            _service = userService;
            RefreshCommand.Execute().Subscribe();
        }

        protected override IEnumerable<string> GetSortOptions()
        {
            return new[]
            {
                "По умолчанию",
                "По имени (А-Я)",
                "По имени (Я-А)",
                "По должности"
            };
        }

        protected override IComparer<User> GetSortComparerForOption(string option)
        {
            return option switch
            {
                "По имени (А-Я)" => SortExpressionComparer<User>
                    .Ascending(e => e.Firstname)
                    .ThenByAscending(e => e.Lastname),

                "По имени (Я-А)" => SortExpressionComparer<User>
                    .Descending(e => e.Firstname)
                    .ThenByDescending(e => e.Lastname),

                "По должности" => SortExpressionComparer<User>
                    .Ascending(e => e.Role)
                    .ThenByAscending(e => e.Firstname),

                _ => GetDefaultSortComparer()
            };
        }

        protected override IComparer<User> GetDefaultSortComparer()
        {
            return SortExpressionComparer<User>
                .Ascending(e => e.UserId);
        }

        protected override string GetSearchableText(User item)
        {
            return $"{item.Firstname} {item.Lastname} {item.Role}";
        }


        protected override async Task LoadDataAsync()
        {
            var users = await _service.GetAllAsync();

            ClearItems();
            foreach (var employee in users)
            {
                AddOrUpdateItem(employee);
            }
        }

        protected override void OnAdd()
        {
            throw new NotImplementedException();
        }

        protected override void OnDelete()
        {
            throw new NotImplementedException();
        }
    }
}
