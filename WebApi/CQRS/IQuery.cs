using MediatR;

namespace RocketStoreApi.CQRS
{
    /// <summary>
    /// Represents a query that returns a response of type TResponse
    /// This interface extends IRequest to define a request for data that 
    /// can be processed by a handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response expected from the query. It must not be nullable.</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> { } //where TResponse : notnull { }
}
