using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Api
{
    public interface IApiService<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity);
        Task<ApiResponse<TEntity>> CreateAsync(TEntity entity);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}
