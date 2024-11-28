using MediatR;

namespace RocketStoreApi.CQRS
{
    /// <summary>
    /// Represents a handler for processing a query of type TQuery
    /// and returning a response of type TResponse
    /// This interface extends IRequestHandler{TQuery, TResponse} to define a handler 
    /// responsible for executing the query and returning the corresponding response.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to be handled. It must implement IQuery.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler. It must not be nullable.</typeparam>
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> { }
}
