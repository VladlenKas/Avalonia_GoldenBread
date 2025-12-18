using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Crud
{
    public interface ICrudService<TEntity> where TEntity : class
    {
        TEntity Clone(TEntity entity);
        Task<ApiResponse<TEntity>> SaveAsync(TEntity entity, bool isNew);
        Task<ApiResponse<object>> DeleteAsync(TEntity entity);
        void MapToViewModel(TEntity entity, object viewModel);
        TEntity MapFromViewModel(object viewModel, TEntity? existingEntity = null);
    }
}   
