namespace GoldenBread.Desktop.Mappers
{
    public interface IMapper<TEntity> where TEntity : class
    {
        void MapEntityToViewModel(TEntity entity, object viewModel);
        TEntity MapEntityFromViewModel(object viewModel, TEntity? existingEntity = null);
    }
}
