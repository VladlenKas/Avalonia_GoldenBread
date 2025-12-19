using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public interface IService<TEntity> where TEntity : class
    {
        TEntity Clone(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<ApiResponse<TEntity>> SaveAsync(TEntity entity, bool isNew);
        Task<ApiResponse<object>> DeleteAsync(TEntity entity);
    }
}   
