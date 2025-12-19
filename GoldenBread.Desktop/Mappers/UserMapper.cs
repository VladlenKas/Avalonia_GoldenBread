using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Mappers
{
    public class UserMapper : IMapper<User>
    {
        public void MapEntityToViewModel(User entity, object viewModel)
        {
            if (viewModel is UsersPageViewModel vm)
            {
                vm.EditFirstname = entity.Firstname;
                vm.EditLastname = entity.Lastname;
                vm.EditPatronymic = entity.Patronymic;
                vm.EditBirthday = entity.Birthday.ToDateString();
                vm.EditEmail = entity.Email;
                vm.EditPassword = entity.Password;
                vm.EditRole = vm.AvailableRoles.FirstOrDefault(x => x.Role == entity.Role);
                vm.EditStatus = vm.AvailableStatuses.FirstOrDefault(x => x.Status == entity.VerificationStatus);
            }
        }

        public User MapEntityFromViewModel(object viewModel, User? existingEntity = null)
        {
            var user = existingEntity ?? new User();

            if (viewModel is UsersPageViewModel vm)
            {
                user.Firstname = vm.EditFirstname;
                user.Lastname = vm.EditLastname;
                user.Patronymic = vm.EditPatronymic;
                user.Birthday = vm.EditBirthday.ToDateOnly();
                user.Email = vm.EditEmail;
                user.Password = vm.EditPassword;
                user.Role = vm.EditRole?.Role ?? UserRole.Admin;
                user.VerificationStatus = vm.EditStatus?.Status ?? VerificationStatus.Pending;
                user.AccountType = AccountType.User;
            }

            return user;
        }
    }
}
