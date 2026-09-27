using R3;

namespace Core.Loading
{
    public sealed class LoadingModel : IReadOnlyLoadingModel
    {
        public ReactiveProperty<bool> IsLoading { get; } = new ReactiveProperty<bool>(true);

        ReadOnlyReactiveProperty<bool> IReadOnlyLoadingModel.IsLoading => IsLoading;
    }
}
