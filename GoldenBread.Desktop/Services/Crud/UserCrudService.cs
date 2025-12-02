using GoldenBread.Desktop.Services.Api;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Crud
{
    public class UserCrudService(IApiService<User> userApiService) : ICrudService<User>
    {
        public User Clone(User entity) => new User
        {
            UserId = entity.UserId,
            Firstname = entity.Firstname,
            Lastname = entity.Lastname,
            Email = entity.Email,
            Password = entity.Password
        };

        public bool Validate(User entity) =>
            !string.IsNullOrWhiteSpace(entity.Firstname) &&
            !string.IsNullOrWhiteSpace(entity.Email);

        public async Task<bool> SaveAsync(User entity)
        {
            if (entity.UserId == 0) 
                await userApiService.CreateAsync(entity);
            else
                await userApiService.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(User entity)
        {
            await userApiService.DeleteAsync(entity.UserId);

            return true;
        }
    }
}
