using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity);
        Task<ApiResponse<TEntity>> CreateAsync(TEntity entity);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}
