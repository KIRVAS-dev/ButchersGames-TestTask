using Infrastructure.ExtendedExceptions;

namespace Core.Loading
{
    public sealed class LoadingService : ILoadingService
    {
        private readonly LoadingModel _model;

        public LoadingService(LoadingModel model)
        {
            _model = model;
        }

        void ILoadingService.Complete()
        {
            Guard.AgainstTrue(!_model.IsLoading.Value, () => new InvalidLoadingCompletionException());

            _model.IsLoading.Value = false;
        }
    }
}
