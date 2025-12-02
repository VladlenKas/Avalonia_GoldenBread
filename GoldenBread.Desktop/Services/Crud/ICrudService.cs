using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services.Crud
{
    public interface ICrudService<TEntity>
    {
        TEntity Clone(TEntity entity);
        bool Validate(TEntity entity);
        Task<bool> SaveAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
    }
}   
