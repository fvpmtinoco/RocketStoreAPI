using MediatR;

namespace RocketStoreApi.CQRS
{
    public interface ICommand<out TResponse> : IRequest<TResponse> { }

    // For readability purposes only, as it is not necessary to specify the return type as Unit (void)
    public interface ICommand : IRequest<Unit> { }
}
