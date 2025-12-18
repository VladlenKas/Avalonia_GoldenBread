using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Crud
{
    public class UserCrudService(IApiService<User> userApiService) : ICrudService<User>
    {
        public User Clone(User entity)
        {
            return new User
            {
                UserId = entity.UserId,
                Firstname = entity.Firstname,
                Lastname = entity.Lastname,
                Patronymic = entity.Patronymic,
                Birthday = entity.Birthday,
                Email = entity.Email,
                Password = entity.Password,
                Role = entity.Role,
                VerificationStatus = entity.VerificationStatus,
                AccountType = entity.AccountType
            };
        }

        public async Task<ApiResponse<User>> SaveAsync(User entity, bool isNew)
        {
            if (isNew)
            {
                return await userApiService.CreateAsync(entity);
            }
            else
            {
                return await userApiService.UpdateAsync(entity);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(User entity)
        {
            return await userApiService.DeleteAsync(entity.UserId);
        }

        public void MapToViewModel(User entity, object viewModel)
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

        public User MapFromViewModel(object viewModel, User? existingEntity = null)
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
