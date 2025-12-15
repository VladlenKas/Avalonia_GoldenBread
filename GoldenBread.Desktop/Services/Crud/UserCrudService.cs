using GoldenBread.Desktop.Services.Api;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.ReactiveModels;
using GoldenBread.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Crud
{
    public class UserCrudService(IApiService<User> userApiService) : ICrudService<UserReactive>
    {
        public UserReactive Clone(UserReactive entity) => UserApiService.Clone(entity);

        public bool Validate(UserReactive entity) =>
            !string.IsNullOrWhiteSpace(entity.Firstname) &&
            !string.IsNullOrWhiteSpace(entity.Email);

        public async Task<bool> SaveAsync(UserReactive entity)
        {
            var user = UserApiService.ToModel(entity);

            if (entity.UserId == 0) 
                await userApiService.CreateAsync(user);
            else
                await userApiService.UpdateAsync(user);

            return true;
        }

        public async Task<bool> DeleteAsync(UserReactive entity)
        {
            await userApiService.DeleteAsync(entity.UserId);

            return true;
        }
    }
}
