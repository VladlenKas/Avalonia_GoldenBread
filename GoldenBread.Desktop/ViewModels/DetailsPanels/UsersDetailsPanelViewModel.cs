using Castle.Core.Resource;
using GoldenBread.Desktop.ViewModels.DetailsPanels;
using GoldenBread.Domain.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.DetailsPanels
{
    public class UsersDetailsPanelViewModel : DetailsPanelViewModel<User>
    {
        [Reactive] public string HeaderTitle { get; private set; } = string.Empty;

        public UsersDetailsPanelViewModel()
        {
            // Заголовок можно сделать реактивным, если нужно
            this.WhenAnyValue(x => x.Mode)
                .Subscribe(mode =>
                {
                    HeaderTitle = mode switch
                    {
                        PanelMode.View => "Просмотр клиента",
                        PanelMode.Edit => "Редактирование клиента",
                        PanelMode.Create => "Новый клиент",
                        _ => string.Empty
                    };
                });
        }

        protected override User CloneEntity(User entity)
        {
            return new User
            {
                UserId = entity.UserId,
                Firstname = entity.Firstname,
                Lastname = entity.Lastname,
                Email = entity.Email,
                Password = entity.Password
            };
        }

        protected override bool ValidateEntity(User entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Firstname) &&
                   !string.IsNullOrWhiteSpace(entity.Email);
        }

        protected override async Task SaveEntityAsync(User entity)
        {
            // Здесь можно вызвать сервис сохранения
            // Пока просто задержка для имитации работы
            await Task.Delay(200);
        }

        protected override async Task DeleteEntityAsync(User entity)
        {
            // Здесь вызов сервиса удаления
            await Task.Delay(200);
        }
    }
}
