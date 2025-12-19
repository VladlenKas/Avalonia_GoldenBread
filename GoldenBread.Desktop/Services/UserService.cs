using GoldenBread.Desktop.Repositories;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class UserService(IRepository<User> repository) : IService<User>
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

        public async Task<List<User>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<ApiResponse<User>> SaveAsync(User entity, bool isNew)
        {
            if (isNew)
            {
                return await repository.CreateAsync(entity);
            }
            else
            {
                return await repository.UpdateAsync(entity);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(User entity)
        {
            return await repository.DeleteAsync(entity.UserId);
        }
    }
}
